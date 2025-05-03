using UnityEngine;
using UnityEngine.UI;

public class SpriteSwitcher : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private IOSButton toggle;

    void Start() {
        UpdateSprite(toggle.GetisOn());
        toggle.button.onClick.AddListener(() => UpdateSprite(toggle.GetisOn()));
    }

    private void UpdateSprite(bool isOn) {
        image.sprite = isOn ? sprite2 : sprite1;
    }
}