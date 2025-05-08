using UnityEngine;

public class SideContCam : MonoBehaviour
{
    public Camera cam;
    private float width = 2436f;
    private float height = 1125f;
    private float pixelPerUnit = 100f;

    void Awake()
    {
        
        cam.orthographicSize = (height / 2f / pixelPerUnit);

        float aspectScr = (float)Screen.height / (float)Screen.width;
        float aspectImg = height / width;

        if (aspectImg > aspectScr)
        {
            float ratioHeight = height / Screen.height;
            float ratioWidth = width / (Screen.width * ratioHeight);
            cam.rect = new Rect((1f - ratioWidth) / 2f, 0f, ratioWidth, 1f);
        }
        else
        {
            float ratioWidth = width / Screen.width;
            float ratioHeight = height / (Screen.height * ratioWidth);
            cam.rect = new Rect(0f, (1f - ratioHeight) / 2f, 1f, ratioHeight);
        }
    }
}