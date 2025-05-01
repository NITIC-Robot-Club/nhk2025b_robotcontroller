using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ROS2;
using topicSt = std_msgs.msg.String;
using Ts = twistring.msg.Twistring;
using System.Linq;
public class SubConCutom : MonoBehaviour
{
    private ROS2UnityComponent ros2Unity;
    private ROS2Node ros2Node;
    private ISubscription<Ts> sub;
    private Queue<string> recqueue = new Queue<string>();
    [SerializeField] private string topic_name = "/my_sub_topic_name";
    
    // Start is called before the first frame update
    void Start()
    {
        TryGetComponent(out ros2Unity);
    }
    void Update()
    {
        if(ros2Unity.Ok()){
            if(ros2Node == null){
                ros2Node = ros2Unity.CreateNode("UnitySubNode");
                sub = ros2Node.CreateSubscription<Ts>(topic_name,callback);
            }
        }
        while(recqueue.Count != 0){
            Debug.Log(recqueue.Dequeue());
        }
    }
    void callback(Ts msg)
    {
        if(msg.Cmd!="")recqueue.Enqueue("ID."+msg.Id+":"+msg.Cmd);
    }
    
}
