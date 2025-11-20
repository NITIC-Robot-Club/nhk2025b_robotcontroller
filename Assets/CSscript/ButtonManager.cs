using UnityEngine;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button automateReadyButton;
    [SerializeField] private UnitySubscriber unitySubscriber;

    void Start()
    {
        if (automateReadyButton != null)
        {
            automateReadyButton.onClick.AddListener(SafePanelTransition);
        }
        else
        {
            Debug.LogError("[ButtonManager] automateReadyButton is not assigned in Inspector!");
        }
    }

    private void SafePanelTransition()
    {
        if (unitySubscriber == null)
        {
            Debug.LogError("[ButtonManager] UnitySubscriber reference is missing!");
            return;
        }
        StartCoroutine(InvokePanelTransitionSafely());
    }

    private System.Collections.IEnumerator InvokePanelTransitionSafely()
    {
        yield return null;
        try
        {
            unitySubscriber.panelTransition();
            Debug.Log("[ButtonManager] panelTransition() called successfully.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"[ButtonManager] Error calling panelTransition(): {ex.Message}");
        }
    }
}
