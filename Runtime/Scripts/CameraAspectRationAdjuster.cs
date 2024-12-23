using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAspectRationAdjuster : MonoBehaviour
{
    public float defaultHeight = 5f;
    public Vector2 defaultAspect = new Vector2(16, 9);

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
        AdjustCameraSize();
    }

    void Update()
    {
        AdjustCameraSize();
    }

    void AdjustCameraSize()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;

        float targetAspect = defaultAspect.x / defaultAspect.y;

        if (screenAspect >= targetAspect)
        {
            cam.orthographicSize = defaultHeight;
        }
        else
        {
            float differenceInSize = targetAspect / screenAspect;
            cam.orthographicSize = defaultHeight * differenceInSize;
        }
    }
}
