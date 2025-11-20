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
    [SerializeField] GameObject eArmPanel;
    [SerializeField] GameObject autoPanel;
    [SerializeField] GameObject manualPanel;
    [SerializeField] private SpriteSwitcher manualSwitcher;
    [SerializeField] private Button manualSwitcherButton;
    [SerializeField] private Button[] boxArmButtons;
    [SerializeField] private Button[] conveyorButtons;
    [SerializeField] private Button[] chassisButtons;
    [SerializeField] private Button[] pylonArmButtons;
    [SerializeField] private Button[] eArmButtons;
    private bool manualSwitcherState = true;
    private bool is_autoPanel = false;
    private bool isChassisPanel = true;
    private bool isConveyorPanel = false;
    private bool isBoxArmPanel = false;
    private bool isPylonArmPanel = false;
    private bool isEArmPanel = false;
    public GameObject JoyCpn;

    //Visualize Information Panel
    [SerializeField] private Button infoButton;
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private GameObject vChassisPanel;
    [SerializeField] private GameObject vConveyorPanel;
    [SerializeField] private GameObject vBoxArmPanel;
    [SerializeField] private GameObject vPylonArmPanel;
    [SerializeField] private GameObject vEArmPanel;

    private bool isInfoActive = false;

    // [SerializeField] private UnitySubscriber sub;

    void Start() 
    {
        manualSwitcherButton.onClick.AddListener(manualSwitcherOn);
        // autoOn();
        chassisOn();
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
        foreach (Button button in eArmButtons)
        {
            button.onClick.AddListener(eArmOn);
        }
    }

    void Update()
    {
        isInfoActive = infoButton.GetComponent<HoldPanelAction>().getIsInfoActive();
        if (isInfoActive)
        {
            if (is_autoPanel)
            {
                vChassisPanel.SetActive(true);
                vConveyorPanel.SetActive(false);
                vBoxArmPanel.SetActive(false);
                vPylonArmPanel.SetActive(false);
                vEArmPanel.SetActive(false);
            }
            else if (isChassisPanel)
            {
                vChassisPanel.SetActive(true);
                vConveyorPanel.SetActive(false);
                vBoxArmPanel.SetActive(false);
                vPylonArmPanel.SetActive(false);
                vEArmPanel.SetActive(false);
            }
            else if (isConveyorPanel)
            {
                vChassisPanel.SetActive(false);
                vConveyorPanel.SetActive(true);
                vBoxArmPanel.SetActive(false);
                vPylonArmPanel.SetActive(false);
                vEArmPanel.SetActive(false);
            }
            else if (isBoxArmPanel)
            {
                vChassisPanel.SetActive(false);
                vConveyorPanel.SetActive(false);
                vBoxArmPanel.SetActive(true);
                vPylonArmPanel.SetActive(false);
                vEArmPanel.SetActive(false);
            }
            else if (isPylonArmPanel)
            {
                vChassisPanel.SetActive(false);
                vConveyorPanel.SetActive(false);
                vBoxArmPanel.SetActive(false);
                vPylonArmPanel.SetActive(true);
                vEArmPanel.SetActive(false);
            }
            else if (isEArmPanel)
            {
                vChassisPanel.SetActive(false);
                vConveyorPanel.SetActive(false);
                vBoxArmPanel.SetActive(false);
                vPylonArmPanel.SetActive(false);
                vEArmPanel.SetActive(true);
            }
        }
    }

    void autoOn()
    {
        is_autoPanel = true;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = false;
        isEArmPanel = false;
        autoPanel.SetActive(true);
        manualPanel.SetActive(false);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
        eArmPanel.SetActive(false);
        JoyCpn.GetComponent<UnityPublisher>().ResetJoystickInput();
    }

    void manualSwitcherOn()
    {
        manualSwitcherState = !manualSwitcherState;
        if (manualSwitcherState)
        {
            chassisOn();
            // sub.panelTransition();
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
        isEArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(true);
        conveyorPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
        chassisPanel.SetActive(false);
        eArmPanel.SetActive(false);
    }

    void conveyorOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = true;
        isChassisPanel = false;
        isPylonArmPanel = false;
        isEArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(true);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
        eArmPanel.SetActive(false);
    }

    void chassisOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = true;
        isPylonArmPanel = false;
        isEArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(true);
        pylonArmPanel.SetActive(false);
        eArmPanel.SetActive(false);
    }

    void pylonArmOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = true;
        isEArmPanel = false;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(true);
        eArmPanel.SetActive(false);
    }

    void eArmOn()
    {
        is_autoPanel = false;
        isBoxArmPanel = false;
        isConveyorPanel = false;
        isChassisPanel = false;
        isPylonArmPanel = false;
        isEArmPanel = true;
        autoPanel.SetActive(false);
        manualPanel.SetActive(true);
        boxArmPanel.SetActive(false);
        conveyorPanel.SetActive(false);
        chassisPanel.SetActive(false);
        pylonArmPanel.SetActive(false);
        eArmPanel.SetActive(true);
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
        else if (isEArmPanel)
        {
            return "EArm";
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

    public bool getIsEArm()
    {
        return isEArmPanel;
    }
}
