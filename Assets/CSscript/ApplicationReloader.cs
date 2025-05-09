using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Kogane;

public class ApplicationReloader : MonoBehaviour
{
    [SerializeField] private Button reloadButton;
    private void Start() {reloadButton.onClick.AddListener(Reload);}
    private void Reload() {ApplicationRestarter.Restart();}
}
