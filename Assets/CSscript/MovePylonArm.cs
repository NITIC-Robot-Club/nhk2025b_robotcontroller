using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections;
using TMPro;

public class MovePylonArm : MonoBehaviour
{
    [SerializeField] private GameObject unitySubscriberObject;
    [SerializeField] private GameObject leftPylonArm;
    [SerializeField] private GameObject rightPylonArm;
    [SerializeField] private float minHeight = 0f;
    [SerializeField] private float maxHeight = 200f;
    [SerializeField] private TMP_Text leftArmText;
    [SerializeField] private TMP_Text rightArmText;
    [SerializeField] private TMP_Text expandText;
    private Vector3 leftPosition;
    private Vector3 leftRotation;
    private Vector3 rightPosition;
    private Vector3 rightRotation;
    private float[] expand = new float[2];
    private float[] height = new float[2];
    private float[] strength = new float[2];

    private UnitySubscriber unitySubscriber;
    private Vector3 leftInitialPosition;
    private Vector3 rightInitialPosition;

    void Start()
    {
        unitySubscriber = unitySubscriberObject.GetComponent<UnitySubscriber>();
        leftInitialPosition = leftPylonArm.transform.localPosition;
        rightInitialPosition = rightPylonArm.transform.localPosition;
    }

    void Update()
    {
        expand[0] = unitySubscriber.pylonArmExpand[0];
        expand[1] = unitySubscriber.pylonArmExpand[1];
        height[0] = unitySubscriber.pylonArmHeight[0];
        height[1] = unitySubscriber.pylonArmHeight[1];
        leftArmText.SetText($"Left Arm - \n  Height: {height[0]}\n  Strong: {strength[0]}");
        rightArmText.SetText($"Right Arm - \n  Height: {height[1]}\n  Strong: {strength[1]}");
        expandText.SetText($"Expand - Left: {expand[0]}, Right: {expand[1]}");

        float leftAngle = Mathf.Clamp(expand[0], 0f, 180f);
        float rightAngle = -Mathf.Clamp(expand[1], 0f, 180f);

        leftPylonArm.transform.localPosition = new Vector3(
            leftInitialPosition.x,
            leftInitialPosition.y + Mathf.Clamp(2*height[0], minHeight, maxHeight),
            leftInitialPosition.z
        );

        rightPylonArm.transform.localPosition = new Vector3(
            rightInitialPosition.x,
            rightInitialPosition.y + Mathf.Clamp(2*height[1], minHeight, maxHeight),
            rightInitialPosition.z
        );

        leftPylonArm.transform.localRotation = Quaternion.Euler(leftAngle, 90f, -180f);
        rightPylonArm.transform.localRotation = Quaternion.Euler(-rightAngle, -90f, -180f);
    }
}