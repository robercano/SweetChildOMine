using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterStatus : MonoBehaviour {

    private Image m_lifeBar;
    private RectTransform m_lifeBarRect;
    private float m_lifeBarRatio;

    private Image m_avatar;
    private Text m_name;

    private Miner m_activeMiner;

    // Use this for initialization
    void Start()
    {
        m_lifeBar = transform.Find("LifeBar").gameObject.GetComponent<Image>();
        m_lifeBarRect = m_lifeBar.rectTransform;
        m_lifeBarRatio = m_lifeBarRect.sizeDelta.x;

        m_avatar = transform.Find("Avatar").gameObject.GetComponent<Image>();
        m_name = transform.Find("Name").gameObject.GetComponent<Text>();

        m_activeMiner = null;

        ClearAll();
    }

    void Update()
    {
        if (m_activeMiner != null)
        {
            UpdateInfo();
        }
    }

    void SetName(string name)
    {
        m_name.text = name;
    }
    void ClearName()
    {
        m_name.text = "";
    }

    void SetAvatar(Sprite avatar)
    {
        m_avatar.sprite = avatar;
        m_avatar.color = Color.white;
    }
    void ClearAvatar()
    {
        m_avatar.color = Color.clear;
        m_avatar.sprite = null;
    }

    void SetLife(float percentage)
    {
        percentage = Mathf.Clamp(percentage, 0.0f, 1.0f);

        m_lifeBarRect.sizeDelta = new Vector2(percentage * m_lifeBarRatio, m_lifeBarRect.sizeDelta.y);
    }
    void ClearLife()
    {
        m_lifeBarRect.sizeDelta = new Vector2(0.0f, m_lifeBarRect.sizeDelta.y);
    }

    void ClearAll()
    {
        ClearName();
        ClearAvatar();
        ClearLife();
    }

    /* Public API */
    public void UpdateInfo()
    {
        SetAvatar(m_activeMiner.GetCurrentAvatar());
        SetName(m_activeMiner.Name);
        SetLife(m_activeMiner.Life / m_activeMiner.MaxLife);
    }

    public void SetActiveMiner(Miner miner)
    {
        m_activeMiner = miner;

        if (m_activeMiner == null)
            ClearAll();
        else
            UpdateInfo();
    }

    public Miner GetActiveMiner()
    {
        return m_activeMiner;
    }
}
