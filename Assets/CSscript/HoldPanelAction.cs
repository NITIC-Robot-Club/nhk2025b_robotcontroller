using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;

public class HoldPanelAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Button targetButton;
    [SerializeField] private GameObject targetPanel;
    [SerializeField] private InputActionReference _hold;
    [SerializeField] GameObject uiope;
    [SerializeField] private Sprite sprite1;
    [SerializeField] private Sprite sprite2;
    [SerializeField] private Image buttonImage;
    private bool isHold = false;
    private bool isAuto;

    private void Awake()
    {
        targetPanel.SetActive(false);
        if (_hold == null) return;
        _hold.action.performed += OnHold;
        _hold.action.canceled += OffHold;
        _hold.action.Enable();
    }

    private void OnDestroy() 
    {
        if (_hold == null) return;
        _hold.action.performed -= OnHold;
        _hold.action.canceled -= OffHold;
        _hold.action.Disable();
    }

    private void Update()
    {
        isAuto = uiope.GetComponent<PanelContoroller>().getIsAuto();
    }

    private void OnHold(InputAction.CallbackContext context)
    {
        isHold = true;
        targetPanel.SetActive(true);
        buttonImage.sprite = sprite2;
    }

    private void OffHold(InputAction.CallbackContext context)
    {
        isHold = false;
        targetPanel.SetActive(false);
        buttonImage.sprite = sprite1;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isHold = true;
        targetPanel.SetActive(true);
        buttonImage.sprite = sprite2;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isHold = false;
        targetPanel.SetActive(false);
        buttonImage.sprite = sprite1;
    }

    public bool getIsHold()
    {
        return isHold;
    }
}