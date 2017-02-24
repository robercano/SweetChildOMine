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
    public int Amount
    {
        get
        {
            int amount;
            int.TryParse(m_amount.text, out amount);
            return amount;
        }
        set
        {
            m_amount.text = "x" + value.ToString();
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
    private Text m_amount;
	private Item m_item;
	private Image m_material;
    private Image m_status;
    private WidgetFader m_widgetFader;

    // Use this for initialization
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_amount = transform.FindDeepChild("Amount").GetComponent<Text>();
        m_status = transform.FindDeepChild("Status").GetComponent<Image>();
        m_material = transform.FindDeepChild("Material").GetComponent<Image>();

        Title = "";
        Amount = 0;
        Material = null;

        m_widgetFader = GetComponent<WidgetFader>();
        m_widgetFader.DisableImmediate();
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