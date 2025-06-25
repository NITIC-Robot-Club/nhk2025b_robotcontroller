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
using Pe = rcl_interfaces.msg.ParameterEvent;

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
    private ISubscription<Pe> parameter_sub;

    private Queue<string> recqueue = new Queue<string>();

    //Visualize OccupancyGrid
    public RawImage rawImage;
    private int ogWidth;
    private int ogHeight;
    private sbyte[] ogData;
    private Texture2D ogTexture;
    private bool ogDirty = false;
    private const int ogWidthDefault = 1750;        // px
    private const int ogHeightDefault = 960;        // px
    private bool ogIsRed = false;
    private bool isRed = false;

    //Pose Variables
    const float m2pixX = 1750.00f / 10.0f;          // px/m
    const float m2pixY = 960.00f / 5.0f;            // px/m
    // const float anchorX = -75f;                     // px
    // const float anchorY = -75f;                     // px

    //Current Pose Subscriber
    public GameObject robot;
    private RectTransform robotRectTransform;
    private float posX;// = anchorX;
    private float posY;// = anchorY;
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
    const int maxPointCount = 100;
    private Pa subscribedPath;
    private GameObject[] points = new GameObject[100];
    private RectTransform[] pointRectTransforms = new RectTransform[100];
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject pathParent;

    //Visualize Robot State
    [SerializeField] private TMP_Text currentStateText;
    private string currentState = "Current State";
    [SerializeField] GameObject buttonPrefab;
    [SerializeField] GameObject buttonParent;
    private int stateSize;
    private int[] stateID = new int[50];
    private string[] stateName = new string[50];
    private int[] prevStateID = new int[50];
    private string[] prevStateName = new string[50];
    private int prevStateSize = 0;
    private bool stateChanged = false;
    private Vector2 initialPosition = new Vector2(0, 500);

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
            pointRectTransforms[i].anchorMin = new Vector2(1, 1);
            pointRectTransforms[i].anchorMax = new Vector2(1, 1);
            pointRectTransforms[i].pivot = new Vector2(1, 0);
        }
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("UnitySubNode");
                map_sub = ros2Node.CreateSubscription<Og>("/behavior/map", mappingCallback);
                currentpose_sub = ros2Node.CreateSubscription<Ps>("/localization/current_pose", currentposeCallback);
                goalpose_sub = ros2Node.CreateSubscription<Ps>("/behavior/goal_pose", goalposeCallback);
                path_sub = ros2Node.CreateSubscription<Pa>("/planning/path", pathCallback);
                lookaheadpose_sub = ros2Node.CreateSubscription<Ps>("/control/lookahead_pose", lookaheadposeCallback);
                result_sub = ros2Node.CreateSubscription<Sw>("/swerve/result", resultCallback);
                cmd_sub = ros2Node.CreateSubscription<Sw>("/visualization/swerve", cmdCallback);
                state_sub = ros2Node.CreateSubscription<Sa>("/behavior/avaiable_state_array", stateCallback);
                parameter_sub = ros2Node.CreateSubscription<Pe>("/parameter_events", parameterCallback);
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
            lookaheadRectTransform.anchoredPosition = new Vector3(lposX, lposY, 0f);
            lookahead.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, loriZ, loriW);
            goalRectTransform.anchoredPosition = new Vector3(gposX, gposY, 0f);
            goal.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, goriZ, goriW);
            robotRectTransform.anchoredPosition = new Vector3(posX, posY, 0f);
            robot.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, oriZ, oriW);
        }
        else
        {
            lookaheadRectTransform.anchorMin = new Vector2(1, 0);
            lookaheadRectTransform.anchorMax = new Vector2(1, 0);
            goalRectTransform.anchorMin = new Vector2(1, 0);
            goalRectTransform.anchorMax = new Vector2(1, 0);
            robotRectTransform.anchorMin = new Vector2(1, 0);
            robotRectTransform.anchorMax = new Vector2(1, 0);
            lookaheadRectTransform.anchoredPosition = new Vector3(lposX, lposY, 0f);
            lookahead.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, loriZ, loriW);
            goalRectTransform.anchoredPosition = new Vector3(gposX, gposY, 0f);
            goal.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, goriZ, goriW);
            robotRectTransform.anchoredPosition = new Vector3(posX, posY, 0f);
            robot.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, oriZ, oriW);
        }

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
            image.color = swerveColor;
            soriZ = Mathf.Sin(wheelAngle[i] /  Mathf.Rad2Deg / 2.0f);
            soriW = Mathf.Cos(wheelAngle[i] /  Mathf.Rad2Deg / 2.0f);
            swerveRectTransform[i].transform.rotation = new Quaternion(0f, 0f, soriZ, soriW);
        }

        //Visualize OccupancyGrid
        if (ogDirty && ogData != null)
        {
            ogTexture = new Texture2D(ogWidth, ogHeight, TextureFormat.RGBA32, false);
            ogTexture.filterMode = FilterMode.Point;
            ogTexture.wrapMode = TextureWrapMode.Clamp;
            for (int y = 0; y < ogHeight; y++)
            {
                for (int x = 0; x < ogWidth; x++)
                {
                    int index = y * ogWidth + x;
                    sbyte val = ogData[index];
                    Color ogColor;
                    if (val == -1) ogColor = Color.gray;
                    else if (val == 0) ogColor = Color.white;
                    else ogColor = Color.black;
                    ogTexture.SetPixel(ogWidth - x - 1, ogHeight - y - 1, ogColor);
                }
            }
            ogTexture.Apply();

            if (rawImage != null)
            {
                rawImage.texture = ogTexture;
                rawImage.rectTransform.sizeDelta = new Vector2(ogWidthDefault, ogHeightDefault);
            }
            ogDirty = false;
        }

        //Visualize Path
        if(subscribedPath != null && subscribedPath.Poses.Length > 0)
        {
            int setmax=0;
            if(subscribedPath.Poses.Length<maxPointCount) setmax= subscribedPath.Poses.Length;
            else setmax = 100;
            for(int i = 0;i < maxPointCount;i++){
                int num = i*subscribedPath.Poses.Length/maxPointCount;
                if(num > (subscribedPath.Poses.Length-1))num = subscribedPath.Poses.Length-1;

                float px = -(float)subscribedPath.Poses[num].Pose.Position.X*m2pixY;
                float py = -(float)subscribedPath.Poses[num].Pose.Position.Y*m2pixX;

                pointRectTransforms[i] = (RectTransform)points[i].transform;
                pointRectTransforms[i].anchoredPosition = new Vector3(px, py, 0);
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
                GameObject button = Instantiate(buttonPrefab);
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
    }

    void mappingCallback(Og msg)
    {
        ogWidth = (int)msg.Info.Width;
        ogHeight = (int)msg.Info.Height;
        ogData = (sbyte[])msg.Data.Clone();
        ogDirty = true;

    }

    void currentposeCallback(Ps msg)
    {
        if (msg.Pose.Position.Y < 0)
        {
            ogIsRed = true;
        }
        else
        {
            ogIsRed = false;
        }
        posX = -(float)msg.Pose.Position.X * m2pixY;
        posY = -(float)msg.Pose.Position.Y * m2pixX;
        oriZ = -(float)msg.Pose.Orientation.Z;
        oriW = -(float)msg.Pose.Orientation.W;
    }

    void goalposeCallback(Ps msg)
    {
        if (msg.Pose.Position.Y < 0)
        {
            ogIsRed = true;
        }
        else
        {
            ogIsRed = false;
        }
        gposX = -(float)msg.Pose.Position.X * m2pixY;
        gposY = -(float)msg.Pose.Position.Y * m2pixX;
        goriZ = -(float)msg.Pose.Orientation.Z;
        goriW = -(float)msg.Pose.Orientation.W;
    }

    void pathCallback(Pa msg)
    {
        subscribedPath = msg;
    }

    void lookaheadposeCallback(Ps msg)
    {
        if (msg.Pose.Position.Y < 0)
        {
            ogIsRed = true;
        }
        else
        {
            ogIsRed = false;
        }
        lposX = -(float)msg.Pose.Position.X * m2pixY;
        lposY = -(float)msg.Pose.Position.Y * m2pixX;
        loriZ = -(float)msg.Pose.Orientation.Z;
        loriW = -(float)msg.Pose.Orientation.W;
    }

    void resultCallback(Sw msg)
    {
        for (int i = 0; i < 4; i++)
        {
            if (-1.0f <= msg.Wheel_speed[i] && msg.Wheel_speed[i] <= 1.0f)
            {
                wheelSpeed[i] = 0f;
            }
            else if (msg.Wheel_speed[i] < -1.0f)
            {
                wheelSpeed[i] = Mathf.Abs(msg.Wheel_speed[i]);
                msg.Wheel_angle[i] -= Mathf.PI;
            }
            else 
            {
                wheelSpeed[i] = (float)msg.Wheel_speed[i];
            }
            wheelAngle[i] = (float)msg.Wheel_angle[i] * Mathf.Rad2Deg;
        }
    }

    void cmdCallback(Sw msg)
    {
    }

    void stateCallback(Sa msg)
    {
        stateChanged = false;
        stateSize = msg.State.Length;
        currentState = msg.Name;
        if (stateSize != prevStateSize)
        {
            stateChanged = true;
        }
        else
        {
            for (int i = 0; i < stateSize; i++)
            {
                if (stateID[i] != msg.State[i].Id || stateName[i] != msg.State[i].Name)
                {
                    stateChanged = true;
                    break;
                }
            }
        }
        for (int i = 0; i < stateSize; i++)
        {
            stateID[i] = msg.State[i].Id;
            stateName[i] = msg.State[i].Name;
        }
        prevStateSize = stateSize;
        Array.Copy(stateID, prevStateID, stateSize);
        Array.Copy(stateName, prevStateName, stateSize);
    }

    void parameterCallback(Pe msg)
    {
        // foreach (var parameter in msg.Changed_parameters)
        // {
        //     if (parameter.Name == "is_red")
        //     {
        //         ogIsRed = parameter.Value.Bool_value;
        //     }
        // }
    }

    void sendStatus(int status)
    {
        unityPublisher.intQueue.Enqueue(status);
        Debug.Log("SendStatus: " + status.ToString());
    }
}