using UnityEngine;
using System.Collections;
using TMPro;
using UnityEngine.UI;
using Unity.Mathematics;

public class CircleGraphManager : MonoBehaviour
{
    [SerializeField] private Image circleImage;
    [SerializeField] private TMP_Text percentageText;
    [SerializeField] private bool isRotateMode = false;
    [SerializeField] private bool isVelocityMode = false;
    [SerializeField] private Color lowColor = Color.grey;
    [SerializeField] private Color highColor = Color.white;
    [System.NonSerialized] public bool isInverse = false;
    private Color normal = new Color32(0x64, 0xD6, 0x67, 0xFF);
    private Color warn = new Color32(0xFF, 0x9D, 0x00, 0xFF);
    private GameObject right;
    private GameObject left;
    private GameObject yazirusi;
    private bool isYazirusi = false;
    private string text;
    private float setPercentage = 0.0f;
    [SerializeField] private bool colorhanten = false;
    void Start()
    {
        isYazirusi = isRotateMode || isVelocityMode;
        if (isRotateMode)
        {
            right = transform.Find("Right").gameObject;
            left = transform.Find("Left").gameObject;
        }
        if (isYazirusi) yazirusi = circleImage.transform.Find("Yazirusi").gameObject;

        UpdateCircleGraph(0.0f, 0.0f);
    }
    void Update()
    {
        circleImage.fillAmount = setPercentage;
        percentageText.SetText(text);
        if (circleImage.transform.localScale.x > 0 && isInverse)
        {
            circleImage.transform.localScale = new Vector3(-1, 1, 1);
            if (isRotateMode)
            {
                right.GetComponent<Image>().color = lowColor;
                left.GetComponent<Image>().color = highColor;
            }
        }
        else if (circleImage.transform.localScale.x < 0 && !isInverse)
        {
            circleImage.transform.localScale = new Vector3(1, 1, 1);
            if (isRotateMode)
            {
                right.GetComponent<Image>().color = highColor;
                left.GetComponent<Image>().color = lowColor;
            }
        }
        if (isYazirusi) yazirusi.transform.localRotation = Quaternion.Euler(0, 0, -setPercentage * 360);

        if (colorhanten)
        {
            if (setPercentage <= 0.5f) circleImage.color = normal;
            else if (setPercentage <= 0.75f) circleImage.color = warn;
            else circleImage.color = Color.red;
            return;
        }
        else if (!isYazirusi)
        {
            if (setPercentage <= 0.25f) circleImage.color = Color.red;
            else if (setPercentage <= 0.5f) circleImage.color = warn;
            else circleImage.color = normal;
        }

    }
    public void UpdateCircleGraph(float value, float percentage)
    {
        if (isRotateMode) setPercentage = math.clamp(percentage, 0.0f, 1.0f) / 3;
        else if (isVelocityMode) setPercentage = math.clamp(percentage, 0.0f, 1.0f) * 2 / 3;
        else setPercentage = math.clamp(percentage, 0.0f, 1.0f);
        
        if (isRotateMode) text = value.ToString("F0");
        else text = value.ToString("F1");
    }
}
