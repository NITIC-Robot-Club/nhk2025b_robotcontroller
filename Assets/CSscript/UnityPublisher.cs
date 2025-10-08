using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ROS2;
using topicSt = std_msgs.msg.String;
using twist = geometry_msgs.msg.Twist;
using TS = geometry_msgs.msg.TwistStamped;
using Int32 = std_msgs.msg.Int32;
using Bool = std_msgs.msg.Bool;
using Co = nhk2025b_msgs.msg.Command;
using Ba = nhk2025b_msgs.msg.BoxArm;
using Cn = nhk2025b_msgs.msg.Conveyor;
using Pl = nhk2025b_msgs.msg.PylonArm;
using EA = nhk2025b_msgs.msg.EArm;
using TMPro;
public class UnityPublisher : MonoBehaviour
{
    [SerializeField] private GameObject controllerActions;
    [SerializeField] private GameObject uiope;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    [System.NonSerialized] public Queue<string> queue = new Queue<string>();
    [System.NonSerialized] public Queue<int> intQueue = new Queue<int>();
    [System.NonSerialized] public Queue<twist> twistmsgs = new Queue<twist>();
    [System.NonSerialized] public Queue<Co> commandmsgs = new Queue<Co>();
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
    [SerializeField] Button pauseButton;
    [SerializeField] Button continueButton;
    [SerializeField] Button wingResetButton;
    [SerializeField] Button clawResetButton;
    private IEnumerator commandRoutine;
    private IEnumerator publishButtonCommandRoutine;
    private IPublisher<Co> command_pub; 
    private bool allowAutomate = false;
    private bool signal = false;

    //Publish BoxArm
    private IEnumerator boxArmRoutine;
    private IPublisher<Ba> boxArm_pub;
    [SerializeField] private Slider boxArmHeightSlider1;
    [SerializeField] private Slider boxArmHeightSlider2;
    [SerializeField] private Slider boxArmHandPositionSlider1;
    [SerializeField] private Slider boxArmHandPositionSlider2;
    [SerializeField] private Slider boxArmExpandSlider1;
    [SerializeField] private Slider boxArmExpandSlider2;

    //Publish Conveyor
    private IEnumerator conveyorRoutine;
    private IPublisher<Cn> conveyor_pub;
    [SerializeField] private Slider boxConveyorRpmSlider1;
    [SerializeField] private Slider boxConveyorRpmSlider2;

    //Publish PylonArm
    private IEnumerator pylonArmRoutine;
    private IPublisher<Pl> pylonArm_pub;
    [SerializeField] private Slider pylonArmHeightSlider1;
    [SerializeField] private Slider pylonArmHeightSlider2;
    [SerializeField] private Slider pylonArmCollectRpmSlider1;
    [SerializeField] private Slider pylonArmCollectRpmSlider2;
    [SerializeField] private Slider pylonArmExpandSlider1;
    [SerializeField] private Slider pylonArmExpandSlider2;

    //Publish Field Status
    [SerializeField] private Toggle fieldToggle;
    private IPublisher<Bool> field_pub;
    private IEnumerator fieldRoutine;
    private bool isRed = true;

    //Publish EArm
    private IEnumerator eArmRoutine;
    private IPublisher<EA> eArm_pub;
    [SerializeField] private Slider eArmGetSlider;
    [SerializeField] private Slider eArmExpandSlider;

    //
    [SerializeField] private Button slowToggle;
    private bool isSlow = false;
    [SerializeField] private float slowAccelLimit = 1.1f;
    private float lastVX = 0f;
    private float lastVY = 0f;
    private float lastTime = 0f;

    // Publish Chassis Arrow Value
    private HoldButtonAction holdButtonAction;
    private IEnumerator publishArrowButtonHold;
    private IEnumerator publishArrowButtonOnClick;
    private float verticalValue = 0.0f;
    private float horizontalValue = 0.0f;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    private bool isForwardPressed = false;
    private bool isBackwardPressed = false;
    private bool isLeftPressed = false;
    private bool isRightPressed = false;

    void Start()
    {
        holdButtonAction = uiope.GetComponent<HoldButtonAction>();
        TryGetComponent(out ros2Unity);

        joy_routine = JoyAsync();
        statusRoutine = publishStatus();
        commandRoutine = publishCommandReady();
        boxArmRoutine = publishBoxArm();
        conveyorRoutine = publishConveyor();
        pylonArmRoutine = publishPylonArm();
        eArmRoutine = publishEArm();

        automateReadyButton.onClick.AddListener(() => automateReadyButtonClicked());
        pauseButton.onClick.AddListener( () => pauseButtonClicked());
        continueButton.onClick.AddListener( () => continueButtonClicked());
        clawResetButton.onClick.AddListener( () => clawResetButtonClicked());
        wingResetButton.onClick.AddListener( () => wingResetButtonClicked());
        fieldRoutine = publishFieldStatus();

        // クリックは値をセットするだけ（送信は JoyAsync に統合）
        slowToggle.onClick.AddListener(() => isSlow = !isSlow);

        StartCoroutine(joy_routine);
        StartCoroutine(statusRoutine);
        StartCoroutine(commandRoutine);
        StartCoroutine(boxArmRoutine);
        StartCoroutine(conveyorRoutine);
        StartCoroutine(pylonArmRoutine);
        StartCoroutine(fieldRoutine);
        StartCoroutine(eArmRoutine);

        // --- ここでUIの押下イベント登録（長押し含む） ---
        AddEvent(forwardButton.gameObject, EventTriggerType.PointerDown, (_) => isForwardPressed = true);
        AddEvent(forwardButton.gameObject, EventTriggerType.PointerUp, (_) => isForwardPressed = false);
        AddEvent(backwardButton.gameObject, EventTriggerType.PointerDown, (_) => isBackwardPressed = true);
        AddEvent(backwardButton.gameObject, EventTriggerType.PointerUp, (_) => isBackwardPressed = false);
        AddEvent(leftButton.gameObject, EventTriggerType.PointerDown, (_) => isLeftPressed = true);
        AddEvent(leftButton.gameObject, EventTriggerType.PointerUp, (_) => isLeftPressed = false);
        AddEvent(rightButton.gameObject, EventTriggerType.PointerDown, (_) => isRightPressed = true);
        AddEvent(rightButton.gameObject, EventTriggerType.PointerUp, (_) => isRightPressed = false);

        forwardButton.onClick.AddListener(() => verticalValue = 1.0f);
        backwardButton.onClick.AddListener(() => verticalValue = -1.0f);
        leftButton.onClick.AddListener(() => horizontalValue = 1.0f);
        rightButton.onClick.AddListener(() => horizontalValue = -1.0f);
    }

    private void AddEvent(GameObject obj, EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        if (trigger == null) trigger = obj.AddComponent<EventTrigger>();
        var entry = new EventTrigger.Entry { eventID = type };
        entry.callback.AddListener(action);
        trigger.triggers.Add(entry);
    }

    void Update()
    {
        leftdsjoy = controllerActions.GetComponent<ControllerActions>().Getleftjoy();
        rightdsjoy = controllerActions.GetComponent<ControllerActions>().Getrightjoy();
        is_auto = uiope.GetComponent<PanelContoroller>().getIsAuto();
        if(is_auto) ResetJoystickInput();
        if(ros2Unity.Ok()){
            if(ros2Node == null){
                ros2Node = ros2Unity.CreateNode("robotcontroller_publisher");
                joy_pub = ros2Node.CreatePublisher<TS>("/controller/cmd_vel");
                status_pub = ros2Node.CreatePublisher<Int32>("/behavior/set_status_num");
                command_pub = ros2Node.CreatePublisher<Co>("/command");
                boxArm_pub = ros2Node.CreatePublisher<Ba>("/box_arm/controller_cmd");
                conveyor_pub = ros2Node.CreatePublisher<Cn>("/conveyor/controller_cmd");
                pylonArm_pub = ros2Node.CreatePublisher<Pl>("/pylon_arm/controller_cmd");
                eArm_pub = ros2Node.CreatePublisher<EA>("/e_arm/controller_cmd");
                field_pub = ros2Node.CreatePublisher<Bool>("/is_red");
            }
        }
        isRed = fieldToggle.GetisAwake();
    }

    IEnumerator JoyAsync()
    {
        while (true)
        {
            if (!is_auto && joy_pub != null)
            {
                float baseVX = (XYJoy != null) ? XYJoy.Vertical * (isSlow ? 1.0f : 2.0f) : 0f;
                float baseVY = (XYJoy != null) ? -XYJoy.Horizontal * (isSlow ? 1.0f : 2.0f) : 0f;
                float targetWZ = (ZJoy != null) ? (-ZJoy.Horizontal * Mathf.PI * (isSlow ? (1 / 1.5f) : 1f)) : 0f;

                bool chassis = uiope != null && uiope.GetComponent<PanelContoroller>().getIsChassis();

                float clickVX = chassis ? verticalValue * 2.0f : 0f;
                float clickVY = chassis ? horizontalValue * 2.0f : 0f;

                float holdVX = 0f, holdVY = 0f;
                if (chassis)
                {
                    if (isForwardPressed) holdVX += 2.0f;
                    if (isBackwardPressed) holdVX -= 2.0f;
                    if (isLeftPressed) holdVY += 2.0f;
                    if (isRightPressed) holdVY -= 2.0f;
                }

                float targetVX = baseVX + clickVX + holdVX;
                float targetVY = baseVY + clickVY + holdVY;

                float outVX = targetVX;
                float outVY = targetVY;
                float now = Time.realtimeSinceStartup;
                float dt = (lastTime > 0f) ? Mathf.Max(0f, now - lastTime) : pub_hz;

                if (isSlow)
                {
                    float maxDelta = slowAccelLimit * Mathf.Max(0.001f, dt);
                    outVX = Mathf.MoveTowards(lastVX, targetVX, maxDelta);
                    outVY = Mathf.MoveTowards(lastVY, targetVY, maxDelta);
                }

                ROS2Clock clock = new ROS2Clock();
                TS sendtwist = new TS
                {
                    Twist = new geometry_msgs.msg.Twist(),
                    Header = new std_msgs.msg.Header()
                };
                sendtwist.Twist.Linear.X = outVX;
                sendtwist.Twist.Linear.Y = outVY;
                sendtwist.Twist.Angular.Z = targetWZ;
                clock.UpdateROSClockTime(sendtwist.Header.Stamp);
                sendtwist.Header.Frame_id = "base_link";
                joy_pub.Publish(sendtwist);

                verticalValue = 0.0f;
                horizontalValue = 0.0f;
                lastVX = outVX;
                lastVY = outVY;
                lastTime = now;
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
        allowAutomate = !allowAutomate;
        commandmsgs.Enqueue(new Co { Allow_automate = allowAutomate, Signal = signal, Reset_claw = false, Reset_wing = false });
    }

    private void pauseButtonClicked()
    {
        signal = false;
        commandmsgs.Enqueue(new Co { Allow_automate = allowAutomate, Signal = signal, Reset_claw = false, Reset_wing = false });
    }

    private void continueButtonClicked()
    {
        signal = true;
        commandmsgs.Enqueue(new Co { Allow_automate = allowAutomate, Signal = signal, Reset_claw = false, Reset_wing = false });
    }

    private void clawResetButtonClicked()
    {
        for (int i = 0; i < 3; i++)
        {
            commandmsgs.Enqueue(new Co { Allow_automate = allowAutomate, Signal = signal, Reset_claw = true, Reset_wing = false });
        }
    }

    private void wingResetButtonClicked()
    {
        for (int i = 0; i < 3; i++)
        {
            commandmsgs.Enqueue(new Co { Allow_automate = allowAutomate, Signal = signal, Reset_claw = false, Reset_wing = true });
        }
    }

    IEnumerator publishCommandReady()
    {
        while (true)
        {
            if (command_pub != null)
            {
                if (commandmsgs.Count == 0)
                {
                    Co command = new Co
                    {
                        Allow_automate = allowAutomate,
                        Signal = signal,
                        Reset_claw = false,
                        Reset_wing = false
                    };
                    command_pub.Publish(command);
                }                
                else 
                {
                    Co command_msg = commandmsgs.Dequeue();
                    command_pub.Publish(command_msg);
                }
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

    IEnumerator publishBoxArm()
    {
        while (true)
        {
            if (!is_auto && boxArm_pub != null)
            {
                Ba boxArm_msg = new Ba();
                boxArm_msg.Height[0] = boxArmHeightSlider1.value / 1000.0f;
                boxArm_msg.Height[1] = boxArmHeightSlider2.value / 1000.0f;
                boxArm_msg.Hand_position[0] = boxArmHandPositionSlider1.value;
                boxArm_msg.Hand_position[1] = boxArmHandPositionSlider2.value;
                boxArm_msg.Expand[0] = boxArmExpandSlider1.value * Mathf.Deg2Rad;
                boxArm_msg.Expand[1] = boxArmExpandSlider2.value * Mathf.Deg2Rad;
                boxArm_pub.Publish(boxArm_msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    IEnumerator publishConveyor()
    {
        while (true)
        {
            if (!is_auto && conveyor_pub != null)
            {
                Cn conveyor_msg = new Cn();
                conveyor_msg.Conveyor_rpm[0] = boxConveyorRpmSlider1.value;
                conveyor_msg.Conveyor_rpm[1] = boxConveyorRpmSlider2.value;
                conveyor_pub.Publish(conveyor_msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    IEnumerator publishPylonArm()
    {
        while (true)
        {
            if (!is_auto && pylonArm_pub != null)
            {
                Pl pylonArm_msg = new Pl();
                pylonArm_msg.Height[0] = pylonArmHeightSlider1.value / 1000.0f;
                pylonArm_msg.Height[1] = pylonArmHeightSlider2.value / 1000.0f;
                pylonArm_msg.Collect_rpm[0] = pylonArmCollectRpmSlider1.value;
                pylonArm_msg.Collect_rpm[1] = pylonArmCollectRpmSlider2.value;
                pylonArm_msg.Expand[0] = pylonArmExpandSlider1.value * Mathf.Deg2Rad;
                pylonArm_msg.Expand[1] = pylonArmExpandSlider2.value * Mathf.Deg2Rad;
                pylonArm_pub.Publish(pylonArm_msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    IEnumerator publishFieldStatus()
    {
        while (true)
        {
            if (field_pub != null)
            {
                Bool field_msg = new Bool();
                field_msg.Data = isRed;
                field_pub.Publish(field_msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }

    IEnumerator publishEArm()
    {
        while (true)
        {
            if (!is_auto && eArm_pub != null)
            {
                EA eArm_msg = new EA();
                eArm_msg.Expand = eArmExpandSlider.value * Mathf.Deg2Rad;
                eArm_msg.Get = eArmGetSlider.value;
                eArm_pub.Publish(eArm_msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }
}