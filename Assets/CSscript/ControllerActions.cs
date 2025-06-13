using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
[System.Serializable] // <- これが大事。忘れずに
public class NewDs4button {
    public ButtonList ds4button;
    public Button button;
    public PanelList panel;
}
public class ControllerActions : MonoBehaviour
{
    [SerializeField] GameObject uiope;
    [SerializeField] List<NewDs4button> ds4data;
    private GameInputs _gameInputs;
    private PanelList nowpanel;
    private bool is_main;
    private Vector2 _leftdsjoy;
    private Vector2 _rightdsjoy;
    private float _r2float;
    private bool connect;
    public FixedJoystick Rightjoy;
    public FixedJoystick Leftjoy;
    private void Awake(){
        StartCoroutine(connectcheck());
        _gameInputs = new GameInputs();

        // Actionイベント登録
        _gameInputs.Player.Move.started += OnMove;
        _gameInputs.Player.Move.performed += OnMove;
        _gameInputs.Player.Move.canceled += OnMove;
        _gameInputs.Player.Dir.started += OnDir;
        _gameInputs.Player.Dir.performed += OnDir;
        _gameInputs.Player.Dir.canceled += OnDir;
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
    private void OnMove(InputAction.CallbackContext context){
        _leftdsjoy = context.ReadValue<Vector2>();
    }
    private void OnDir(InputAction.CallbackContext context){
        _rightdsjoy = context.ReadValue<Vector2>();
    }
    void Update(){
        is_main = uiope.GetComponent<PanelContoroller>().Getismain();
        if(is_main){
            nowpanel = PanelList.main;
        }else{
            nowpanel = PanelList.con;
        }
        if(connect){
            Leftjoy.Setpos(_leftdsjoy);
            Rightjoy.Setpos(_rightdsjoy);
        }
    }

    private void OnDestroy(){
        _gameInputs?.Dispose();
    }
    private void OnMaru(InputAction.CallbackContext context){Osu(ButtonList.Maru);}
    private void OnBatu(InputAction.CallbackContext context){Osu(ButtonList.Batu);}
    private void OnSikaku(InputAction.CallbackContext context){Osu(ButtonList.Sikaku);}
    private void OnSankaku(InputAction.CallbackContext context){Osu(ButtonList.Sankaku);}
    private void OnUp(InputAction.CallbackContext context){Osu(ButtonList.Up);}
    private void OnDown(InputAction.CallbackContext context){Osu(ButtonList.Down);}
    private void OnLeft(InputAction.CallbackContext context){Osu(ButtonList.Left);}
    private void OnRight(InputAction.CallbackContext context){Osu(ButtonList.Right);}
    private void OnL1(InputAction.CallbackContext context){Osu(ButtonList.L1);}
    private void OnL2(InputAction.CallbackContext context){Osu(ButtonList.L2);}
    private void OnR1(InputAction.CallbackContext context){Osu(ButtonList.R1);}
    private void OnR2(InputAction.CallbackContext context){Osu(ButtonList.R2);}
    private void OnLo(InputAction.CallbackContext context){Osu(ButtonList.Lo);}
    private void OnRo(InputAction.CallbackContext context){Osu(ButtonList.Ro);}
    private void OnShare(InputAction.CallbackContext context){Osu(ButtonList.Share);}
    private void OnOptions(InputAction.CallbackContext context){Osu(ButtonList.Options);}
    private void OnTouchPad(InputAction.CallbackContext context){Osu(ButtonList.TouchPad);}
    private void OnY(InputAction.CallbackContext context){Osu(ButtonList.Y);}
    private void OnX(InputAction.CallbackContext context){Osu(ButtonList.X);}
    private void OnA(InputAction.CallbackContext context){Osu(ButtonList.A);}
    private void OnB(InputAction.CallbackContext context){Osu(ButtonList.B);}
    private void OnStart(InputAction.CallbackContext context){Osu(ButtonList.Start);}
    private void OnSelect(InputAction.CallbackContext context){Osu(ButtonList.Select);}
    private void Osu(ButtonList OsuButton){
        List<NewDs4button> result = ds4data.FindAll(m => m.ds4button == OsuButton);
        for(int count = 0;count < result.Count;count++){
            if(result[count].panel == nowpanel || result[count].panel == PanelList.CMD){
                Debug.Log("New:"+result[count].ds4button.ToString());
                result[count].button.onClick.Invoke();
            }
        }
    }
    public Vector2 Getleftjoy(){
        return _leftdsjoy;
    }
    public Vector2 Getrightjoy(){
        return _rightdsjoy;
    }
    public float Getr2float(){
        return _r2float;
    }
    IEnumerator connectcheck(){
        bool okuru = false;
        while(true){    
            yield return new WaitForFixedUpdate();
            var game = Gamepad.all;
            if(game.Count==0){
                okuru = false;
                connect =false;
                _leftdsjoy = new Vector2(0,0);
                _rightdsjoy = new Vector2(0,0);
            }else{
                connect = true;
                if(!okuru){
                    Debug.Log("connected!");
                    okuru=true;
                }
            }
        }
    }
}
public enum ButtonList
{
    Sikaku,Batu,Maru,Sankaku,L1,L2,R1,R2,Lo,Ro,Up,Down,Right,Left,Share,Options,TouchPad,X,Y,A,B,Start,Select
}
public enum PanelList
{
    con,main,CMD,Null
}