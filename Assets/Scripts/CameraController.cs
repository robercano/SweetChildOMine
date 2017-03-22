using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour {

    public int ReferenceWidth;
    public int ReferenceHeight;
    public bool DoublSize = true;

    private int m_screenWidth;
    private int m_screenHeight;

    // Use this for initialization
    void Start () {
        updateCameraSize();
    }

    void updateCameraSize()
    {
        float ratio = Mathf.Round(Screen.height / ReferenceHeight);
        if (ratio > -float.Epsilon && ratio < float.Epsilon)
            ratio = 1.0f;

        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        if (DoublSize)
            Camera.main.orthographicSize = (float)Screen.height / ratio / 4.0f;
        else
            Camera.main.orthographicSize = (float)Screen.height / ratio / 2.0f;
    }

    void Update()
    {
        if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
        {
            updateCameraSize();
        }
    }
}
