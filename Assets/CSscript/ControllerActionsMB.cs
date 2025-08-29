using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
[System.Serializable]
public class AndButton {
    public AndButtonList andbutton;
    public Button button;
    public AndPanelList panel;
}
public class ControllerActionsMB : MonoBehaviour
{
    [SerializeField] GameObject uiope;
    [SerializeField] List<AndButton> AndData;
    private GameInputs _gameInputs;
    private AndPanelList nowpanel;
    private bool is_auto;
    private void Awake()
    {
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Maru.performed += OnMaru;
        _gameInputs.Player.Batu.performed += OnBatu;
        _gameInputs.Player.Sikaku.performed += OnSikaku;
        _gameInputs.Player.Sankaku.performed += OnSankaku;
        _gameInputs.Player.Up.performed += OnUp;
        _gameInputs.Player.Down.performed += OnDown;
        _gameInputs.Player.Left.performed += OnLeft;
        _gameInputs.Player.Right.performed += OnRight;
        _gameInputs.Player.L1.performed += OnL1;
        _gameInputs.Player.L2.performed += OnL2;
        _gameInputs.Player.R1.performed += OnR1;
        _gameInputs.Player.R2.performed += OnR2;
        _gameInputs.Player.Lo.performed += OnLo;
        _gameInputs.Player.Ro.performed += OnRo;
        _gameInputs.Player.Share.performed += OnShare;
        _gameInputs.Player.Options.performed += OnOptions;
        _gameInputs.Player.TouchPad.performed += OnTouchPad;
        _gameInputs.Player.Y.performed += OnY;
        _gameInputs.Player.X.performed += OnX;
        _gameInputs.Player.A.performed += OnA;
        _gameInputs.Player.B.performed += OnB;
        _gameInputs.Player.Start.performed += OnStart;
        _gameInputs.Player.Select.performed += OnSelect;

        _gameInputs.Player.Enable();
    }

    void Update()
    {
        is_auto = uiope.GetComponent<PanelContoroller>().getIsAuto();
        if(is_auto)
        {
            nowpanel = AndPanelList.auto;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsChassis())
        {
            nowpanel = AndPanelList.chassis;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsConveyor())
        {
            nowpanel = AndPanelList.conveyor;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsBoxArm())
        {
            nowpanel = AndPanelList.box_arm;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsPylonArm())
        {
            nowpanel = AndPanelList.pylon_arm;
        }
    }

    private void OnDestroy()
    {
        _gameInputs?.Dispose();
    }

    private void OnMaru(InputAction.CallbackContext context){Osu(AndButtonList.Maru);}
    private void OnBatu(InputAction.CallbackContext context){Osu(AndButtonList.Batu);}
    private void OnSikaku(InputAction.CallbackContext context){Osu(AndButtonList.Sikaku);}
    private void OnSankaku(InputAction.CallbackContext context){Osu(AndButtonList.Sankaku);}
    private void OnUp(InputAction.CallbackContext context){Osu(AndButtonList.Up);}
    private void OnDown(InputAction.CallbackContext context){Osu(AndButtonList.Down);}
    private void OnLeft(InputAction.CallbackContext context){Osu(AndButtonList.Left);}
    private void OnRight(InputAction.CallbackContext context){Osu(AndButtonList.Right);}
    private void OnL1(InputAction.CallbackContext context){Osu(AndButtonList.L1);}
    private void OnL2(InputAction.CallbackContext context){Osu(AndButtonList.L2);}
    private void OnR1(InputAction.CallbackContext context){Osu(AndButtonList.R1);}
    private void OnR2(InputAction.CallbackContext context){Osu(AndButtonList.R2);}
    private void OnLo(InputAction.CallbackContext context){Osu(AndButtonList.Lo);}
    private void OnRo(InputAction.CallbackContext context){Osu(AndButtonList.Ro);}
    private void OnShare(InputAction.CallbackContext context){Osu(AndButtonList.Share);}
    private void OnOptions(InputAction.CallbackContext context){Osu(AndButtonList.Options);}
    private void OnTouchPad(InputAction.CallbackContext context){Osu(AndButtonList.TouchPad);}
    private void OnY(InputAction.CallbackContext context){Osu(AndButtonList.Y);}
    private void OnX(InputAction.CallbackContext context){Osu(AndButtonList.X);}
    private void OnA(InputAction.CallbackContext context){Osu(AndButtonList.A);}
    private void OnB(InputAction.CallbackContext context){Osu(AndButtonList.B);}
    private void OnStart(InputAction.CallbackContext context){Osu(AndButtonList.Start);}
    private void OnSelect(InputAction.CallbackContext context){Osu(AndButtonList.Select);}
    private void Osu(AndButtonList OsuButton){
        List<AndButton> result = AndData.FindAll(m => m.andbutton == OsuButton);
        for(int count = 0;count < result.Count;count++){
            if(result[count].panel == nowpanel || result[count].panel == AndPanelList.CMD){
                // Debug.Log("Clicked "+result[count].andbutton.ToString());
                result[count].button.onClick.Invoke();
            }
        }
    }
}
public enum AndButtonList
{
    Sikaku,Batu,Maru,Sankaku,L1,L2,R1,R2,Lo,Ro,Up,Down,Right,Left,Share,Options,TouchPad,X,Y,A,B,Start,Select
}
public enum AndPanelList
{   
    con,main,CMD,Null,auto,chassis,conveyor,box_arm,pylon_arm
}