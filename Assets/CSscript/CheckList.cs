using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckList : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite uncheckedSprite;
    [SerializeField] private Sprite checkedSprite;

    private CanvasGroup canvasGroup;

    void Start()
    {
        if (image == null) image = GetComponent<Image>();
        image.sprite = uncheckedSprite;

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.blocksRaycasts = false;

        UnityEngine.UI.Toggle toggle = GetComponent<UnityEngine.UI.Toggle>();
        if (toggle != null)
        {
            toggle.onValueChanged.AddListener(ToggleChanged);
        }
    }

    public void ToggleChanged(bool isOn)
    {
        image.sprite = isOn ? checkedSprite : uncheckedSprite;
    }
}