using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class Toggle : MonoBehaviour
{
    [SerializeField] private Image backgroundImage;
    [SerializeField] private RectTransform handle;
    [SerializeField] public bool onAwake = false;
    [NonSerialized] public bool Value;
    private float handlePosX;
    private Sequence sequence;
    private static readonly Color OFF_BG_COLOR = new Color(0.92f, 0.92f, 0.92f);
    private static readonly Color ON_BG_COLOR = new Color(0.2f, 0.84f, 0.3f);
    private static readonly Color ORIGINAL_TEXT_COLOR = new Color(0.0f, 0.0f, 1.0f);
    private static readonly Color TARGET_TEXT_COLOR = new Color(1.0f, 0.0f, 0.0f);
    private const float SWITCH_DURATION = 0.36f;
    [SerializeField] private Toggle targetToggle;
    [SerializeField] private TextMeshProUGUI displayText;
    [SerializeField] private string originalText = "/originalText";
    [SerializeField] private string targetText = "/targetText";
    
    private void Start() {
        handlePosX = Mathf.Abs(handle.anchoredPosition.x);
        Value = onAwake;
        UpdateToggle(0);
        displayText.text = originalText;
        displayText.color = ORIGINAL_TEXT_COLOR;
    }

    public void SwitchToggle() {
        Value = !Value;
        UpdateToggle(SWITCH_DURATION);
    }

    private void UpdateToggle(float duration) {
        var bgColor = Value ? ON_BG_COLOR : OFF_BG_COLOR;
        var handleDestX = Value ? handlePosX : -handlePosX;

        sequence?.Complete();
        sequence = DOTween.Sequence();
        sequence.Append(backgroundImage.DOColor(bgColor, duration))
            .Join(handle.DOAnchorPosX(handleDestX, duration / 2));
    }

    private void Update() {
        if (targetToggle.Value) {
            displayText.text = targetText;
            displayText.color = ORIGINAL_TEXT_COLOR;
        }
        else {
            displayText.text = originalText;
            displayText.color = TARGET_TEXT_COLOR;
        }
    }

    public bool GetisAwake() {
        return Value;
    }
}