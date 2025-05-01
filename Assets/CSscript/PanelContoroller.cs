using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PanelContoroller : MonoBehaviour
{
    [SerializeField] GameObject conPanel;      //メインカメラ格納用
    [SerializeField] GameObject mainPanel;
    [SerializeField] Button toCon;
    [SerializeField] Button toMain;
    [SerializeField] private GameObject cmdpanel;
    private readonly Vector2 _on = new(7f, -1.3f);
	private readonly Vector2 _off = new(20f, -1.3f);

    private bool is_mainpanel = true;

 
    void Start () {
        toCon.onClick.AddListener(conOn);
        toMain.onClick.AddListener(mainOn);
        mainOn();
    }
    void Update()
    {
        Transform myt = cmdpanel.transform;
        if(is_mainpanel){
            myt.position = _on;
        }else{
            myt.position = _off;
        }
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
    }
    public bool Getismain(){
        return is_mainpanel;
    }
}
