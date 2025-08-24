using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject manualPanel;
    [SerializeField] GameObject chassisPanel;
    [SerializeField] GameObject conveyorPanel;
    [SerializeField] GameObject boxArmPanel;
    [SerializeField] GameObject pylonArmPanel;
    [SerializeField] GameObject autoPanel;
    [SerializeField] private SpriteSwitcher manualSwitcher;
    [SerializeField] private Button manualSwitcherButton;
    [SerializeField] private Button chassisToConveyorButton;
    [SerializeField] private Button conveyorToChassisButton;
    [SerializeField] private Button chassisToPylonArmButton;
    [SerializeField] private Button pylonArmToChassisButton;
    [SerializeField] private Button conveyorToBoxArmButton;
    [SerializeField] private Button boxArmToConveyorButton;
    [SerializeField] private Button pylonArmToBoxArmButton;
    [SerializeField] private Button boxArmToPylonArmButton;
    private bool manualSwitcherState = false;
    private bool is_autoPanel = true;
    private bool is_manualPanel = false;
    private bool isChassisPanel = false;
    private bool isConveyorPanel = false;
    private bool isBoxArmPanel = false;
    private bool isPylonArmPanel = false;
    public GameObject JoyCpn;
    private string previousPanel = "Auto";

    void Start () {
        manualSwitcherButton.onClick.AddListener(manualSwitcherOn);
        autoOn();
        chassisToConveyorButton.onClick.AddListener(chassisToConveyor);
        conveyorToChassisButton.onClick.AddListener(conveyorToChassis);
        chassisToPylonArmButton.onClick.AddListener(chassisToPylonArm);
        pylonArmToChassisButton.onClick.AddListener(pylonArmToChassis);
        conveyorToBoxArmButton.onClick.AddListener(conveyorToBoxArm);
        boxArmToConveyorButton.onClick.AddListener(boxArmToConveyor);
        pylonArmToBoxArmButton.onClick.AddListener(pylonArmToBoxArm);
        boxArmToPylonArmButton.onClick.AddListener(boxArmToPylonArm);
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
        isChassisPanel = true;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        chassisPanel.SetActive(true);
        conveyorPanel.SetActive(false);
        boxArmPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
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

    void chassisToPylonArm(){
        isChassisPanel = false;
        isPylonArmPanel = true;
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(true);
    }

    void conveyorToChassis(){
        isConveyorPanel = false;
        isChassisPanel = true;
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(true);
    }

    void conveyorToBoxArm(){
        isConveyorPanel = false;
        isBoxArmPanel = true;
        conveyorPanel.SetActive(false);
        boxArmPanel.SetActive(true);
    }

    void chassisToConveyor(){
        isChassisPanel = false;
        isConveyorPanel = true;
        chassisPanel.SetActive(false);
        conveyorPanel.SetActive(true);
    }

    void pylonArmToChassis(){
        isPylonArmPanel = false;
        isChassisPanel = true;
        pylonArmPanel.SetActive(false);
        chassisPanel.SetActive(true);
    }

    void boxArmToConveyor(){
        isBoxArmPanel = false;
        isConveyorPanel = true;
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(true);
    }

    void pylonArmToBoxArm(){
        isPylonArmPanel = false;
        isBoxArmPanel = true;
        pylonArmPanel.SetActive(false);
        boxArmPanel.SetActive(true);
    }

    void boxArmToPylonArm(){
        isBoxArmPanel = false;
        isPylonArmPanel = true;
        boxArmPanel.SetActive(false);
        pylonArmPanel.SetActive(true);
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

    public bool getIsChassis(){
        return isChassisPanel;
    }

    public bool getIsConveyor(){
        return isConveyorPanel;
    }

    public bool getIsBoxArm(){
        return isBoxArmPanel;
    }

    public bool getIsPylonArm(){
        return isPylonArmPanel;
    }
}
