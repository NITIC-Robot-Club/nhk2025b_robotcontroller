using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Image buttonImage;
    [SerializeField] private Button button;
    private bool isOn = true;

    void Start()
    {
        button.onClick.AddListener(UpdateSprite);
    }

    private void UpdateSprite()
    {
        isOn = !isOn;
        buttonImage.sprite = isOn ? sprite2 : sprite1;
    }

    public bool GetisOn()
    {
        return isOn;
    }
}