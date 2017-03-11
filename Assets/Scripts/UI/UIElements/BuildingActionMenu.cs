using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public sealed class BuildingActionMenu : UIElement {

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

    public delegate int OnRetrieveDelegate();
    public OnRetrieveDelegate OnRetrieveWorkLeft;

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
    private Text m_info;
    private Text m_action;
    private Button m_buttonAction;
    private WidgetFader m_widgetFader;
    
    override protected void Awake()
    {
		base.Awake ();

        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_info = transform.FindDeepChild("Info").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();

        m_buttonAction = transform.FindDeepChild("Action").GetComponent<Button>();
        m_buttonAction.interactable = false;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    override protected void Update()
    {
		base.Update ();

        Info = "(" + OnRetrieveWorkLeft() + "% left)";

        m_buttonAction.interactable = (OnAction != null);
    }
    
    /* Public interface */
    public void Enable()
    {
        m_widgetFader.Enable();
    }
    public void Disable()
    {
        m_widgetFader.Disable();
    }

    /* Events */
   public void OnActionClick()
    {
        OnAction();
    }
}
