using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class ActionContextMenu : MonoBehaviour {

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
    public int MaxNumItems;
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
    public delegate void OnActionDelegate(int selectedAmount);
    public OnActionDelegate OnAction;

    int SelectedNumItems
    {
        get
        {
            return int.Parse(m_numItems.text);
        }
        set
        {
            m_numItems.text = value.ToString();
        }
    }
    string Info
    {
        get
        {
            return m_info.text;
        }
        set
        {
            m_info.text = value;
        }
    }

    private Text m_title;
    private Text m_numItems;
    private Text m_info;
    private Text m_action;
    private WidgetFader m_widgetFader;
    
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_numItems = transform.FindDeepChild("NumItems").GetComponent<Text>();
        m_info = transform.FindDeepChild("Info").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    void UpdateInfo()
    {
        Info = "(" + (MaxNumItems - SelectedNumItems) + " left)";
    }

      /* Public interface */
    public void Enable()
    {
        SelectedNumItems = 0;
        UpdateInfo();
        m_widgetFader.Enable();
    }
    public void Disable()
    {
        m_widgetFader.Disable();
    }

    /* Events */
    public void OnLeftClick()
    {
        if (SelectedNumItems > 0)
        {
            SelectedNumItems--;
            UpdateInfo();
        }
    }
    public void OnRightClick()
    {
        if(SelectedNumItems < MaxNumItems)
        {
            SelectedNumItems++;
            UpdateInfo();
        }
    }
    public void OnActionClick()
    {
        OnAction(SelectedNumItems);
    }
}
