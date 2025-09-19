using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class MoveBoxArm : MonoBehaviour
{
    [SerializeField] private GameObject unitySubscriberObject;
    [SerializeField] private GameObject leftBoxArmHeight;
    [SerializeField] private GameObject rightBoxArmHeight;
    [SerializeField] private GameObject leftBoxArmStrong;
    [SerializeField] private GameObject rightBoxArmStrong;
    [SerializeField] private GameObject leftBoxArmExpand;
    [SerializeField] private GameObject rightBoxArmExpand;
    [SerializeField] private float minHeight = 0f;
    [SerializeField] private float maxHeight = 200f;
    [SerializeField] private TMP_Text leftText;
    [SerializeField] private TMP_Text rightText;
    private Vector3 leftPosition;
    private Vector3 leftRotation;
    private Vector3 rightPosition;
    private Vector3 rightRotation;
    private float[] expand = new float[2];
    private float[] height = new float[2];
    private float[] strength = new float[2];
    [SerializeField] private Slider leftExpandSlider;
    [SerializeField] private Slider rightExpandSlider;
    [SerializeField] private Slider leftHeightSlider;
    [SerializeField] private Slider rightHeightSlider;
    [SerializeField] private Slider leftStrengthSlider;
    [SerializeField] private Slider rightStrengthSlider;

    private UnitySubscriber unitySubscriber;
    private Vector3 leftInitialPosition;
    private Vector3 rightInitialPosition;
    private Vector3 leftStrongInitialPosition;
    private Vector3 rightStrongInitialPosition;

    void Start()
    {
        unitySubscriber = unitySubscriberObject.GetComponent<UnitySubscriber>();
        leftInitialPosition = leftBoxArmHeight.transform.localPosition;
        rightInitialPosition = rightBoxArmHeight.transform.localPosition;
        leftStrongInitialPosition = leftBoxArmStrong.transform.localPosition;
        rightStrongInitialPosition = rightBoxArmStrong.transform.localPosition;
    }

    void Update()
    {
        expand[0] = unitySubscriber.boxArmExpand[0];
        expand[1] = unitySubscriber.boxArmExpand[1];
        height[0] = unitySubscriber.boxArmHeight[0];
        height[1] = unitySubscriber.boxArmHeight[1];
        strength[0] = unitySubscriber.boxArmPositionStrong[0];
        strength[1] = unitySubscriber.boxArmPositionStrong[1];
        leftText.SetText($"Left Arm - \n  Expand: {expand[0]}\n  Height: {height[0]}\n  Strength: {strength[0]}");
        rightText.SetText($"Right Arm - \n  Expand: {expand[1]}\n  Height: {height[1]}\n  Strength: {strength[1]}");

        float leftZ = Mathf.Lerp(90f, 0f, Mathf.InverseLerp(0f, 90f, expand[0]));
        leftBoxArmExpand.transform.localRotation = Quaternion.Euler(0f, 0f, leftZ);

        float rightZ = Mathf.Lerp(-90f, 0f, Mathf.InverseLerp(0f, 90f, expand[1]));
        rightBoxArmExpand.transform.localRotation = Quaternion.Euler(0f, 0f, rightZ);

        leftBoxArmHeight.transform.localPosition = new Vector3(
            leftInitialPosition.x,
            leftInitialPosition.y + Mathf.Clamp(2*height[0], minHeight, maxHeight),
            leftInitialPosition.z
        );
        leftBoxArmStrong.transform.localPosition = new Vector3(
            -275f - Mathf.Clamp(strength[0], 320f, 520f),
            leftStrongInitialPosition.y + Mathf.Clamp(2*height[0]*strength[0], minHeight, maxHeight),
            leftStrongInitialPosition.z
        );
        rightBoxArmHeight.transform.localPosition = new Vector3(
            rightInitialPosition.x,
            rightInitialPosition.y + Mathf.Clamp(2*height[1], minHeight, maxHeight),
            rightInitialPosition.z
        );
        rightBoxArmStrong.transform.localPosition = new Vector3(
            275f + Mathf.Clamp(strength[1], 320f, 520f),
            rightStrongInitialPosition.y + Mathf.Clamp(2*height[1]*strength[1], minHeight, maxHeight),
            rightStrongInitialPosition.z
        );
    }
}