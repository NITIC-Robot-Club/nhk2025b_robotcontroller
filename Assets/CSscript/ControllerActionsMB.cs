using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
[System.Serializable] // <- これが大事。忘れずに
public class andButton {
    public andButtonList ds4button;
    public Button button;
    public andPanelList panel;
}
public class ControllerActionsMB : MonoBehaviour
{
    [SerializeField] GameObject uiope;
    [SerializeField] List<andButton> data;
    private GameInputs _gameInputs;
    private andPanelList nowpanel;
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
            nowpanel = andPanelList.auto;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsChassis())
        {
            nowpanel = andPanelList.chassis;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsConveyor())
        {
            nowpanel = andPanelList.conveyor;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsBoxArm())
        {
            nowpanel = andPanelList.box_arm;
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsPylonArm())
        {
            nowpanel = andPanelList.pylon_arm;
        }
    }

    private void OnDestroy()
    {
        _gameInputs?.Dispose();
    }

    private void OnMaru(InputAction.CallbackContext context){Osu(andButtonList.Maru);}
    private void OnBatu(InputAction.CallbackContext context){Osu(andButtonList.Batu);}
    private void OnSikaku(InputAction.CallbackContext context){Osu(andButtonList.Sikaku);}
    private void OnSankaku(InputAction.CallbackContext context){Osu(andButtonList.Sankaku);}
    private void OnUp(InputAction.CallbackContext context){Osu(andButtonList.Up);}
    private void OnDown(InputAction.CallbackContext context){Osu(andButtonList.Down);}
    private void OnLeft(InputAction.CallbackContext context){Osu(andButtonList.Left);}
    private void OnRight(InputAction.CallbackContext context){Osu(andButtonList.Right);}
    private void OnL1(InputAction.CallbackContext context){Osu(andButtonList.L1);}
    private void OnL2(InputAction.CallbackContext context){Osu(andButtonList.L2);}
    private void OnR1(InputAction.CallbackContext context){Osu(andButtonList.R1);}
    private void OnR2(InputAction.CallbackContext context){Osu(andButtonList.R2);}
    private void OnLo(InputAction.CallbackContext context){Osu(andButtonList.Lo);}
    private void OnRo(InputAction.CallbackContext context){Osu(andButtonList.Ro);}
    private void OnShare(InputAction.CallbackContext context){Osu(andButtonList.Share);}
    private void OnOptions(InputAction.CallbackContext context){Osu(andButtonList.Options);}
    private void OnTouchPad(InputAction.CallbackContext context){Osu(andButtonList.TouchPad);}
    private void OnY(InputAction.CallbackContext context){Osu(andButtonList.Y);}
    private void OnX(InputAction.CallbackContext context){Osu(andButtonList.X);}
    private void OnA(InputAction.CallbackContext context){Osu(andButtonList.A);}
    private void OnB(InputAction.CallbackContext context){Osu(andButtonList.B);}
    private void OnStart(InputAction.CallbackContext context){Osu(andButtonList.Start);}
    private void OnSelect(InputAction.CallbackContext context){Osu(andButtonList.Select);}
    private void Osu(andButtonList OsuButton){
        List<andButton> result = data.FindAll(m => m.ds4button == OsuButton);
        for(int count = 0;count < result.Count;count++){
            if(result[count].panel == nowpanel || result[count].panel == andPanelList.CMD){
                // Debug.Log("Clicked "+result[count].ds4button.ToString());
                result[count].button.onClick.Invoke();
            }
        }
    }
}
public enum andButtonList
{
    Sikaku,Batu,Maru,Sankaku,L1,L2,R1,R2,Lo,Ro,Up,Down,Right,Left,Share,Options,TouchPad,X,Y,A,B,Start,Select
}
public enum andPanelList
{   
    con,main,CMD,Null,auto,chassis,conveyor,box_arm,pylon_arm
}