using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

public sealed class MiningActionMenu : UIElement {

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

    public float OnButtonDownStartRepeatInterval = 1.0f;
    public float OnButtonDownContinueRepeatInterval = 0.2f;

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
		
    private Text m_numItems;
    private Text m_info;
    private Text m_action;
    private Button m_buttonLeft;
    private Button m_buttonRight;
    private Button m_buttonAction;
    private WidgetFader m_widgetFader;

    private bool m_isLeftButtonPressed;
    private bool m_isRightButtonPressed;
    private bool m_isFirstClick;
    private float m_previousClickTime;

    UIContainer m_materialInventory;

    override protected void Awake()
    {
		base.Awake ();

        m_numItems = transform.FindDeepChild("NumItems").GetComponent<Text>();
        m_info = transform.FindDeepChild("Info").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();

        m_buttonLeft = transform.FindDeepChild("ButtonLeft").GetComponent<Button>();
        m_buttonRight = transform.FindDeepChild("ButtonRight").GetComponent<Button>();
        m_buttonAction = transform.FindDeepChild("Action").GetComponent<Button>();

        m_materialInventory = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();

        m_buttonLeft.interactable = false;
        m_buttonRight.interactable = false;
        m_buttonAction.interactable = false;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();

        m_isLeftButtonPressed = false;
        m_isRightButtonPressed = false;
        m_isFirstClick = true;
        m_previousClickTime = 0.0f;

        SelectedNumItems = 1;
    }

    override protected void Update()
    {
		base.Update ();

        Info = "(" + (OnRetrieveCurrentItems() - SelectedNumItems) + " left)";

        if (m_isLeftButtonPressed || m_isRightButtonPressed)
        {
            if (((m_isFirstClick == true) && ((Time.time - m_previousClickTime) > OnButtonDownStartRepeatInterval)) ||
                ((m_isFirstClick == false) && ((Time.time - m_previousClickTime) > OnButtonDownContinueRepeatInterval)))
            {
                if (m_isLeftButtonPressed)
                {
                    SelectedNumItems--;
                }
                else if (m_isRightButtonPressed)
                {
                    SelectedNumItems++;
                }
                m_isFirstClick = false;
                m_previousClickTime = Time.time;
            }
        }
        else
        {
            m_isFirstClick = true;
            m_previousClickTime = Time.time;
        }

        if (OnAction == null)
        {
            m_buttonLeft.interactable = false;
            m_buttonRight.interactable = false;
            m_buttonAction.interactable = false;
        }
        else
        {
            m_buttonAction.interactable = true;

            if (SelectedNumItems <= 0)
            {
                SelectedNumItems = 0;
                m_buttonLeft.interactable = false;
                m_buttonAction.interactable = false;
            }
            else
            {
                m_buttonLeft.interactable = true;
                m_buttonAction.interactable = true;
            }

            if (SelectedNumItems > OnRetrieveMaxItems())
            {
                /* Signal the error in the inventory */
                m_materialInventory.SignalError();

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
    public void OnLeftClickDown()
    {
        SelectedNumItems--;
        m_isLeftButtonPressed = true;
    }
    public void OnLeftClickUp()
    {
        m_isLeftButtonPressed = false;
    }
    public void OnRightClickDown()
    {
        SelectedNumItems++;
        m_isRightButtonPressed = true;
    }
    public void OnRightClickUp()
    {
        m_isRightButtonPressed = false;
    }
    public void OnActionClick()
    {
        OnAction(SelectedNumItems);
    }
}
