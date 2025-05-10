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
using TMPro;
public class UnityPublisher : MonoBehaviour
{
    [SerializeField] private GameObject controllerActions;
    [SerializeField] private GameObject uiope;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    //private IPublisher<Ts> pub;
    private int target_num = 0;
    [System.NonSerialized] public Queue<string> queue = new Queue<string>();
    [System.NonSerialized] public Queue<twist> twistmsgs = new Queue<twist>();
    //private IEnumerator routine;
    //[SerializeField] private string twistringTopicName = "/Twistring_topic_name";
    [SerializeField] private string twiststampedTopicName = "/TwistStamped_topic_name";
    [SerializeField] private float pub_hz = 0.05f;
    private bool is_main;

    public FixedJoystick XYJoy;
    public FixedJoystick ZJoy;
    private Vector2 leftdsjoy;
    private Vector2 rightdsjoy;
    private IPublisher<TS> joy_pub;
    private IEnumerator joy_routine;

    void Start()
    {
        TryGetComponent(out ros2Unity);
        //routine = PublishTwistring();
        joy_routine = JoyAsync();
    }

    void Update()
    {
        leftdsjoy = controllerActions.GetComponent<ControllerActions>().Getleftjoy();
        rightdsjoy = controllerActions.GetComponent<ControllerActions>().Getrightjoy();
        is_main = uiope.GetComponent<PanelContoroller>().Getismain();
        if(ros2Unity.Ok()){
            if(ros2Node == null){
                ros2Node = ros2Unity.CreateNode("UnityPubNode");
                //pub = ros2Node.CreatePublisher<Ts>(twistringTopicName);
                joy_pub = ros2Node.CreatePublisher<TS>(twiststampedTopicName);
                //StartCoroutine(routine);
                StartCoroutine(joy_routine);
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
            if(msg.Cmd!="" && is_main || !is_main){
                pub.Publish(msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }*/

    IEnumerator JoyAsync()
    {
        while (true)
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
            if(!is_main) joy_pub.Publish(sendtwist);
            yield return new WaitForSeconds(pub_hz);
        }
    }
}
