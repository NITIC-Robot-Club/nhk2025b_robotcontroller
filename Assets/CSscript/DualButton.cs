using UnityEngine;
using UnityEngine.UI;

public class DualButton : MonoBehaviour
{
    [SerializeField] private Button originButton;
    [SerializeField] private Button targetButton;

    void Start() 
    {
        originButton.gameObject.SetActive(true);
        targetButton.gameObject.SetActive(false);
        originButton.onClick.AddListener(OnOriginButtonClick);
        targetButton.onClick.AddListener(OnTargetButtonClick);
    }

    private void OnOriginButtonClick() 
    {
        originButton.gameObject.SetActive(false);
        targetButton.gameObject.SetActive(true);
    }

    private void OnTargetButtonClick() 
    {
        targetButton.gameObject.SetActive(false);
        originButton.gameObject.SetActive(true);
    }
}