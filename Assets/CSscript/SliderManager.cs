using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderManager : MonoBehaviour
{
    private Slider slider;
    [SerializeField] private TextMeshProUGUI sliderText;
    [SerializeField] private string unit = "/unit_name";
    [SerializeField] private bool isMethodOn = false;
    [SerializeField, Range(0, 5)] private int decimalDigits = 2;

    void Start() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { Method(); });
        Method();
    }
    
    public void Method() {
        string format = "F" + decimalDigits.ToString();
        sliderText.text = $"{slider.value.ToString(format)}{unit}";
    }
}
