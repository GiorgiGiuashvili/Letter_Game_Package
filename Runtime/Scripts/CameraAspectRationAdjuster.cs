using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraAspectRatioAdjuster : MonoBehaviour
{
    public float defaultHeight = 5f;
    public Vector2 defaultAspect = new Vector2(16, 9);
    public Canvas canvas;
    public RectTransform[] uiElements;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCameraSize();
        AdjustUIElements();
    }

    void Update()
    {
        AdjustCameraSize();
        AdjustUIElements();
    }

    void AdjustCameraSize()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = defaultAspect.x / defaultAspect.y;

        if (screenAspect > targetAspect)
        {
            float differenceInSize = screenAspect / targetAspect;
            cam.orthographicSize = defaultHeight / differenceInSize;
        }
        else if (screenAspect < targetAspect)
        {
            float differenceInSize = targetAspect / screenAspect;
            cam.orthographicSize = defaultHeight * differenceInSize;
        }
        else
        {
            cam.orthographicSize = defaultHeight;
        }
    }

    void AdjustUIElements()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = defaultAspect.x / defaultAspect.y;
        float aspectRatioScale = screenAspect / targetAspect;

        foreach (var element in uiElements)
        {
            Vector2 newSize = element.sizeDelta;
            newSize.x *= aspectRatioScale;
            element.sizeDelta = newSize;
        }
    }
}
