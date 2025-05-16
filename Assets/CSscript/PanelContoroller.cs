using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject conPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject infoPanel;
    [SerializeField] Button toCon;
    //[SerializeField] Button toMain;
    [SerializeField] private SpriteSwitcher conSwitcher;
    [SerializeField] private SpriteSwitcher infoSwitcher;
    private bool is_mainpanel = true;
    public GameObject JoyCpn;
    private bool previousMain = true;
    private bool previousCon = false;
    private bool previousInfo = false;
 
    void Start () {
        toCon.onClick.AddListener(conOn);
        //toMain.onClick.AddListener(mainOn);
        mainOn();
    }
    void Update()
    {
        UpdatePanelState();
    }

    void conOn(){
        is_mainpanel = false;
        conPanel.SetActive(true);
        mainPanel.SetActive(false);
    }
    void mainOn(){
        is_mainpanel = true;
        conPanel.SetActive(false);
        mainPanel.SetActive(true);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }
    void UpdatePanelState()
    {
        if (conSwitcher.GetisOn())
        {
            if (infoSwitcher.GetisOn())
            {
                is_mainpanel = false;
                previousMain = false;
                previousCon = false;
                previousInfo = true;
                infoPanel.SetActive(false);
                conPanel.SetActive(true);
            }
            else 
            {
                is_mainpanel = false;
                previousMain = false;
                previousCon = true;
                previousInfo = false;
                conPanel.SetActive(true);
                mainPanel.SetActive(false);
            }
        }
        else
        {
            if (infoSwitcher.GetisOn())
            {
                is_mainpanel = false;
                previousMain = false;
                previousCon = false;
                previousInfo = true;
                conPanel.SetActive(false);
                infoPanel.SetActive(true);
            }
            else
            {
                is_mainpanel = true;
                previousMain = true;
                previousCon = false;
                previousInfo = false;
                conPanel.SetActive(false);
                mainPanel.SetActive(true);
            }
        }
        if (infoSwitcher.GetisOn())
        {
            if (is_mainpanel)
            {
                previousMain = true;
                previousCon = false;
                previousInfo = true;
            }
            else
            {
                previousMain = false;
                previousCon = true;
                previousInfo = false;
            }
            is_mainpanel = false;
            conPanel.SetActive(false);
            mainPanel.SetActive(false);
            infoPanel.SetActive(true);
            JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
        }
        else
        {
            if (previousMain)
            {
                is_mainpanel = true;
                conPanel.SetActive(false);
                mainPanel.SetActive(true);
                infoPanel.SetActive(false);
                JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
            }
            else if (previousCon)
            {
                is_mainpanel = false;
                conPanel.SetActive(true);
                mainPanel.SetActive(false);
                infoPanel.SetActive(false);
                JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
            }
            else if (previousInfo)
            {
                is_mainpanel = false;
                conPanel.SetActive(false);
                mainPanel.SetActive(false);
                infoPanel.SetActive(true);
                JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
            }
        }
    }
    public bool Getismain(){
        return is_mainpanel;
    }
}
