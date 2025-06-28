using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ROS2;
using topicSt = std_msgs.msg.String;
using twist = geometry_msgs.msg.Twist;
//using Ts = twistring.msg.Twistring;
using TS = geometry_msgs.msg.TwistStamped;
using Int32 = std_msgs.msg.Int32;
using Co = nhk2025b_msgs.msg.Command;
using TMPro;
public class UnityPublisher : MonoBehaviour
{
    [SerializeField] private GameObject controllerActions;
    [SerializeField] private GameObject uiope;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    //private IPublisher<Ts> pub;
    //private int target_num = 0;
    [System.NonSerialized] public Queue<string> queue = new Queue<string>();
    [System.NonSerialized] public Queue<int> intQueue = new Queue<int>();
    [System.NonSerialized] public Queue<twist> twistmsgs = new Queue<twist>();
    //private IEnumerator routine;
    //[SerializeField] private string twistringTopicName = "/Twistring_topic_name";
    [SerializeField] private string twiststampedTopicName = "/TwistStamped_topic_name";
    [SerializeField] private float pub_hz = 0.05f;
    private bool is_auto;

    public FixedJoystick XYJoy;
    public FixedJoystick ZJoy;
    private Vector2 leftdsjoy;
    private Vector2 rightdsjoy;
    private IPublisher<TS> joy_pub;
    private IEnumerator joy_routine;

    //Publish Status
    private IEnumerator statusRoutine;
    private IPublisher<Int32> status_pub;

    //Publish Command
    [SerializeField] Button automateReadyButton;
    [SerializeField] Button signalButton;
    private bool isAutomateReady = true;
    private bool isSignalOn = true;
    private IEnumerator commandRoutine;
    private IPublisher<Co> command_pub; 

    private Co pendingCommand = null;

    void Start()
    {
        TryGetComponent(out ros2Unity);
        joy_routine = JoyAsync();
        statusRoutine = publishStatus();
        commandRoutine = publishCommandReady();
        automateReadyButton.onClick.AddListener(() => automateReadyButtonClicked());
        signalButton.onClick.AddListener(() => signalButtonClicked());
    }

    void Update()
    {
        leftdsjoy = controllerActions.GetComponent<ControllerActions>().Getleftjoy();
        rightdsjoy = controllerActions.GetComponent<ControllerActions>().Getrightjoy();
        is_auto = uiope.GetComponent<PanelContoroller>().getIsAuto();
        if(is_auto) ResetJoystickInput();
        if(ros2Unity.Ok()){
            if(ros2Node == null){
                ros2Node = ros2Unity.CreateNode("unity_publisher");
                //pub = ros2Node.CreatePublisher<Ts>(twistringTopicName);
                joy_pub = ros2Node.CreatePublisher<TS>(twiststampedTopicName);
                status_pub = ros2Node.CreatePublisher<Int32>("/behavior/set_status_num");
                command_pub = ros2Node.CreatePublisher<Co>("/command");
                //StartCoroutine(routine);
                StartCoroutine(joy_routine);
                StartCoroutine(statusRoutine);
                StartCoroutine(commandRoutine);
            }
        }
    }

    /*IEnumerator PublishTwistring()
    {
        while(true)
        {
            Ts msg = new Ts();
            if(queue.Count!=0)msg.Cmd = queue.Dequeue();
            while(twistmsgs.Count!=0)msg.Twist = twistmsgs.Dequeue();
            msg.Id = Convert.ToSByte(target_num);
            if(msg.Cmd!="" && is_auto || !is_auto){
                pub.Publish(msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }*/

    IEnumerator JoyAsync()
    {
        while (true)
        {
            if (!is_auto)
            {
                ROS2Clock clock = new ROS2Clock();
                TS sendtwist = new TS
                {
                    Twist = new geometry_msgs.msg.Twist(),
                    Header = new std_msgs.msg.Header()
                };
                sendtwist.Twist.Linear.X = XYJoy.Vertical * 2.0f;
                sendtwist.Twist.Linear.Y = -XYJoy.Horizontal * 2.0f;
                sendtwist.Twist.Angular.Z = -ZJoy.Horizontal * Mathf.PI;
                clock.UpdateROSClockTime(sendtwist.Header.Stamp);
                sendtwist.Header.Frame_id = "base_link";
                joy_pub.Publish(sendtwist);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    IEnumerator publishStatus()
    {
        while (true)
        {
            if (intQueue.Count != 0)
            {
                Int32 status_msg = new Int32();
                status_msg.Data = intQueue.Dequeue();
                if (is_auto)
                {
                    status_pub.Publish(status_msg);
                }
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    private void automateReadyButtonClicked()
    {
        isAutomateReady = !isAutomateReady;
        SetPendingCommand();
    }

    private void signalButtonClicked()
    {
        isSignalOn = !isSignalOn;
        SetPendingCommand();
    }
    private void SetPendingCommand()
    {
        ROS2Clock clock = new ROS2Clock();
        pendingCommand = new Co
        {
            Header = new std_msgs.msg.Header()
        };
        clock.UpdateROSClockTime(pendingCommand.Header.Stamp);
        pendingCommand.Header.Frame_id = "base_link";
        pendingCommand.Automate_ready = isAutomateReady;
        pendingCommand.Signal = isSignalOn;
    }

    IEnumerator publishCommandReady()
    {
        while (true)
        {
            if (pendingCommand != null)
            {
                command_pub.Publish(pendingCommand);
                pendingCommand = null;
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    public void ResetJoystickInput()
    {
        XYJoy.transform.Find("Handle").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        ZJoy.transform.Find("Handle").GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        XYJoy.Setpos(Vector2.zero);
        ZJoy.Setpos(Vector2.zero);
    }
}
