using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ROS2;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using topicSt = std_msgs.msg.String;
using Og = nav_msgs.msg.OccupancyGrid;
using Ps = geometry_msgs.msg.PoseStamped;
using Pa = nav_msgs.msg.Path;
using TS = geometry_msgs.msg.TwistStamped;
using Sw = nhk2025b_msgs.msg.Swerve;
using Rs = nhk2025b_msgs.msg.RobotStatus;
using TMPro;

public class UnitySubscriber : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<Og> map_sub;
    private ISubscription<Ps> currentpose_sub;
    private ISubscription<Ps> goalpose_sub;
    private ISubscription<Pa> path_sub;
    private ISubscription<Ps> lookaheadpose_sub;
    private ISubscription<Sw> result_sub;
    private ISubscription<Sw> cmd_sub;

    private Queue<string> recqueue = new Queue<string>();

    //public Image mapImage;
    [SerializeField] private TMP_Text mapTopicText;
    private string data;
    //Pose Variables
    const float m2pixX = 1589.74f / 10.0f;          // px/m
    const float m2pixY = 813.47f / 5.0f;            // px/m
    const float anchorX = -100f;                    // px
    const float anchorY = -100f;                    // px

    //Current Pose Subscriber
    public GameObject robot;
    private RectTransform robotRectTransform;
    private float posX = anchorX;
    private float posY = anchorY;
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
    public Color minColor = Color.green;
    public Color maxColor = Color.red;
    private Renderer rend;
    [SerializeField] private TMP_Text swerveText0;
    [SerializeField] private TMP_Text swerveText1;
    [SerializeField] private TMP_Text swerveText2;
    [SerializeField] private TMP_Text swerveText3;
    public GameObject[] swerve = new GameObject[4];
    private RectTransform[] swerveRectTransform = new RectTransform[4];
    private float[] wheelAngle = new float[4];
    private float[] wheelSpeed = new float[4];
    private float angleDeg = 0f;

    void Start()
    {
        TryGetComponent(out ros2Unity);
        robotRectTransform = (RectTransform)robot.transform;
        robotRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        goalRectTransform = (RectTransform)goal.transform;
        goalRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        lookaheadRectTransform = (RectTransform)lookahead.transform;
        lookaheadRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);

        for (int i = 0; i < 4; i++)
        {
            swerveRectTransform[i] = (RectTransform)swerve[i].transform;
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
            }
        }
        if (mapTopicText != null) mapTopicText.SetText(data);

        lookaheadRectTransform.anchoredPosition = new Vector3(lposX, lposY, 0f);
        lookahead.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, loriZ, loriW);
        goalRectTransform.anchoredPosition = new Vector3(gposX, gposY, 0f);
        goal.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, goriZ, goriW);
        robotRectTransform.anchoredPosition = new Vector3(posX, posY, 0f);
        robot.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, oriZ, oriW);

        if (swerveText0 != null) swerveText0.SetText($"WheelAngle0: {wheelAngle[0]}°\nWheelSpeed0: {wheelSpeed[0]}rpm");
        if (swerveText1 != null) swerveText1.SetText($"WheelAngle1: {wheelAngle[1]}°\nWheelSpeed1: {wheelSpeed[1]}rpm");
        if (swerveText2 != null) swerveText2.SetText($"WheelAngle2: {wheelAngle[2]}°\nWheelSpeed2: {wheelSpeed[2]}rpm");
        if (swerveText3 != null) swerveText3.SetText($"WheelAngle3: {wheelAngle[3]}°\nWheelSpeed3: {wheelSpeed[3]}rpm");

        for (int i = 0; i < swerve.Length; i++)
        {
            float t = Mathf.Clamp01(Mathf.Abs(wheelSpeed[i]) / maxSpeed);
            Color color = Color.Lerp(minColor, maxColor, t);
            var image = swerve[i]?.GetComponent<UnityEngine.UI.Image>();
            image.color = color;
            swerveRectTransform[i].transform.rotation = Quaternion.Euler(0f, 0f, wheelAngle[i] + 180f);
        }
    }

    void mappingCallback(Og msg)
    {
        /*Debug.Log($"map data: {msg.Info.Resolution}, {msg.Info.Width}, {msg.Info.Height}, " +
                            $"{msg.Info.Origin.Position.X}, {msg.Info.Origin.Position.Y}, {msg.Info.Origin.Position.Z}, {msg.Info.Origin.Orientation.W}");
        data = $"map data: {msg.Info.Resolution}, {msg.Info.Width}, {msg.Info.Height}, " +
                              $"{msg.Info.Origin.Position.X}, {msg.Info.Origin.Position.Y}, {msg.Info.Origin.Position.Z}, {msg.Info.Origin.Orientation.W}";*/
    }

    void currentposeCallback(Ps msg)
    {
        posX = -(float)msg.Pose.Position.X * m2pixY + anchorX;
        posY = -(float)msg.Pose.Position.Y * m2pixX + anchorY;
        oriZ = -(float)msg.Pose.Orientation.Z;
        oriW = -(float)msg.Pose.Orientation.W;
    }

    void goalposeCallback(Ps msg)
    {
        gposX = -(float)msg.Pose.Position.X * m2pixY + anchorX;
        gposY = -(float)msg.Pose.Position.Y * m2pixX + anchorY;
        goriZ = -(float)msg.Pose.Orientation.Z;
        goriW = -(float)msg.Pose.Orientation.W;
    }

    void pathCallback(Pa msg)
    {
    }

    void lookaheadposeCallback(Ps msg)
    {
        lposX = -(float)msg.Pose.Position.X * m2pixY + anchorX;
        lposY = -(float)msg.Pose.Position.Y * m2pixX + anchorY;
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
            }
            else 
            {
                wheelSpeed[i] = (float)msg.Wheel_speed[i];
            }
            wheelAngle[i] = (float)msg.Wheel_angle[i] * Mathf.Rad2Deg;
            if (wheelAngle[i] > 180f) wheelAngle[i] -= 360f;
        }
    }

    void cmdCallback(Sw msg)
    {
    }
}