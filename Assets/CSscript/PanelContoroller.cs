using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject conPanel;
    [SerializeField] GameObject mainPanel;
    [SerializeField] GameObject infoPanel;
    //[SerializeField] Button toMain;
    [SerializeField] private SpriteSwitcher conSwitcher;
    [SerializeField] private Button conSwitcherButton;
    private bool conSwitcherState = false;
    [SerializeField] private SpriteSwitcher infoSwitcher;
    [SerializeField] private Button infoSwitcherButton;
    private bool infoSwitcherState = false;
    private bool is_mainpanel = true;
    private bool is_conpanel = false;
    private bool is_infopanel = false;
    public GameObject JoyCpn;
    private string previousPanel = "Main";
 
    void Start () {
        conSwitcherButton.onClick.AddListener(conSwitcherOn);
        infoSwitcherButton.onClick.AddListener(infoSwitcherOn);
        mainOn();
    }

    void Update()
    {
    }

    void mainOn(){
        is_mainpanel = true;
        is_conpanel = false;
        is_infopanel = false;
        mainPanel.SetActive(true);
        conPanel.SetActive(false);
        infoPanel.SetActive(false);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }

    void conOn(){
        is_mainpanel = false;
        is_conpanel = true;
        is_infopanel = false;
        mainPanel.SetActive(false);
        conPanel.SetActive(true);
        infoPanel.SetActive(false);
    }

    void infoOn(){
        is_mainpanel = false;
        is_conpanel = false;
        is_infopanel = true;
        mainPanel.SetActive(false);
        conPanel.SetActive(false);
        infoPanel.SetActive(true);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }

    void conSwitcherOn(){
        conSwitcherState = !conSwitcherState;
        if (conSwitcherState)
        {
            previousPanel = GetCurrentPanel();
            conOn();
        }
        else
        {
            mainOn();
        }
    }

    void infoSwitcherOn(){
        infoSwitcherState = !infoSwitcherState;
        if (infoSwitcherState)
        {
            previousPanel = GetCurrentPanel();
            infoOn();
        }
        else
        {
            if (previousPanel == "Main")
            {
                mainOn();
            }
            else if (previousPanel == "Con")
            {
                conOn();
            }
        }
    }

    string GetCurrentPanel()
    {
        if (is_mainpanel)
        {
            return "Main";
        }
        else if (is_conpanel)
        {
            return "Con";
        }
        else if (is_infopanel)
        {
            return "Info";
        }
        else
        {
            return null;
        }
    }

    private string GetPreviousPanel()
    {
        return previousPanel;
    }

    public bool Getismain(){
        return is_mainpanel;
    }

    public bool Getiscon(){
        return is_conpanel;
    }

    public bool Getisinfo(){
        return is_infopanel;
    }
}
