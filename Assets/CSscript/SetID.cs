using UnityEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using ROS2;
using TMPro;
using UnityEngine.UI;

    public class SetID : MonoBehaviour
    {
        [SerializeField] private UInt32 domain_ID = 7;
        void Start(){
            Environment.SetEnvironmentVariable("ROS_DOMAIN_ID", domain_ID.ToString());
            string value = Environment.GetEnvironmentVariable("ROS_DOMAIN_ID");
            Debug.Log("current ROS_DOMAIN_ID:" + value);
        }
    }
