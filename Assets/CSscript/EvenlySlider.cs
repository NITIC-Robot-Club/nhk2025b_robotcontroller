using UnityEngine;
using UnityEngine.UI;

public class EvenlySlider : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private float minValue = 1f;

    public void OnValueChanged()
    {
        float newValue = Mathf.Round(slider.value / minValue) * minValue;
        if (slider.value != newValue)
        {
            slider.value = newValue;
        }
    }
}
