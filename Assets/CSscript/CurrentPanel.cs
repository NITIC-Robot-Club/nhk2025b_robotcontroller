using UnityEngine;
using TMPro;

public class CurrentPanel : MonoBehaviour
{
    [SerializeField] private TMP_Text viewerText;
    [SerializeField] GameObject uiope;

    private void Update()
    {
        if(uiope.GetComponent<PanelContoroller>().getIsAuto())
        {
            viewerText.gameObject.SetActive(false);
        }
        else
        {
            viewerText.gameObject.SetActive(true);
        }
        if(uiope.GetComponent<PanelContoroller>().getIsChassis())
        {
            viewerText.text = "Chassis";
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsConveyor())
        {
            viewerText.text = "Conveyor";
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsBoxArm())
        {
            viewerText.text = "Box Arm";
        }
        else if(uiope.GetComponent<PanelContoroller>().getIsPylonArm())
        {
            viewerText.text = "Pylon Arm";
        }
    }
}