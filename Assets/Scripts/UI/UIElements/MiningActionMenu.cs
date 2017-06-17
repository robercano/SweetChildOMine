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
	public string MaterialName
	{
		get
		{
			return m_materialActionText.text;
		}
		set
		{
			m_materialActionText.text = value;
		}
	}
    public delegate void OnActionDelegate(int selectedAmount);
    public OnActionDelegate OnAction;

    public delegate int OnRetrieveDelegate();
    public OnRetrieveDelegate OnRetrieveMaxItems;
    public OnRetrieveDelegate OnRetrieveCurrentItems;

    public float OnButtonDownStartRepeatInterval = 1.0f;
    public float OnButtonDownContinueRepeatInterval = 0.2f;

    private int SelectedNumItems
    {
        get
        {
            return int.Parse(m_selectedNumItems.text);
        }
        set
        {
            m_selectedNumItems.text = value.ToString();
			m_amountInAction.text =  value.ToString();
        }
    }
    string TotalItems
    {
        get
        {
            return m_totalNumItems.text;
        }
        set
        {
            m_totalNumItems.text = value;
        }
    }
		
	private Text m_selectedNumItems;
    private Text m_totalNumItems;
    private Text m_action;
	private Text m_amountInAction;
	private Text m_materialActionText;
    private Button m_buttonLeft;
    private Button m_buttonRight;
    private Button m_buttonAction;
	private Text m_inventoryFull;
    private WidgetFader m_widgetFader;
	private Image m_sourceImage;

    private bool m_isLeftButtonPressed;
    private bool m_isRightButtonPressed;
    private bool m_isFirstClick;
    private float m_previousClickTime;

    UIContainer m_materialInventory;

    override protected void Awake()
    {
		base.Awake ();

        m_selectedNumItems = transform.FindDeepChild("NumItemsSelected").GetComponent<Text>();
		m_totalNumItems = transform.FindDeepChild("NumItemsTotal").GetComponent<Text>();
        m_action = transform.FindDeepChild("ActionText").GetComponent<Text>();
		m_amountInAction = transform.FindDeepChild("AmountActionText").GetComponent<Text>();
		m_materialActionText = transform.FindDeepChild("MaterialActionText").GetComponent<Text>();
		m_inventoryFull = transform.FindDeepChild ("InventoryFull").GetComponent<Text>();
		m_sourceImage = transform.FindDeepChild ("SourceSprite").GetComponent<Image>();

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
		TotalItems = OnRetrieveCurrentItems ().ToString();

        if (m_isLeftButtonPressed || m_isRightButtonPressed)
        {
            if (((m_isFirstClick == true) && ((Time.time - m_previousClickTime) > OnButtonDownStartRepeatInterval)) ||
                ((m_isFirstClick == false) && ((Time.time - m_previousClickTime) > OnButtonDownContinueRepeatInterval)))
            {
                if (m_isLeftButtonPressed)
                {
					TryToChangeSelectedNumItems(SelectedNumItems - 1);
                }
                else if (m_isRightButtonPressed)
                {
					TryToChangeSelectedNumItems(SelectedNumItems + 1);
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
                m_buttonAction.interactable = false;
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
		TryToChangeSelectedNumItems(SelectedNumItems - 1);
        m_isLeftButtonPressed = true;
    }
    public void OnLeftClickUp()
    {
        m_isLeftButtonPressed = false;
    }
    public void OnRightClickDown()
    {
		TryToChangeSelectedNumItems(SelectedNumItems + 1);
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

	public void TryToChangeSelectedNumItems(int newValue)
	{
		int normalisedNewValue = newValue;
		int maxItems = OnRetrieveMaxItems ();

		if (newValue < 0)
		{
			normalisedNewValue = 0;
		}
		else if (newValue > maxItems)
		{
			normalisedNewValue = maxItems;
			SetInventoryFullMessageVisible (true);
			m_materialInventory.SignalError();
		}

		if (SelectedNumItems != normalisedNewValue)
		{
			SelectedNumItems = normalisedNewValue;
			UpdateUIAfterNumSelectedChanged ();
		}
	}

	public void SetSourceImage(Sprite newSprite)
	{
		m_sourceImage.sprite = newSprite;
	}

	private void UpdateUIAfterNumSelectedChanged ()
	{
		if(SelectedNumItems >= OnRetrieveMaxItems ())
		{
			SetInventoryFullMessageVisible (true);
		}
		{
			SetInventoryFullMessageVisible (false);
		}
		UpdateMoreAndLessButtons ();
	}

	private void UpdateMoreAndLessButtons()
	{
		if (SelectedNumItems <= 0)
		{
			m_buttonLeft.interactable = false;
		}
		else
		{
			m_buttonLeft.interactable = true;
		}

		if (SelectedNumItems >= OnRetrieveMaxItems())
		{
			m_buttonRight.interactable = false;
		}
		else
		{
			m_buttonRight.interactable = true;
		}
	}

	private void SetInventoryFullMessageVisible(bool visible)
	{
		float opacity = 1.0f;
		if (!visible)
		{
			opacity = 0.0f;
		}
		m_inventoryFull.color = new Color (
			m_inventoryFull.color.r,
			m_inventoryFull.color.g,
			m_inventoryFull.color.b,
			opacity);
	}

}
