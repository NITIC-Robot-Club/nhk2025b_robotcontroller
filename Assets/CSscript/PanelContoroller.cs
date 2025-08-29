using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject chassisPanel;
    [SerializeField] GameObject conveyorPanel;
    [SerializeField] GameObject boxArmPanel;
    [SerializeField] GameObject pylonArmPanel;
    [SerializeField] GameObject autoPanel;
    [SerializeField] GameObject manualPanel;
    [SerializeField] private SpriteSwitcher manualSwitcher;
    [SerializeField] private Button manualSwitcherButton;
    [SerializeField] private Button[] boxArmButtons;
    [SerializeField] private Button[] conveyorButtons;
    [SerializeField] private Button[] chassisButtons;
    [SerializeField] private Button[] pylonArmButtons;
    private bool manualSwitcherState = false;
    private bool is_autoPanel = true;
    private bool isChassisPanel = false;
    private bool isConveyorPanel = false;
    private bool isBoxArmPanel = false;
    private bool isPylonArmPanel = false;
    public GameObject JoyCpn;

    void Start() 
    {
        manualSwitcherButton.onClick.AddListener(manualSwitcherOn);
        autoOn();
        foreach (Button button in boxArmButtons)
        {
            button.onClick.AddListener(boxArmOn);
        }
        foreach (Button button in conveyorButtons)
        {
            button.onClick.AddListener(conveyorOn);
        }
        foreach (Button button in chassisButtons)
        {
            button.onClick.AddListener(chassisOn);
        }
        foreach (Button button in pylonArmButtons)
        {
            button.onClick.AddListener(pylonArmOn);
        }
    }

    void autoOn()
    {
        is_autoPanel = true;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = false;
        autoPanel.SetActive(true);
        manualPanel.SetActive(false);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }

    void manualSwitcherOn()
    {
        manualSwitcherState = !manualSwitcherState;
        if (manualSwitcherState)
        {
            chassisOn();
        }
        else
        {
            autoOn();
        }
    }

    void boxArmOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = true;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(true);
        conveyorPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
    }

    void conveyorOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = true;
        isChassisPanel = false;
        isPylonArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(true);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
    }

    void chassisOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = true;
        isPylonArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(true);
        pylonArmPanel.SetActive(false);
    }

    void pylonArmOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = true;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(true);
    }

    string GetCurrentPanel()
    {
        if (is_autoPanel)
        {
            return "Auto";
        }
        else if (isBoxArmPanel)
        {
            return "BoxArm";
        }
        else if (isConveyorPanel)
        {
            return "Conveyor";
        }
        else if (isChassisPanel)
        {
            return "Chassis";
        }
        else if (isPylonArmPanel)
        {
            return "PylonArm";
        }
        else
        {
            return null;
        }
    }

    public bool getIsAuto()
    {
        return is_autoPanel;
    }

    public bool getIsChassis()
    {
        return isChassisPanel;
    }

    public bool getIsConveyor()
    {
        return isConveyorPanel;
    }

    public bool getIsBoxArm()
    {
        return isBoxArmPanel;
    }

    public bool getIsPylonArm()
    {
        return isPylonArmPanel;
    }
}
