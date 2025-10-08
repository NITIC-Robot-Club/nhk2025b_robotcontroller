using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Collections;
using ROS2;

public class HoldButtonAction : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private InputActionReference _forwardHold;
    [SerializeField] private InputActionReference _backwardHold;
    [SerializeField] private InputActionReference _leftHold;
    [SerializeField] private InputActionReference _rightHold;
    [SerializeField] GameObject uiope;
    [SerializeField] private Button forwardButton;
    [SerializeField] private Button backwardButton;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    private bool isForwardPressed = false;
    private bool isBackwardPressed = false;
    private bool isLeftPressed = false;
    private bool isRightPressed = false;
    private IPublisher<geometry_msgs.msg.TwistStamped> joy_pub;

    private void Awake()
    {
        if (_forwardHold != null)
        {
            _forwardHold.action.started += OnForwardTap;
            _forwardHold.action.performed += OnForwardHold;
            _forwardHold.action.canceled += OffForwardHold;
            _forwardHold.action.Enable();
        }
        if (_backwardHold != null)
        {
            _backwardHold.action.started += OnBackwardTap;
            _backwardHold.action.performed += OnBackwardHold;
            _backwardHold.action.canceled += OffBackwardHold;
            _backwardHold.action.Enable();
        }
        if (_leftHold != null)
        {
            _leftHold.action.started += OnLeftTap;
            _leftHold.action.performed += OnLeftHold;
            _leftHold.action.canceled += OffLeftHold;
            _leftHold.action.Enable();
        }
        if (_rightHold != null)
        {
            _rightHold.action.started += OnRightTap;
            _rightHold.action.performed += OnRightHold;
            _rightHold.action.canceled += OffRightHold;
            _rightHold.action.Enable();
        }
    }

    // private void OnDestroy() 
    // {
    //     if (_forwardHold != null)
    //     {
    //         _forwardHold.action.performed -= OnForwardHold;
    //         _forwardHold.action.canceled -= OffForwardHold;
    //         _forwardHold.action.Disable();
    //     }
    //     if (_backwardHold != null)
    //     {
    //         _backwardHold.action.performed -= OnBackwardHold;
    //         _backwardHold.action.canceled -= OffBackwardHold;
    //         _backwardHold.action.Disable();
    //     }
    //     if (_leftHold != null)
    //     {
    //         _leftHold.action.performed -= OnLeftHold;
    //         _leftHold.action.canceled -= OffLeftHold;
    //         _leftHold.action.Disable();
    //     }
    //     if (_rightHold != null)
    //     {
    //         _rightHold.action.performed -= OnRightHold;
    //         _rightHold.action.canceled -= OffRightHold;
    //         _rightHold.action.Disable();
    //     }
    // }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.pointerPress == forwardButton.gameObject) isForwardPressed = true;
        if (eventData.pointerPress == backwardButton.gameObject) isBackwardPressed = true;
        if (eventData.pointerPress == leftButton.gameObject) isLeftPressed = true;
        if (eventData.pointerPress == rightButton.gameObject) isRightPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerPress == forwardButton.gameObject) isForwardPressed = false;
        if (eventData.pointerPress == backwardButton.gameObject) isBackwardPressed = false;
        if (eventData.pointerPress == leftButton.gameObject) isLeftPressed = false;
        if (eventData.pointerPress == rightButton.gameObject) isRightPressed = false;
    }

    private void OnForwardHold(InputAction.CallbackContext context)
    {
        isForwardPressed = true;
    }

    private void OnBackwardHold(InputAction.CallbackContext context)
    {
        isBackwardPressed = true;
    }

    private void OnLeftHold(InputAction.CallbackContext context)
    {
        isLeftPressed = true;
    }

    private void OnRightHold(InputAction.CallbackContext context)
    {
        isRightPressed = true;
    }

    private void OffForwardHold(InputAction.CallbackContext context)
    {
        isForwardPressed = false;
    }

    private void OffBackwardHold(InputAction.CallbackContext context)
    {
        isBackwardPressed = false;
    }

    private void OffLeftHold(InputAction.CallbackContext context)
    {
        isLeftPressed = false;
    }

    private void OffRightHold(InputAction.CallbackContext context)
    {
        isRightPressed = false;
    }

    private void OnForwardTap(InputAction.CallbackContext context)
    {
        forwardButton.onClick.Invoke();
    }

    private void OnBackwardTap(InputAction.CallbackContext context)
    {
        backwardButton.onClick.Invoke();
    }

    private void OnLeftTap(InputAction.CallbackContext context)
    {
        leftButton.onClick.Invoke();
    }

    private void OnRightTap(InputAction.CallbackContext context)
    {
        rightButton.onClick.Invoke();
    }

    public bool getIsForwardPressed()
    {
        return isForwardPressed;
    }

    public bool getIsBackwardPressed()
    {
        return isBackwardPressed;
    }

    public bool getIsLeftPressed()
    {
        return isLeftPressed;
    }

    public bool getIsRightPressed()
    {
        return isRightPressed;
    }
}