using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject manualPanel; // conPanel → manualPanel
    [SerializeField] GameObject autoPanel;   // mainPanel → autoPanel
    //[SerializeField] Button toMain;
    [SerializeField] private SpriteSwitcher manualSwitcher; // conSwitcher → manualSwitcher
    [SerializeField] private Button manualSwitcherButton;   // conSwitcherButton → manualSwitcherButton
    private bool manualSwitcherState = false;
    private bool is_autoPanel = true;
    private bool is_manualPanel = false;
    public GameObject JoyCpn;
    private string previousPanel = "Auto";

    void Start () {
        manualSwitcherButton.onClick.AddListener(manualSwitcherOn);
        autoOn();
    }

    void autoOn(){
        is_autoPanel = true;
        is_manualPanel = false;
        autoPanel.SetActive(true);
        manualPanel.SetActive(false);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }

    void manualOn(){
        is_autoPanel = false;
        is_manualPanel = true;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
    }

    void manualSwitcherOn(){
        manualSwitcherState = !manualSwitcherState;
        if (manualSwitcherState)
        {
            previousPanel = GetCurrentPanel();
            manualOn();
        }
        else
        {
            autoOn();
        }
    }

    string GetCurrentPanel()
    {
        if (is_autoPanel)
        {
            return "Auto";
        }
        else if (is_manualPanel)
        {
            return "Manual";
        }
        else
        {
            return null;
        }
    }

    private string getPreviousPanel()
    {
        return previousPanel;
    }

    public bool getIsAuto(){
        return is_autoPanel;
    }

    public bool getIsManual(){
        return is_manualPanel;
    }
}
