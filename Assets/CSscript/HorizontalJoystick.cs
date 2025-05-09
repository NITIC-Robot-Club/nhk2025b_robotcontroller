using UnityEngine;

public class LockHorizontalJoystick : MonoBehaviour
{
    public RectTransform handle;
    void Update()
    {
        if (handle != null) handle.anchoredPosition = new Vector2(handle.anchoredPosition.x, 0f);
    }
}
