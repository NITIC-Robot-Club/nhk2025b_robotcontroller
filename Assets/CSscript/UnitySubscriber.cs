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
    private string mapTopicName = "/behavior/map";
    private string currentposeToicName = "localization/current_pose";
    private string goalposeTopicName = "/behavior/goal_pose";
    private string pathTopicName = "/planning/path";
    private string lookahreadposeTopicName = "/control/lookahread_pose";
    private string resultTopicName = "/visualization/swerve/result";
    private string cmdTopicName = "/visualization/swerve/cmd";

    public Image mapImage;

    void Start()
    {
        TryGetComponent(out ros2Unity);
    }

    void Update()
    {
        if (ros2Unity.Ok())
        {
            if (ros2Node == null)
            {
                ros2Node = ros2Unity.CreateNode("UnitySubNode");
                map_sub = ros2Node.CreateSubscription<Og>(mapTopicName, mapCallback);
                currentpose_sub = ros2Node.CreateSubscription<Ps>(currentposeToicName, currentposeCallback);
                goalpose_sub = ros2Node.CreateSubscription<Ps>(goalposeTopicName, goalposeCallback);
                path_sub = ros2Node.CreateSubscription<Pa>(pathTopicName, pathCallback);
                lookahreadpose_sub = ros2Node.CreateSubscription<Ps>(lookahreadposeTopicName, lookahreadposeCallback);
                result_sub = ros2Node.CreateSubscription<Sw>(resultTopicName, resultCallback);
                cmd_sub = ros2Node.CreateSubscription<Sw>(cmdTopicName, cmdCallback);
            }
        }
    }

    void mapCallback(Og msg)
    {
        Debug.Log("Map received.");
    }

    void currentposeCallback(Ps msg)
    {
        Debug.Log("Current Pose: " + msg.Pose.Position.X + ", " + msg.Pose.Position.Y);
    }

    void goalposeCallback(Ps msg)
    {
        Debug.Log("Goal Pose: " + msg.Pose.Position.X + ", " + msg.Pose.Position.Y);
    }

    void pathCallback(Pa msg)
    {
    }

    void lookahreadposeCallback(Ps msg)
    {
        Debug.Log("Look Ahead Pose: " + msg.Pose.Position.X + ", " + msg.Pose.Position.Y);
    }

    void resultCallback(Sw swerveResult)
    {
    }

    void cmdCallback(Sw msg)
    {
    }
}