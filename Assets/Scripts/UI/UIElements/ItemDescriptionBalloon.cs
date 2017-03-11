using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class ItemDescriptionBalloon : UIElement
{

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
    public string Description
    {
        get
        {
            return m_description.text;
        }
        set
        {
            m_description.text = value;
        }
    }

    private Text m_title;
    private Text m_description;
    private WidgetFader m_widgetFader;

    // Use this for initialization
    override protected void Awake()
    {
        base.Awake();

        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_description = transform.FindDeepChild("Description").GetComponent<Text>();

        Title = "";
        Description = "";

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    public void Enable()
    {
        m_widgetFader.Enable();
    }

    public void Disable()
    {
        m_widgetFader.Disable();
    }
    public void DisableImmediate()
    {
        m_widgetFader.DisableImmediate();
    }
}