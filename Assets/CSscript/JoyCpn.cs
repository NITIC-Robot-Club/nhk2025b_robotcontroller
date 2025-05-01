using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using twist = geometry_msgs.msg.Twist;

public class JoyCpn : MonoBehaviour
{
    public FixedJoystick XYJoy;
    public FixedJoystick ZJoy;
    private PubConCutom pubcon;
    private bool is_main;
    [SerializeField] private GameObject uiope;
    private Vector2 leftdsjoy;
    private Vector2 rightdsjoy;
    void Start(){
        pubcon = GameObject.Find("Pubcontoroller").GetComponent<PubConCutom>();
        StartCoroutine(JoyAsync());
    }

    // Update is called once per frame
    void Update(){
        leftdsjoy = uiope.GetComponent<NewDS4con>().Getleftjoy();
        rightdsjoy = uiope.GetComponent<NewDS4con>().Getrightjoy();
        is_main = uiope.GetComponent<PanelContoroller>().Getismain();
    }
    IEnumerator JoyAsync(){
        while(true){
            yield return new WaitForFixedUpdate();
            twist sendtwist = new twist();
            sendtwist.Linear.X = XYJoy.Vertical;
            sendtwist.Linear.Y = (-1)*XYJoy.Horizontal;
            sendtwist.Angular.Z = (-1)*ZJoy.Horizontal;
            if(is_main){sendtwist = new twist();}
            pubcon.twistmsgs.Enqueue(sendtwist);
        }
    }
}
