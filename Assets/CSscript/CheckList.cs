using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckList : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite uncheckedSprite;
    [SerializeField] private Sprite checkedSprite;
    void Start()
    {
        image.sprite = uncheckedSprite;
        gameObject.GetComponent<UnityEngine.UI.Toggle>().onValueChanged.AddListener(ToggleChanged);
    }
    public void ToggleChanged(bool isOn)
    {
        if (isOn)
        {
            image.sprite = checkedSprite;
        }
        else
        {
            image.sprite = uncheckedSprite;
        }
    }
}