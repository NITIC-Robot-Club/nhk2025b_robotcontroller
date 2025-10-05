using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ROS2;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using topicSt = std_msgs.msg.String;
using Og = nav_msgs.msg.OccupancyGrid;
using Ps = geometry_msgs.msg.PoseStamped;
using Pa = nav_msgs.msg.Path;
using TS = geometry_msgs.msg.TwistStamped;
using Sw = nhk2025b_msgs.msg.Swerve;
using Sa = nhk2025b_msgs.msg.StateArray;
using Ba = nhk2025b_msgs.msg.BoxArm;
using Cn = nhk2025b_msgs.msg.Conveyor;
using Pl = nhk2025b_msgs.msg.PylonArm;
using Rs = nhk2025b_msgs.msg.RobotStatus;
using EA = nhk2025b_msgs.msg.EArm;
using IMA = std_msgs.msg.Int32MultiArray;

public class UnitySubscriber : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private UnityPublisher unityPublisher;
    private ISubscription<Og> map_sub;
    private ISubscription<Ps> currentpose_sub;
    private ISubscription<Ps> goalpose_sub;
    private ISubscription<Pa> path_sub;
    private ISubscription<Ps> lookaheadpose_sub;
    private ISubscription<Sw> result_sub;
    private ISubscription<Sw> cmd_sub;
    private ISubscription<Sa> state_sub;
    private ISubscription<nhk2025b_msgs.msg.State> now_state_sub;
    private ISubscription<Ba> boxarm_sub;
    private ISubscription<Cn> conveyor_sub;
    private ISubscription<Pl> pylonarm_sub;
    private ISubscription<Rs> robotstatus_sub;
    private ISubscription<EA> earm_sub;
    private ISubscription<IMA> missing_can_id_pub;

    // /.../cmd topic
    private ISubscription<Ba> boxarm_cmd_sub;
    private ISubscription<Cn> conveyor_cmd_sub;
    private ISubscription<Pl> pylonarm_cmd_sub;
    private ISubscription<EA> earm_cmd_sub;
    private float[] boxArmHeightCmd = new float[2];
    private float[] boxArmExpandCmd = new float[2];
    private float[] boxArmHandCmd = new float[2];
    private float[] conveyorCmd = new float[2];
    private float[] pylonArmHeightCmd = new float[2];
    private float[] pylonArmExpandCmd = new float[2];
    private float[] pylonArmCollectRpmCmd = new float[2];
    private float earmGetCmd;
    private float earmExpandCmd;
    private Queue<string> recqueue = new Queue<string>();

    //Visualize OccupancyGrid
    public RawImage rawImage;
    private int ogWidth = 1920;
    private int ogHeight = 960;
    private sbyte[] ogData;
    private Texture2D ogTexture;
    private bool ogDirty = false;
    private const int ogWidthDefault = 1920;        // px
    private const int ogHeightDefault = 960;        // px
    private bool ogIsRed = false;
    private bool isRed = false;
    private sbyte[] prevOgData = null;
    private int prevOgWidth = 0;
    private int prevOgHeight = 0;

    //Pose Variables
    const float m2pixX = 1920.00f / 10.0f;          // px/m
    const float m2pixY = 960.00f / 5.0f;            // px/m

    //Current Pose Subscriber
    public GameObject robot;
    private RectTransform robotRectTransform;
    private float posX;
    private float posY;
    private float oriZ;
    private float oriW;

    //Goal Pose Subscriber
    public GameObject goal;
    private RectTransform goalRectTransform;
    private float gposX;
    private float gposY;
    private float goriZ;
    private float goriW;

    //Lookahead Subscriber
    public GameObject lookahead;
    private RectTransform lookaheadRectTransform;
    private float lposX;
    private float lposY;
    private float loriZ;
    private float loriW;

    //Result Subscriber
    const float maxSpeed = 1000.0f;
    public Color minColor = Color.blue;
    public Color maxColor = Color.green;
    private Renderer rend;
    [SerializeField] private TMP_Text swerveText0;
    [SerializeField] private TMP_Text swerveText1;
    [SerializeField] private TMP_Text swerveText2;
    [SerializeField] private TMP_Text swerveText3;
    public GameObject[] swerve = new GameObject[4];
    private RectTransform[] swerveRectTransform = new RectTransform[4];
    private float[] wheelAngle = new float[4];
    private float[] previousWheelAngle = new float[4];
    private float[] wheelSpeed = new float[4];
    private float soriZ;
    private float soriW;

    //Visualize Path
    const int maxPointCount = 50;
    private Pa subscribedPath;
    private Pa prevPath;
    private GameObject[] points = new GameObject[50];
    private RectTransform[] pointRectTransforms = new RectTransform[50];
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject pathParent;

    //Visualize Robot State
    [SerializeField] private TMP_Text currentStateText;
    private string currentState = "Current State";
    [SerializeField] GameObject buttonPrefabYellow;
    [SerializeField] GameObject buttonPrefabBlue;
    [SerializeField] GameObject buttonParent;
    private int stateSize;
    private int[] stateID = new int[50];
    private string[] stateName = new string[50];
    private int[] prevStateID = new int[50];
    private string[] prevStateName = new string[50];
    private int prevStateSize = 0;
    private bool stateChanged = false;
    private Vector2 initialPosition = new Vector2(0, 1350);
    private string nowStateName = "";
    private int nowStateId = 0;

    //Visualize BoxArm State
    [System.NonSerialized] public float[] boxArmExpand = new float[2];
    [System.NonSerialized] public float[] boxArmHeight = new float[2];
    [System.NonSerialized] public float[] boxArmHandPosition = new float[2];

    //Visualize Conveyor State
    private float[] conveyorRPM = new float[2]; 
    [SerializeField] private TMP_Text conveyorRPMText;

    //Visualize PylonArm State
    [System.NonSerialized] public float[] pylonArmExpand = new float[2];
    [System.NonSerialized] public float[] pylonArmHeight = new float[2];
    [System.NonSerialized] public float[] pylonArmCollectRPM = new float[2];

    // private ISubscription<Rs> robotstatus_sub;
    private const float max_voltage = 12.6f;
    private const float min_voltage = 11.1f;
    [System.NonSerialized] public float[] voltage = new float[3];
    [SerializeField] private CircleGraphManager voltageCircleGraph1;
    [SerializeField] private CircleGraphManager voltageCircleGraph2;
    [SerializeField] private CircleGraphManager voltageCircleGraph3;
    [SerializeField] private UnityEngine.UI.Toggle resetPylonArmHeightToggle1;
    [SerializeField] private UnityEngine.UI.Toggle resetPylonArmHeightToggle2;
    [SerializeField] private UnityEngine.UI.Toggle resetPylonArmExpandToggle1;
    [SerializeField] private UnityEngine.UI.Toggle resetPylonArmExpandToggle2;
    [SerializeField] private UnityEngine.UI.Toggle resetBoxArmHeightToggle1;
    [SerializeField] private UnityEngine.UI.Toggle resetBoxArmHeightToggle2;
    [SerializeField] private UnityEngine.UI.Toggle resetBoxArmHandToggle1;
    [SerializeField] private UnityEngine.UI.Toggle resetBoxArmHandToggle2;
    [SerializeField] private UnityEngine.UI.Toggle resetEArmExpandToggle;
    [SerializeField] private UnityEngine.UI.Toggle resetEArmGetToggle;
    private bool[] resetPylonArmHeight = new bool[2];
    private bool[] resetPylonArmExpand = new bool[2];
    private bool[] resetBoxArmHeight = new bool[2];
    private bool[] resetBoxArmHand = new bool[2];
    private bool resetEArmExpand = new bool();
    private bool resetEArmGet = new bool();

    // E-Arm
    public float eArmGet;
    public float eArmExpand;
    [SerializeField] private TMP_Text eArmText;

    // Missing CAN ID
    private int[] missingCanId = new int[0];
    [SerializeField] private TMP_Text missingCanIdText;

    // Auto->Manual Panel Transition
    [SerializeField] public Slider boxArmHeightSlider1;
    [SerializeField] public Slider boxArmHeightSlider2;
    [SerializeField] public Slider boxArmHandSlider1;
    [SerializeField] public Slider boxArmHandSlider2;
    [SerializeField] public Slider boxArmExpandSlider1;
    [SerializeField] public Slider boxArmExpandSlider2;
    [SerializeField] public Slider boxConveyorRpmSlider1;
    [SerializeField] public Slider boxConveyorRpmSlider2;
    [SerializeField] public Slider pylonArmHeightSlider1;
    [SerializeField] public Slider pylonArmHeightSlider2;
    [SerializeField] public Slider pylonArmCollectRpmSlider1;
    [SerializeField] public Slider pylonArmCollectRpmSlider2;
    [SerializeField] public Slider pylonArmExpandSlider1;
    [SerializeField] public Slider pylonArmExpandSlider2;
    [SerializeField] public Slider eArmGetSlider;
    [SerializeField] public Slider eArmExpandSlider;

    void Start()
    {
        unityPublisher = GameObject.Find("Pubcontoroller").GetComponent<UnityPublisher>();
        TryGetComponent(out ros2Unity);

        //Initialize Robot/Goal/Lookahead Transforms
        robotRectTransform = (RectTransform)robot.transform;
        robotRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        robotRectTransform.anchorMin = new Vector2(1, 1);
        robotRectTransform.anchorMax = new Vector2(1, 1);
        goalRectTransform = (RectTransform)goal.transform;
        goalRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        goalRectTransform.anchorMin = new Vector2(1, 1);
        goalRectTransform.anchorMax = new Vector2(1, 1);
        lookaheadRectTransform = (RectTransform)lookahead.transform;
        lookaheadRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        lookaheadRectTransform.anchorMin = new Vector2(1, 1);
        lookaheadRectTransform.anchorMax = new Vector2(1, 1);

        //Initialize Swerve Transforms
        for (int i = 0; i < 4; i++)
        {
            swerveRectTransform[i] = (RectTransform)swerve[i].transform;
        }

        //Initialize Path Transforms
        for (int i = 0; i < maxPointCount; i++)
        {
            points[i] = Instantiate(pointPrefab);
            points[i].transform.SetParent(pathParent.transform);
            points[i].transform.localScale = Vector3.one;
            pointRectTransforms[i] = (RectTransform)points[i].transform;
            pointRectTransforms[i].transform.rotation = Quaternion.Euler(0f, 0f, 90f);
            pointRectTransforms[i].anchorMin = new Vector2(1, 1);
            pointRectTransforms[i].anchorMax = new Vector2(1, 1);
        }
        if (swerveText0 != null) swerveText0.SetText($"WheelAngle0: {wheelAngle[0]}°\nWheelSpeed0: {wheelSpeed[0]}rpm");
        if (swerveText1 != null) swerveText1.SetText($"WheelAngle1: {wheelAngle[1]}°\nWheelSpeed1: {wheelSpeed[1]}rpm");
        if (swerveText2 != null) swerveText2.SetText($"WheelAngle2: {wheelAngle[2]}°\nWheelSpeed2: {wheelSpeed[2]}rpm");
        if (swerveText3 != null) swerveText3.SetText($"WheelAngle3: {wheelAngle[3]}°\nWheelSpeed3: {wheelSpeed[3]}rpm");
        float expand = Mathf.Rad2Deg * eArmExpand;
        eArmText.SetText($"E Arm\n  Get: {eArmGet.ToString("F2")}mm\n  Expand: {expand.ToString("F2")}°");
        conveyorRPMText.SetText($"RPM1: {conveyorRPM[0].ToString("F2")}rpm\nRPM2: {conveyorRPM[1].ToString("F2")}rpm");
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("robotcontroller_subscriber");
                map_sub = ros2Node.CreateSubscription<Og>("/behavior/map", mappingCallback);
                currentpose_sub = ros2Node.CreateSubscription<Ps>("/localization/current_pose", currentposeCallback);
                goalpose_sub = ros2Node.CreateSubscription<Ps>("/behavior/goal_pose", goalposeCallback);
                path_sub = ros2Node.CreateSubscription<Pa>("/planning/path", pathCallback);
                lookaheadpose_sub = ros2Node.CreateSubscription<Ps>("/control/lookahead", lookaheadposeCallback);
                result_sub = ros2Node.CreateSubscription<Sw>("/swerve/result", resultCallback);
                cmd_sub = ros2Node.CreateSubscription<Sw>("/visualization/swerve", cmdCallback);
                state_sub = ros2Node.CreateSubscription<Sa>("/behavior/avaiable_state_array", stateCallback);
                now_state_sub = ros2Node.CreateSubscription<nhk2025b_msgs.msg.State>("/behavior/state_now", nowStateCallback);
                boxarm_sub = ros2Node.CreateSubscription<Ba>("/box_arm/result", boxarmCallback);
                conveyor_sub = ros2Node.CreateSubscription<Cn>("/conveyor/result", conveyorCallback);
                pylonarm_sub = ros2Node.CreateSubscription<Pl>("/pylon_arm/result", pylonarmCallback);
                robotstatus_sub = ros2Node.CreateSubscription<Rs>("/robot_status", robotstatusCallback);
                earm_sub = ros2Node.CreateSubscription<EA>("/e_arm/result", earmCallback);
                missing_can_id_pub = ros2Node.CreateSubscription<IMA>("/missing_can_id", missingCanIdCallback);
                conveyor_cmd_sub = ros2Node.CreateSubscription<Cn>("/conveyor/cmd", conveyorCmdCallback);
                pylonarm_cmd_sub = ros2Node.CreateSubscription<Pl>("/pylon_arm/cmd", pylonarmCmdCallback);
                earm_cmd_sub = ros2Node.CreateSubscription<EA>("/e_arm/cmd", earmCmdCallback);
                boxarm_cmd_sub = ros2Node.CreateSubscription<Ba>("/box_arm/cmd", boxarmCmdCallback);
            }
        }

        isRed = ogIsRed;

        //Visualize Robot/Goal/Lookahead Positions
        if (!isRed)
        {
            lookaheadRectTransform.anchorMin = new Vector2(1, 1);
            lookaheadRectTransform.anchorMax = new Vector2(1, 1);
            goalRectTransform.anchorMin = new Vector2(1, 1);
            goalRectTransform.anchorMax = new Vector2(1, 1);
            robotRectTransform.anchorMin = new Vector2(1, 1);
            robotRectTransform.anchorMax = new Vector2(1, 1);
        }
        else
        {
            lookaheadRectTransform.anchorMin = new Vector2(1, 0);
            lookaheadRectTransform.anchorMax = new Vector2(1, 0);
            goalRectTransform.anchorMin = new Vector2(1, 0);
            goalRectTransform.anchorMax = new Vector2(1, 0);
            robotRectTransform.anchorMin = new Vector2(1, 0);
            robotRectTransform.anchorMax = new Vector2(1, 0);
        }

        lookaheadRectTransform.anchoredPosition = new Vector3(lposX, lposY, 0f);
        lookahead.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, loriZ, loriW);
        goalRectTransform.anchoredPosition = new Vector3(gposX, gposY, 0f);
        goal.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, goriZ, goriW);
        robotRectTransform.anchoredPosition = new Vector3(posX, posY, 0f);
        robot.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, oriZ, oriW);

        //Visualize Path
        if(subscribedPath != null && subscribedPath.Poses != null && subscribedPath.Poses.Length > 0)
        {
            // パスが変更された場合のみ更新
            bool pathChanged = false;
            
            if (prevPath == null || 
                prevPath.Poses == null || 
                prevPath.Poses.Length != subscribedPath.Poses.Length)
            {
                pathChanged = true;
            }
            else
            {
                // パスの内容を比較（最初と最後のポイントのみチェックで高速化）
                if (prevPath.Poses.Length > 0 && subscribedPath.Poses.Length > 0)
                {
                    var firstOld = prevPath.Poses[0].Pose.Position;
                    var firstNew = subscribedPath.Poses[0].Pose.Position;
                    var lastOld = prevPath.Poses[prevPath.Poses.Length - 1].Pose.Position;
                    var lastNew = subscribedPath.Poses[subscribedPath.Poses.Length - 1].Pose.Position;
                    
                    if (Math.Abs(firstOld.X - firstNew.X) > 0.01 || 
                        Math.Abs(firstOld.Y - firstNew.Y) > 0.01 ||
                        Math.Abs(lastOld.X - lastNew.X) > 0.01 || 
                        Math.Abs(lastOld.Y - lastNew.Y) > 0.01)
                    {
                        pathChanged = true;
                    }
                }
            }

            if (pathChanged)
            {
                for(int i = 0; i < maxPointCount; i++)
                {
                    if(points[i] != null)
                    {
                        Destroy(points[i]);
                        points[i] = null;
                        pointRectTransforms[i] = null;
                    }
                }
                int setmax = Math.Min(subscribedPath.Poses.Length, maxPointCount);
                
                for(int i = 0; i < setmax; i++)
                {
                    int num = i * subscribedPath.Poses.Length / setmax;
                    if(num >= subscribedPath.Poses.Length)
                        num = subscribedPath.Poses.Length - 1;

                    points[i] = Instantiate(pointPrefab);
                    points[i].transform.SetParent(pathParent.transform);
                    points[i].transform.localScale = Vector3.one;
                    pointRectTransforms[i] = (RectTransform)points[i].transform;

                    float px = -(float)subscribedPath.Poses[num].Pose.Position.X * m2pixY;
                    float py = -(float)subscribedPath.Poses[num].Pose.Position.Y * m2pixX;

                    if (isRed)
                    {
                        pointRectTransforms[i].anchorMin = new Vector2(1, 0);
                        pointRectTransforms[i].anchorMax = new Vector2(1, 0);
                    }
                    else
                    {
                        pointRectTransforms[i].anchorMin = new Vector2(1, 1);
                        pointRectTransforms[i].anchorMax = new Vector2(1, 1);
                    }

                    pointRectTransforms[i].transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, (float)subscribedPath.Poses[num].Pose.Orientation.Z, (float)subscribedPath.Poses[num].Pose.Orientation.W);
                    pointRectTransforms[i].anchoredPosition = new Vector3(px, py, 0);
                }
                prevPath = new Pa();
                prevPath.Poses = new Ps[subscribedPath.Poses.Length];
                for (int i = 0; i < subscribedPath.Poses.Length; i++)
                {
                    prevPath.Poses[i] = new Ps();
                    prevPath.Poses[i].Pose = new geometry_msgs.msg.Pose();
                    prevPath.Poses[i].Pose.Position = new geometry_msgs.msg.Point();
                    prevPath.Poses[i].Pose.Position.X = subscribedPath.Poses[i].Pose.Position.X;
                    prevPath.Poses[i].Pose.Position.Y = subscribedPath.Poses[i].Pose.Position.Y;
                    prevPath.Poses[i].Pose.Position.Z = subscribedPath.Poses[i].Pose.Position.Z;
                }
            }
            else
            {
                for(int i = 0; i < maxPointCount; i++)
                {
                    if(points[i] != null && pointRectTransforms[i] != null)
                    {
                        if (isRed)
                        {
                            pointRectTransforms[i].anchorMin = new Vector2(1, 0);
                            pointRectTransforms[i].anchorMax = new Vector2(1, 0);
                        }
                        else
                        {
                            pointRectTransforms[i].anchorMin = new Vector2(1, 1);
                            pointRectTransforms[i].anchorMax = new Vector2(1, 1);
                        }
                    }
                }
            }
        }
        else
        {
            if (prevPath != null || points[0] != null)
            {
                for(int i = 0; i < maxPointCount; i++)
                {
                    if(points[i] != null)
                    {
                        Destroy(points[i]);
                        points[i] = null;
                        pointRectTransforms[i] = null;
                    }
                }
                prevPath = null;
            }
        }

        //Visualize Robot State
        currentStateText.text = currentState;
    if (stateChanged && stateSize > 0)
    {
        foreach (Transform child in buttonParent.transform)
        {
            Destroy(child.gameObject);
        }
        for (int i = 0; i < stateSize; i++)
        {
            GameObject button;
            if (nowStateId == stateID[i] && nowStateName == stateName[i])
            {
                button = Instantiate(buttonPrefabBlue);
            }
            else
            {
                button = Instantiate(buttonPrefabYellow);
            }
            button.transform.SetParent(buttonParent.transform, false);
            button.transform.localScale = Vector3.one;
            button.GetComponent<RectTransform>().anchoredPosition = initialPosition + new Vector2(0, -i * 125);
            button.GetComponentInChildren<TMP_Text>().text = stateID[i] + ": " + stateName[i];
            int idx = i;
            button.GetComponent<Button>().onClick.AddListener(() => {
                Debug.Log(stateID[idx] + ": " + stateName[idx]);
                sendStatus(stateID[idx]);
            });
        }
        
        prevStateSize = stateSize;
        Array.Copy(stateID, prevStateID, stateSize);
        Array.Copy(stateName, prevStateName, stateSize);
        
        stateChanged = false;
    }

        if (stateSize != prevStateSize || !stateID.SequenceEqual(prevStateID) || !stateName.SequenceEqual(prevStateName))
        {
            stateChanged = true;
        }
        else
        {
            stateChanged = false;
        }

        if (stateChanged)
        {
            prevStateSize = stateSize;
            Array.Copy(stateID, prevStateID, stateSize);
            Array.Copy(stateName, prevStateName, stateSize);
        }

        //Visualize Missing CAN ID (16進数表示)
        if (missingCanIdText != null && missingCanId != null && missingCanId.Length > 0)
        {
            string hexStr = string.Join(", ", missingCanId.Select(id => $"0x{id:X}"));
            missingCanIdText.SetText("Missing CAN ID: " + hexStr);
        }
        else if (missingCanIdText != null)
        {
            missingCanIdText.SetText("Missing CAN ID: None");
        }
    }

    void mappingCallback(Og msg)
    {
        Debug.Log("Received OccupancyGrid");
        int width = (int)msg.Info.Width;
        int height = (int)msg.Info.Height;
        sbyte[] data = msg.Data;
        if (prevOgData != null && 
            prevOgWidth == width && 
            prevOgHeight == height && 
            prevOgData.Length == data.Length)
        {
            bool isSame = true;
            for (int i = 0; i < data.Length; i++)
            {
                if (prevOgData[i] != data[i])
                {
                    isSame = false;
                    break;
                }
            }
            if (isSame)
            {
                return;
            }
        }
        prevOgData = (sbyte[])data.Clone();
        prevOgWidth = width;
        prevOgHeight = height;
        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            Texture2D texture = new Texture2D(width, height, TextureFormat.RGBA32, false);
            texture.filterMode = FilterMode.Point;
            texture.wrapMode = TextureWrapMode.Clamp;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = x + y * width;
                    sbyte val = data[index];

                    Color color;
                    if (val == -1)
                        color = Color.gray;
                    else if (val == 0)
                        color = Color.white;
                    else
                        color = Color.black;
                    texture.SetPixel(width - x - 1, height - y - 1, color);
                }
            }
            texture.Apply();
            rawImage.texture = texture;
            rawImage.rectTransform.sizeDelta = new Vector2(ogWidthDefault, ogHeightDefault);
        });
    }

    void currentposeCallback(Ps msg)
    {
        Debug.Log("Received CurrentPose");
        posX = -(float)msg.Pose.Position.X * m2pixY;
        posY = -(float)msg.Pose.Position.Y * m2pixX;
        oriZ = -(float)msg.Pose.Orientation.Z;
        oriW = -(float)msg.Pose.Orientation.W;
    }

    void goalposeCallback(Ps msg)
    {
        Debug.Log("Received GoalPose");
        gposX = -(float)msg.Pose.Position.X * m2pixY;
        gposY = -(float)msg.Pose.Position.Y * m2pixX;
        goriZ = -(float)msg.Pose.Orientation.Z;
        goriW = -(float)msg.Pose.Orientation.W;
    }

    void lookaheadposeCallback(Ps msg)
    {
        lposX = -(float)msg.Pose.Position.X * m2pixY;
        lposY = -(float)msg.Pose.Position.Y * m2pixX;
        loriZ = -(float)msg.Pose.Orientation.Z;
        loriW = -(float)msg.Pose.Orientation.W;
    }

    void pathCallback(Pa msg)
    {
        Debug.Log("Received Path");
        Pa localPath = msg;
        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            subscribedPath = localPath;
        });
    }

    void resultCallback(Sw msg)
    {
        Debug.Log("Received Swerve Result");
        float[] localWheelSpeed = new float[4];
        float[] localWheelAngle = new float[4];
        
        for (int i = 0; i < 4; i++)
        {
            if (-1.0f <= msg.Wheel_speed[i] && msg.Wheel_speed[i] <= 1.0f)
            {
                localWheelSpeed[i] = 0f;
                localWheelAngle[i] = (float)msg.Wheel_angle[i] * Mathf.Rad2Deg;
            }
            else if (msg.Wheel_speed[i] < -1.0f)
            {
                localWheelSpeed[i] = Mathf.Abs(msg.Wheel_speed[i]);
                localWheelAngle[i] = ((float)msg.Wheel_angle[i] + Mathf.PI) * Mathf.Rad2Deg;
            }
            else 
            {
                localWheelSpeed[i] = (float)msg.Wheel_speed[i];
                localWheelAngle[i] = (float)msg.Wheel_angle[i] * Mathf.Rad2Deg;
            }
        }

        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            wheelSpeed = localWheelSpeed;
            wheelAngle = localWheelAngle;

            //Visualize Swerve Pose
            if (swerveText0 != null) swerveText0.SetText($"WheelAngle0: {wheelAngle[0]}°\nWheelSpeed0: {wheelSpeed[0]}rpm");
            if (swerveText1 != null) swerveText1.SetText($"WheelAngle1: {wheelAngle[1]}°\nWheelSpeed1: {wheelSpeed[1]}rpm");
            if (swerveText2 != null) swerveText2.SetText($"WheelAngle2: {wheelAngle[2]}°\nWheelSpeed2: {wheelSpeed[2]}rpm");
            if (swerveText3 != null) swerveText3.SetText($"WheelAngle3: {wheelAngle[3]}°\nWheelSpeed3: {wheelSpeed[3]}rpm");

            for (int i = 0; i < swerve.Length; i++)
            {
                float t = Mathf.Clamp01(Mathf.Abs(wheelSpeed[i]) / maxSpeed);
                Color swerveColor = Color.Lerp(minColor, maxColor, t);
                var image = swerve[i]?.GetComponent<UnityEngine.UI.Image>();
                if (image != null) image.color = swerveColor;
                float soriZ = Mathf.Sin(wheelAngle[i] / Mathf.Rad2Deg / 2.0f);
                float soriW = Mathf.Cos(wheelAngle[i] / Mathf.Rad2Deg / 2.0f);
                swerveRectTransform[i].transform.rotation = new Quaternion(0f, 0f, soriZ, soriW);
            }
        });
    }

    void cmdCallback(Sw msg)
    {
        Debug.Log("Received Swerve Cmd");
    }

    void earmCallback(EA msg)
    {
        Debug.Log("Received EArm Result");
        float localGet = msg.Get;
        float localExpand = msg.Expand;

        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            eArmGet = localGet;
            eArmExpand = localExpand;
            float expand = Mathf.Rad2Deg * eArmExpand;
            eArmText.SetText($"E Arm\n  Get: {eArmGet.ToString("F2")}mm\n  Expand: {expand.ToString("F2")}°");
        });
    }

    void stateCallback(Sa msg)
    {
        Debug.Log("Received State Array");
        int newStateSize = msg.State.Length;
        string newCurrentState = msg.Name;
        int[] newStateID = new int[50];
        string[] newStateName = new string[50];
        
        for (int i = 0; i < newStateSize; i++)
        {
            newStateID[i] = msg.State[i].Id;
            newStateName[i] = msg.State[i].Name;
        }

        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            bool hasChanged = false;
            if (newStateSize != prevStateSize)
            {
                hasChanged = true;
            }
            else
            {
                for (int i = 0; i < newStateSize; i++)
                {
                    if (newStateID[i] != prevStateID[i] || newStateName[i] != prevStateName[i])
                    {
                        hasChanged = true;
                        break;
                    }
                }
            }
            if (newCurrentState != currentState)
            {
                hasChanged = true;
            }
            if (hasChanged)
            {
                stateSize = newStateSize;
                currentState = newCurrentState;
                for (int i = 0; i < stateSize; i++)
                {
                    stateID[i] = newStateID[i];
                    stateName[i] = newStateName[i];
                }
                stateChanged = true;
            }
        });
    }

    void nowStateCallback(nhk2025b_msgs.msg.State msg)
    {
        string newName = msg.Name;
        int newId = msg.Id;
        
        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (nowStateName != newName || nowStateId != newId)
            {
                nowStateName = newName;
                nowStateId = newId;
                stateChanged = true;
            }
        });
    }

    void sendStatus(int status)
    {
        unityPublisher.intQueue.Enqueue(status);
        Debug.Log("SendStatus: " + status.ToString());
    }

    void boxarmCallback(Ba msg)
    {
        boxArmExpand[0] = msg.Expand[0];
        boxArmExpand[1] = msg.Expand[1];
        boxArmHeight[0] = msg.Height[0];
        boxArmHeight[1] = msg.Height[1];
        boxArmHandPosition[0] = msg.Hand_position[0];
        boxArmHandPosition[1] = msg.Hand_position[1];
    }

    void conveyorCallback(Cn msg)
    {
        float rpm0 = msg.Conveyor_rpm[0];
        float rpm1 = msg.Conveyor_rpm[1];
        
        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            conveyorRPM[0] = rpm0;
            conveyorRPM[1] = rpm1;
            if (conveyorRPMText != null)
            {
                conveyorRPMText.SetText($"RPM1: {conveyorRPM[0].ToString("F2")}rpm\nRPM2: {conveyorRPM[1].ToString("F2")}rpm");
            }
        });
    }

    void pylonarmCallback(Pl msg)
    {
        Debug.Log("Received PylonArm Result");
        pylonArmExpand[0] = msg.Expand[0];
        pylonArmExpand[1] = msg.Expand[1];
        pylonArmHeight[0] = msg.Height[0];
        pylonArmHeight[1] = msg.Height[1];
        pylonArmCollectRPM[0] = msg.Collect_rpm[0];
        pylonArmCollectRPM[1] = msg.Collect_rpm[1];
    }

    void OnOccupancyGridReceived(Og msg)
    {
        var newOgData = msg.Data;

        bool isSame = prevOgData != null && prevOgData.Length == newOgData.Length;
        if (isSame)
        {
            for (int i = 0; i < newOgData.Length; i++)
            {
                if (prevOgData[i] != newOgData[i])
                {
                    isSame = false;
                    break;
                }
            }
        }

        if (isSame)
        {
            return;
        }

        prevOgData = (sbyte[])newOgData.Clone();
        ogData = prevOgData;
        ogDirty = true;
    }

    void robotstatusCallback(Rs msg)
    {
        Debug.Log("Received RobotStatus");
        float[] localVoltage = new float[3];
        bool[] localResetPylonHeight = new bool[2];
        bool[] localResetPylonExpand = new bool[2];
        bool[] localResetBoxHeight = new bool[2];
        bool[] localResetBoxHand = new bool[2];
        bool localResetEArmExpand;
        bool localResetEArmGet;

        localVoltage[0] = msg.Voltage[0];
        localVoltage[1] = msg.Voltage[1];
        localVoltage[2] = msg.Voltage[2];
        localResetPylonHeight[0] = msg.Reset_pylon_height[0];
        localResetPylonHeight[1] = msg.Reset_pylon_height[1];
        localResetPylonExpand[0] = msg.Reset_pylon_expand[0];
        localResetPylonExpand[1] = msg.Reset_pylon_expand[1];
        localResetBoxHeight[0] = msg.Reset_box_arm_height[0];
        localResetBoxHeight[1] = msg.Reset_box_arm_height[1];
        localResetBoxHand[0] = msg.Reset_box_arm_hand[0];
        localResetBoxHand[1] = msg.Reset_box_arm_hand[1];
        localResetEArmExpand = msg.Reset_e_arm_expand;
        localResetEArmGet = msg.Reset_e_arm_get;

        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            voltage = localVoltage;
            resetPylonArmHeight = localResetPylonHeight;
            resetPylonArmExpand = localResetPylonExpand;
            resetBoxArmHeight = localResetBoxHeight;
            resetBoxArmHand = localResetBoxHand;
            resetEArmExpand = localResetEArmExpand;
            resetEArmGet = localResetEArmGet;

            voltageCircleGraph1.UpdateCircleGraph(voltage[0], (voltage[0] - min_voltage) / (max_voltage - min_voltage));
            voltageCircleGraph2.UpdateCircleGraph(voltage[1], (voltage[1] - min_voltage) / (max_voltage - min_voltage));
            voltageCircleGraph3.UpdateCircleGraph(voltage[2], (voltage[2] - min_voltage) / (max_voltage - min_voltage));
            
            resetPylonArmHeightToggle1.isOn = resetPylonArmHeight[0];
            resetPylonArmHeightToggle2.isOn = resetPylonArmHeight[1];
            resetPylonArmExpandToggle1.isOn = resetPylonArmExpand[0];
            resetPylonArmExpandToggle2.isOn = resetPylonArmExpand[1];
            resetBoxArmHeightToggle1.isOn = resetBoxArmHeight[0];
            resetBoxArmHeightToggle2.isOn = resetBoxArmHeight[1];
            resetBoxArmHandToggle1.isOn = resetBoxArmHand[0];
            resetBoxArmHandToggle2.isOn = resetBoxArmHand[1];
            resetEArmExpandToggle.isOn = resetEArmExpand;
            resetEArmGetToggle.isOn = resetEArmGet;
        });
    }

    void missingCanIdCallback(IMA msg)
    {
        Debug.Log("Received Missing Can ID");
        int[] localMissingCanId = (int[])msg.Data.Clone();
        
        CustomMainThreadDispatcher.Instance().Enqueue(() =>
        {
            missingCanId = localMissingCanId;
        });
    }

    void conveyorCmdCallback(Cn msg)
    {
        conveyorCmd[0] = msg.Conveyor_rpm[0];
        conveyorCmd[1] = msg.Conveyor_rpm[1];
    }

    void pylonarmCmdCallback(Pl msg)
    {
        pylonArmExpandCmd[0] = msg.Expand[0];
        pylonArmExpandCmd[1] = msg.Expand[1];
        pylonArmHeightCmd[0] = msg.Height[0];
        pylonArmHeightCmd[1] = msg.Height[1];
        pylonArmCollectRpmCmd[0] = msg.Collect_rpm[0];
        pylonArmCollectRpmCmd[1] = msg.Collect_rpm[1];
    }

    void earmCmdCallback(EA msg)
    {
        earmGetCmd = msg.Get;
        earmExpandCmd = msg.Expand;
    }

    void boxarmCmdCallback(Ba msg)
    {
        boxArmExpandCmd[0] = msg.Expand[0];
        boxArmExpandCmd[1] = msg.Expand[1];
        boxArmHeightCmd[0] = msg.Height[0];
        boxArmHeightCmd[1] = msg.Height[1];
        boxArmHandCmd[0] = msg.Hand_position[0];
        boxArmHandCmd[1] = msg.Hand_position[1];
    }

    public void panelTransition()
    {
        boxArmExpandSlider1.value = boxArmExpandCmd[0];
        boxArmExpandSlider2.value = boxArmExpandCmd[1];
        boxArmHeightSlider1.value = boxArmHeightCmd[0];
        boxArmHeightSlider2.value = boxArmHeightCmd[1];
        boxArmHandSlider1.value = boxArmHandCmd[0];
        boxArmHandSlider2.value = boxArmHandCmd[1];
        boxConveyorRpmSlider1.value = conveyorCmd[0];
        boxConveyorRpmSlider2.value = conveyorCmd[1];
        pylonArmHeightSlider1.value = pylonArmHeightCmd[0];
        pylonArmHeightSlider2.value = pylonArmHeightCmd[1];
        pylonArmCollectRpmSlider1.value = pylonArmCollectRpmCmd[0];
        pylonArmCollectRpmSlider2.value = pylonArmCollectRpmCmd[1];
        pylonArmExpandSlider1.value = pylonArmExpandCmd[0];
        pylonArmExpandSlider2.value = pylonArmExpandCmd[1];
        eArmGetSlider.value = earmGetCmd;
        eArmExpandSlider.value = earmExpandCmd;
    }
}