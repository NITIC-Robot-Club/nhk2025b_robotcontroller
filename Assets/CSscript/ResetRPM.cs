using UnityEngine;
using UnityEngine.UI;

public class ResetRPM : MonoBehaviour
{
    [SerializeField] private Button resetButton;
    [SerializeField] private float resetVal = 0f;
    [SerializeField] private Slider targetSlider;

    public void OnClick()
    {
        targetSlider.value = resetVal;
    }
}