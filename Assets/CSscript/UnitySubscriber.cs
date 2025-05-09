using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using ROS2;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using topicSt = std_msgs.msg.String;
using Ts = twistring.msg.Twistring;
using Og = nav_msgs.msg.OccupancyGrid;
using Ps = geometry_msgs.msg.PoseStamped;
using Pa = nav_msgs.msg.Path;
using TS = geometry_msgs.msg.TwistStamped;
using Swerve = nhk2025b_msgs.msg.Swerve;

public class UnitySubscriber : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<Ts> msg_sub;
    private ISubscription<Og> map_sub;
    private ISubscription<Ps> currentpose_sub;
    private ISubscription<Ps> goalpose_sub;
    private ISubscription<Pa> path_sub;
    private ISubscription<Ps> lookahreadpose_sub;

    private Queue<string> recqueue = new Queue<string>();
    [SerializeField] private string msgTopicName = "/my_sub_topic_name";
    private string mapTopicName = "/behavior/map";
    private string currentposeToicName = "localization/current_pose";
    private string goalposeTopicName = "/control/lookahread_pose";
    private string pathTopicName = "/planning/path";
    private string lookahreadposeTopicName = "/visualization/swerve/result";
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
                msg_sub = ros2Node.CreateSubscription<Ts>(cmdTopicName, callback);
                map_sub = ros2Node.CreateSubscription<Og>(mapTopicName, MapCallback);
            }
        }
    }

    void callback(Ts msg)
    {
        if (msg.Cmd != "") recqueue.Enqueue("ID." + msg.Id + ":" + msg.Cmd);
    }

    void MapCallback(Og msg)
    {
        var info = msg.Info;
        Debug.Log($"Map Info:");
        Debug.Log($"  Width: {info.Width}");
        Debug.Log($"  Height: {info.Height}");
        Debug.Log($"  Resolution: {info.Resolution}");
        Debug.Log($"  Origin Position: ({info.Origin.Position.X}, {info.Origin.Position.Y}, {info.Origin.Position.Z})");
        Debug.Log($"  Origin Orientation: ({info.Origin.Orientation.X}, {info.Origin.Orientation.Y}, {info.Origin.Orientation.Z}, {info.Origin.Orientation.W})");
    }
}
