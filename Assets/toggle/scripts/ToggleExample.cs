using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleExample : MonoBehaviour
{
    [SerializeField] GameObject speedtoggle;
    private bool ison;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        speedtoggle.GetComponent<IOSButton>().Set();
        if(ison!=speedtoggle.GetComponent<IOSButton>().GetisOn()){
            ison = speedtoggle.GetComponent<IOSButton>().GetisOn();
            if(ison){
                Debug.Log("On");
            }else{
                Debug.Log("Off");
            }
        }
    }
}
