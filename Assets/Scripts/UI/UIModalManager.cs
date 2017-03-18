using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using com.kleberswf.lib.core;

public class UIModalManager : Singleton<UIModalManager>
{
    public int ReferenceWidth;
    public int ReferenceHeight;

    public delegate void OnModalDelegate();
    public OnModalDelegate OnModalStarted;
    public OnModalDelegate OnModalFinished;

    private GameObject m_modalPanel;
    private CanvasScaler m_canvasScaler;

    private GameObject m_introPanelPrefab;
    private UIIntroPanel m_introPanel;

    private int m_screenWidth;
    private int m_screenHeight;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        m_modalPanel = transform.FindDeepChild("ModalPanel").gameObject;
        m_canvasScaler = GetComponent<CanvasScaler>();

        m_introPanelPrefab = Resources.Load("UI/UIIntroPanel") as GameObject;

        updateUISize();
    }

    void Update()
    {
        if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
        {
            updateUISize();
        }
    }

    private void updateUISize()
    {
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        int scale = Mathf.CeilToInt(m_screenHeight / (float)ReferenceHeight);

        m_canvasScaler.scaleFactor = scale;
    }

    public void OnModalExit()
    {
        m_modalPanel.SetActive(false);
        if (OnModalFinished != null)
        {
            OnModalFinished();
        }
    }

    public void EnableIntroPanel()
    {
        if (OnModalStarted != null)
        {
            OnModalStarted();
        }

        GameObject UIInstance = GameObject.Instantiate(m_introPanelPrefab, m_modalPanel.transform, false);
        Assert.IsNotNull(UIInstance);
        
        m_introPanel = UIInstance.GetComponent<UIIntroPanel>();
        m_introPanel.OnExit += OnModalExit;

        m_introPanel.gameObject.SetActive(true);
    }
}
