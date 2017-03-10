using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuildInventoryDialogPanel : MonoBehaviour
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
            return m_maxAmount;
        }
        set
        {
            m_maxAmount = value;
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
				m_material.sprite = m_item.Avatar;
			} else {
				m_material.sprite = null;
			}
        }
    }
    
    private Text m_title;
    private Text m_description;
    private Text m_amount;
	private Item m_item;
	private Image m_material;
    private Image m_status;
    private WidgetFader m_widgetFader;

    private int m_currentAmount;
    private int m_maxAmount;

    // Use this for initialization
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_description = transform.FindDeepChild("Description").GetComponent<Text>();
        m_material = transform.FindDeepChild("Material").GetComponent<Image>();
        m_amount = transform.FindDeepChild("Amount").GetComponent<Text>();
        m_status = transform.FindDeepChild("Status").GetComponent<Image>();

        Title = "";
        CurrentAmount = 0;
        RequiredAmount = 0;
        Material = null;
        m_status.sprite = null;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
    }

    void updateAmount()
    {
        m_amount.text = m_currentAmount.ToString() + "/" + m_maxAmount.ToString(); ;
    }

    public void GreenStatus()
    {
        m_status.color = new Color(0.32915f, 1, 0.32915f);
    }

    public void RedStatus()
    {
        m_status.color = new Color(1, 0.34509f, 0.34509f);
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