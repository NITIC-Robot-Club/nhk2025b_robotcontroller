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
    [SerializeField] Button toCon;
    //[SerializeField] Button toMain;
    [SerializeField] private SpriteSwitcher spriteSwitcher;
    private bool is_mainpanel = true;
 
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
    }
    void UpdatePanelState()
    {
        if (spriteSwitcher.GetisOn())
        {
            conPanel.SetActive(true);
            mainPanel.SetActive(false);
        }
        else
        {
            conPanel.SetActive(false);
            mainPanel.SetActive(true);
        }
    }
    public bool Getismain(){
        return is_mainpanel;
    }
}
