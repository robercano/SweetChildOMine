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
            m_amount.text = value.ToString();
        }
    }
    public Sprite Material
    {
        get
        {
            return m_material.sprite;
        }
        set
        {
            m_material.sprite = value;
        }
    }

    private Text m_title;
    private Text m_amount;
    private Image m_material;
    private WidgetFader m_widgetFader;

    // Use this for initialization
    void Awake()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_amount = transform.FindDeepChild("Amount").GetComponent<Text>();
        m_material = transform.FindDeepChild("Material").GetComponent<Image>();

        Title = "";
        Amount = 0;
        Material = null;

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