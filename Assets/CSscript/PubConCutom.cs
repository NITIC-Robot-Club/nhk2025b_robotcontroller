using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ROS2;
using topicSt = std_msgs.msg.String;
using twist = geometry_msgs.msg.Twist;
using Ts = twistring.msg.Twistring;
using TMPro;
public class PubConCutom : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private IPublisher<Ts> pub;
    [SerializeField] private GameObject uiope;
    private int target_num = 0;
    [System.NonSerialized] public Queue<string> queue = new Queue<string>();
    [System.NonSerialized] public Queue<twist> twistmsgs = new Queue<twist>();
    private IEnumerator routine;
    //[SerializeField] private TMP_Text tmp;
    //[SerializeField] private Button upButton;
    //[SerializeField] private Button downButton;
    [SerializeField] private string topic_name = "/my_topic_name";
    [SerializeField] private float pub_hz = 0.05f;
    private bool is_main;
    void Start()
    {
        TryGetComponent(out ros2Unity);
        routine = PublishTwistring();
        //upButton.onClick.AddListener(num_p);
        //downButton.onClick.AddListener(num_m);
    }
    /*void num_p(){
        if(target_num < 100)target_num++;
    }
    void num_m(){
        if(target_num > 0)target_num--;
    }*/
    void Update()
    {
        is_main = uiope.GetComponent<PanelContoroller>().Getismain();
        //tmp.SetText(target_num.ToString());
        if(ros2Unity.Ok()){
            if(ros2Node == null){
                ros2Node = ros2Unity.CreateNode("UnityNode");
                pub = ros2Node.CreatePublisher<Ts>(topic_name);
                StartCoroutine(routine);
            }
        }
    }
    IEnumerator PublishTwistring(){
        while(true){
            Ts msg = new Ts();
            if(queue.Count!=0)msg.Cmd = queue.Dequeue();
            while(twistmsgs.Count!=0)msg.Twist = twistmsgs.Dequeue();
            msg.Id = Convert.ToSByte(target_num);
            if(msg.Cmd!="" && is_main || !is_main){
                pub.Publish(msg);
            }
            yield return new WaitForSeconds(pub_hz);
        }
    }
}
