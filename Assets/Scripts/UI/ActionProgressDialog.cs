using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ActionProgressDialog : MonoBehaviour {

    public string Title
    {
        get
        {
            return m_title.text;
        }
        set
        {
            m_title.text = value;
        }
    }
    public string ActionName
    {
        get
        {
            return m_action.text;
        }
        set
        {
            m_action.text = value;
        }
    }
    public delegate void OnActionDelegate();
    public OnActionDelegate OnAction;

    public int Percentage
    {
        get
        {
            return int.Parse(m_percentage.text);
        }
        set
        {
            SetProgressBar(value);
            m_percentage.text = value.ToString() + "%";
        }
    }

    private Text m_title;
    private Text m_percentage;
    private Text m_action;

    private Image m_progressBarForeground;
    private Image m_progressBarBackground;
    private RectTransform m_progressBarRect;
    private float m_progressBarRatio;

    private WidgetFader m_widgetFader;
    
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_percentage = transform.FindDeepChild("Percentage").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();
        m_progressBarForeground = transform.FindDeepChild("ProgressForeground").GetComponent<Image>();
        m_progressBarBackground = transform.FindDeepChild("ProgressBackground").GetComponent<Image>();

        m_progressBarRect = m_progressBarForeground.rectTransform;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    void Update()
    {
        // Compensate mirroring when character is facing left
        transform.localScale = transform.parent.localScale;
    }

    /* Public interface */
    public void Enable()
    {
        Percentage = 0;

        m_widgetFader.Enable();
    }

    public void Disable()
    {
        m_widgetFader.Disable();
    }

    public void OnActionClick()
    {
        OnAction();
    }

    void SetProgressBar(int percentage)
    {
        percentage = Mathf.Clamp(percentage, 0, 100);

        m_progressBarRect.sizeDelta = new Vector2(percentage * m_progressBarBackground.rectTransform.sizeDelta.x / 100.0f, m_progressBarRect.sizeDelta.y);
    }
    void ClearProgressBar()
    {
        m_progressBarRect.sizeDelta = new Vector2(0.0f, m_progressBarRect.sizeDelta.y);
    }
}
