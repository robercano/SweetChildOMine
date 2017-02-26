using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public class MiningContextMenu : MonoBehaviour {

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
    public delegate void OnActionDelegate(int selectedAmount);
    public OnActionDelegate OnAction;

    public delegate int OnRetrieveDelegate();
    public OnRetrieveDelegate OnRetrieveMaxItems;
    public OnRetrieveDelegate OnRetrieveCurrentItems;

    public int SelectedNumItems
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
    private Button m_buttonLeft;
    private Button m_buttonRight;
    private Button m_buttonAction;
    private WidgetFader m_widgetFader;
    
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_numItems = transform.FindDeepChild("NumItems").GetComponent<Text>();
        m_info = transform.FindDeepChild("Info").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();

        m_buttonLeft = transform.FindDeepChild("ButtonLeft").GetComponent<Button>();
        m_buttonRight = transform.FindDeepChild("ButtonRight").GetComponent<Button>();
        m_buttonAction = transform.FindDeepChild("Action").GetComponent<Button>();

        m_buttonLeft.interactable = false;
        m_buttonRight.interactable = false;
        m_buttonAction.interactable = false;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();

        SelectedNumItems = 1;
    }

    void Update()
    {
        Info = "(" + (OnRetrieveCurrentItems() - SelectedNumItems) + " left)";

        if (OnAction == null)
        {
            m_buttonLeft.interactable = false;
            m_buttonRight.interactable = false;
            m_buttonAction.interactable = false;
        }
        else
        {
            m_buttonAction.interactable = true;

            if (SelectedNumItems <= 1)
            {
                SelectedNumItems = 1;
                m_buttonLeft.interactable = false;
                m_buttonAction.interactable = false;
            }
            else
            {
                m_buttonLeft.interactable = true;
                m_buttonAction.interactable = true;
            }

            if (SelectedNumItems >= OnRetrieveMaxItems())
            {
                SelectedNumItems = OnRetrieveMaxItems();
                m_buttonRight.interactable = false;
            }
            else
            {
                m_buttonRight.interactable = true;
            }
        }
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
    public void OnLeftClick()
    {
        SelectedNumItems--;
    }
    public void OnRightClick()
    {
        SelectedNumItems++;
    }
    public void OnActionClick()
    {
        OnAction(SelectedNumItems);
    }
}
