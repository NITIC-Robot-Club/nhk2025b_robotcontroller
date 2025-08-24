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
    [SerializeField] private float calValue = 1.0f;
    private float meterValue = 0.0f;
    void Start() {
        slider = GetComponent<Slider>();
        slider.onValueChanged.AddListener(delegate { Method(); });
        Method();
    }
    
    public void Method() {
        float roundedValue = Mathf.Round(slider.value * 2) / 2;
        if (slider.value != roundedValue) {
            slider.value = roundedValue;
        }
        meterValue = roundedValue / calValue;
        sliderText.text = $"{meterValue:0.0}{unit}";
    }
}
