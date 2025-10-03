using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValueText : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string unit = "";

    private void Start()
    {
        UpdateValueText();
    }

    private void Update()
    {
        UpdateValueText();
    }

    private void UpdateValueText()
    {
        valueText.text = slider.value.ToString("0") + unit;
    }
}