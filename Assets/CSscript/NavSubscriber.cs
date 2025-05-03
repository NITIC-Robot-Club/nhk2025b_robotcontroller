using System;
using System.Collections;
using System.Collections.Generic;
using ROS2;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Ps = geometry_msgs.msg.PoseStamped;
using Og = nav_msgs.msg.OccupancyGrid;
using Pa = nav_msgs.msg.Path;
using Ts = geometry_msgs.msg.TwistStamped;

public class NavSubscriber : MonoBehaviour
{
    public GameObject Robot;
    public GameObject goal;
    private RectTransform robotrect;
    private RectTransform goalrect;
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<Ps> currentpose_sub;
    private ISubscription<Ps> goalpose_sub;
    private ISubscription<Og> map_sub;
    private ISubscription<Pa> path_sub;
    private ISubscription<Ts> currentvelocity_sub;
    private float translation_x;
    private float translation_y;
    private float rotation_z;
    private float rotation_w;
    private float translation_goal_x;
    private float translation_goal_y;
    private float rotation_goal_z;
    private float rotation_goal_w;
    private bool _destroy;
    private int m2pix = 280;
    private Vector3 testvec = new Vector3(-224.0f, 196.0f, 0);
    private Vector3 nowvec;

    private int MaxPointCount = 100;
    private Pa subpath;
    private GameObject[] points = new GameObject[100];
    private RectTransform[] rects = new RectTransform[100];
    [SerializeField] GameObject pointPrefab;
    [SerializeField] GameObject parentobj;
    [SerializeField] float pointSize = 0.02f;
    [SerializeField] private TMP_Text tmp;
    private string data;

    void Start()
    {
        TryGetComponent(out ros2Unity);
        goalrect = (RectTransform)goal.transform;
        goalrect.anchorMin = new Vector2(1, 0);
        goalrect.anchorMax = new Vector2(1, 0);
        robotrect = (RectTransform)Robot.transform;
        Debug.Log("aiu");
        for (int i = 0; i < MaxPointCount; i++)
        {
            points[i] = Instantiate(pointPrefab);
            points[i].transform.SetParent(parentobj.transform);
            points[i].transform.localScale = Vector3.one;
            rects[i] = (RectTransform)points[i].transform;
            rects[i].anchorMin = new Vector2(1, 0);
            rects[i].anchorMax = new Vector2(1, 0);
        }
    }

    void Update()
    {
        if (Environment.GetEnvironmentVariable("ROS_DOMAIN_ID") == "0")
        {
            if (ros2Unity.Ok())
            {
                if (ros2Node == null)
                {
                    ros2Node = ros2Unity.CreateNode("UnitynavsubNode");
                    currentpose_sub = ros2Node.CreateSubscription<Ps>("localization/current_pose", Callback);
                    goalpose_sub = ros2Node.CreateSubscription<Ps>("bt/goal_pose", goalcallback);
                    map_sub = ros2Node.CreateSubscription<Og>("bt/map", MapCallback);
                    path_sub = ros2Node.CreateSubscription<Pa>("planning/path", PathCallback);
                    currentvelocity_sub = ros2Node.CreateSubscription<Ts>("visualization/current_velocity", VelocityCallback);
                    _destroy = false;
                }
            }
        }
        else
        {
            if (!_destroy && ros2Node != null)
            {
                ros2Node.RemoveSubscription<Ps>(currentpose_sub);
                ros2Unity.RemoveNode(ros2Node);
                ros2Node = null;
                _destroy = true;
            }
        }

        if (subpath != null && subpath.Poses.Length > 0)
        {
            int setmax = 0;
            if (subpath.Poses.Length < MaxPointCount) setmax = subpath.Poses.Length;
            else setmax = 100;
            for (int i = 0; i < MaxPointCount; i++)
            {
                int num = i * subpath.Poses.Length / MaxPointCount;
                if (num > (subpath.Poses.Length - 1)) num = subpath.Poses.Length - 1;
                float y = (float)subpath.Poses[num].Pose.Position.X * m2pix + 196.0f;
                float x = (float)subpath.Poses[num].Pose.Position.Y * -1 * m2pix - 224.0f;
                rects[i] = (RectTransform)points[i].transform;
                rects[i].anchoredPosition = new Vector3(x, y, 0);
            }
        }
        tmp.SetText(data);
        nowvec = goalrect.anchoredPosition;
        goalrect.anchoredPosition = new Vector3(translation_goal_x, translation_goal_y, 0);
        goal.transform.rotation = new Quaternion(0, 0, rotation_goal_z, rotation_goal_w);
        robotrect.anchoredPosition = new Vector3(translation_x, translation_y, 0);
        Robot.transform.rotation = new Quaternion(0, 0, rotation_z, rotation_w);
    }

    void VelocityCallback(Ts twist)
    {
        data = $"Linear: {twist.Twist.Linear.X}, Angular: {twist.Twist.Angular.Z}";
    }

    void PathCallback(Pa subpa)
    {
        subpath = subpa;
    }

    void Callback(Ps subps)
    {
        translation_y = (float)subps.Pose.Position.X * m2pix + 196.0f;
        translation_x = (float)subps.Pose.Position.Y * -1 * m2pix - 224.0f;
        rotation_z = (float)subps.Pose.Orientation.Z * -1;
        rotation_w = (float)subps.Pose.Orientation.W * -1;
    }

    void goalcallback(Ps subps)
    {
        translation_goal_y = (float)subps.Pose.Position.X * m2pix + 196.0f;
        translation_goal_x = (float)subps.Pose.Position.Y * -1 * m2pix - 224.0f;
        rotation_goal_z = (float)subps.Pose.Orientation.Z * -1;
        rotation_goal_w = (float)subps.Pose.Orientation.W * -1;
    }

    void MapCallback(Og map)
    {
    }
}
