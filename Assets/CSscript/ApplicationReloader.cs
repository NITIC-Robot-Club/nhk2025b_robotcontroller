using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kogane;

public class ApplicationReloader : MonoBehaviour
{
    [SerializeField] private Button batteryButton;
    private void Start() {batteryButton.onClick.AddListener(Reload);}
    private void Reload() {ApplicationRestarter.Restart();}
}
