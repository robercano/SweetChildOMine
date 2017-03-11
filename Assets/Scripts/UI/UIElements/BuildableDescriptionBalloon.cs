using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class BuildableDescriptionBalloon : UIElement
{
    public Sprite Valid;
    public Sprite Invalid;

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
    public int CurrentAmount
    {
        get
        {
            return m_currentAmount;
        }
        set
        {
            m_currentAmount = value;
            updateAmount();
        }
    }
    public int RequiredAmount
    {
        get
        {
            return m_requiredAmount;
        }
        set
        {
            m_requiredAmount = value;
            updateAmount();
        }
    }
	public Item Material
    {
        get
        {
			return m_item;
        }
        set
        {
			m_item = value;
			if (m_item != null) {
				m_material.sprite = m_item.StaticAvatar;
			} else {
				m_material.sprite = null;
			}
        }
    }
    
    private Text m_title;
    private Text m_description;
    private Text m_currentAmountText;
    private Text m_requiredAmountText;
    private Item m_item;
	private Image m_material;
    private Image m_status;
    private WidgetFader m_widgetFader;

    private int m_currentAmount;
    private int m_requiredAmount;

    // Use this for initialization
    override protected void Awake()
    {
        base.Awake();

        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_description = transform.FindDeepChild("Description").GetComponent<Text>();
        m_material = transform.FindDeepChild("Material").GetComponent<Image>();
        m_currentAmountText = transform.FindDeepChild("CurrentAmount").GetComponent<Text>();
        m_requiredAmountText = transform.FindDeepChild("RequiredAmount").GetComponent<Text>();
        m_status = transform.FindDeepChild("Status").GetComponent<Image>();

        Title = "";
        CurrentAmount = 0;
        RequiredAmount = 0;
        Material = null;
        m_status.sprite = Invalid;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    void updateAmount()
    {
        m_currentAmountText.text = m_currentAmount.ToString();
        m_requiredAmountText.text = "/" + m_requiredAmount.ToString(); ;
    }

    public void GreenStatus()
    {
        m_status.sprite = Valid;
        m_currentAmountText.color = new Color(0.0f, 0.5960784f, 0.13725490f);
    }

    public void RedStatus()
    {
        m_status.sprite = Invalid;
        m_currentAmountText.color = new Color(0.7568627f, 0.13725490f, 0.1529411f);
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