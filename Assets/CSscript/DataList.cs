using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using ROS2;
using topicSt = std_msgs.msg.String;
using Ts = twistring.msg.Twistring;
[System.Serializable]
public class Data {
    public Button button;
    public string topic;
}
public class DataList : MonoBehaviour
{
    [SerializeField] List<Data> data;
    private PubConCutom pubcon;
    // Start is called before the first frame update
    void Start(){
        Debug.Log(data.Count);
        pubcon = GameObject.Find("Pubcontoroller").GetComponent<PubConCutom>();
        
        for(int count = 0;count < data.Count;count++){
            Button button1 = data[count].button;
            string string1 = data[count].topic;
            button1.onClick.AddListener(() => SendMsg(string1));
        }
    }
    void Update(){
    }
    void SendMsg(string msg){
        pubcon.queue.Enqueue(msg);
    }
}
