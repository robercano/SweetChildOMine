using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionContextMenu : MonoBehaviour {

    public int NumItems
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
    public string Info
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
    public string Action
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

    private Text m_numItems;
    private Text m_info;
    private Text m_action;
    
    void Awake()
    {
        m_numItems = transform.FindChild("NumItems").GetComponent<Text>();
        m_info = transform.FindChild("Info").GetComponent<Text>();
        m_action = transform.FindChild("ActionText").GetComponent<Text>();
    }
}
