using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour {

    public bool m_doubleSize = true;
    private float m_ratio;

    private int m_backgroundHeight;
    private int m_screenWidth;
    private int m_screenHeight;

    // Use this for initialization
    void Start () {
		GameObject sceneObject = GameObject.FindWithTag ("Background");
		Assert.IsNotNull (sceneObject);

		Sprite sceneBackground = sceneObject.GetComponent<SpriteRenderer> ().sprite;
		Assert.IsNotNull (sceneBackground);

        /* Adjust orthographic size of the camera so each pixel on the background sprite
		 * is render to an integer number of pixels at the current screen resolution,
		 * thus maintaining the pixel perfect feeling */
        m_backgroundHeight = (int)(sceneBackground.bounds.extents.y * 2.0f);

        updateCameraSize();
    }

    void updateCameraSize()
    {
        m_ratio = Mathf.Round(Screen.height / m_backgroundHeight);
        if (m_ratio > -float.Epsilon && m_ratio < float.Epsilon)
            m_ratio = 1.0f;

        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        if (m_doubleSize)
            Camera.main.orthographicSize = (float)Screen.height / m_ratio / 4.0f;
        else
            Camera.main.orthographicSize = (float)Screen.height / m_ratio / 2.0f;
    }

    void Update()
    {
        if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
        {
            updateCameraSize();
        }
    }
}
