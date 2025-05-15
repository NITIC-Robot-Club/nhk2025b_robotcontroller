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
    private ISubscription<Ps> lookahreadpose_sub;
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


    void Start()
    {
        TryGetComponent(out ros2Unity);
        robotRectTransform = (RectTransform)robot.transform;
        robotRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        goalRectTransform = (RectTransform)goal.transform;
        goalRectTransform.transform.rotation = Quaternion.Euler(0f, 0f, 90f);
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
                lookahreadpose_sub = ros2Node.CreateSubscription<Ps>("/control/lookahread_pose", lookahreadposeCallback);
                result_sub = ros2Node.CreateSubscription<Sw>("/visualization/swerve/result", resultCallback);
                cmd_sub = ros2Node.CreateSubscription<Sw>("/visualization/swerve/cmd", cmdCallback);
            }
        }
        mapTopicText.SetText(data);

        goalRectTransform.anchoredPosition = new Vector3(gposX, gposY, 0f);
        goal.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, goriZ, goriW);
        robotRectTransform.anchoredPosition = new Vector3(posX, posY, 0f);
        robot.transform.rotation = Quaternion.Euler(0f, 0f, 90f) * new Quaternion(0f, 0f, oriZ, oriW);
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

    void lookahreadposeCallback(Ps msg)
    {
    }

    void resultCallback(Sw msg)
    {
    }

    void cmdCallback(Sw msg)
    {
    }
}