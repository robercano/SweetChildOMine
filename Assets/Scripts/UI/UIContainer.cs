using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIContainer : MonoBehaviour {

    public string Name
    {
        get
        {
            return Name;
        }

        set
        {
            Name = value;
            m_UITexts[0].text = Name;
        }
    }
    
    private Text[] m_UITexts;
    private Image[] m_UIImages;
    private Sprite[] m_slots;

    // Used to sort the slots from left to right
    public class SlotComparer : IComparer<Image>
    {
        public int Compare(Image a, Image b)
        {
            if (a.transform.position.x < b.transform.position.x)
                return -1;
            else
                return 1;
        }
    }

    // Use this for initialization
    void Start()
    {
        IComparer<Image> comparer = new SlotComparer();

        m_UITexts = GetComponentsInChildren<Text>();
        m_UIImages = GetComponentsInChildren<Image>();

        m_slots = new Sprite[m_UIImages.Length];

        Array.Sort(m_UIImages, comparer);
    }

    public bool SetSlot(int slot, Sprite sprite)
    {
        if ((slot == 0) || ((slot - 1) > m_slots.Length))
            return false;

        m_slots[slot] = sprite;
        m_UIImages[slot].sprite = sprite;
        m_UIImages[slot].color = Color.white;

        return true;
    }
    public bool ClearSlot(int slot)
    {
        if ((slot == 0) || ((slot - 1) > m_slots.Length))
            return false;

        m_slots[slot] = null;
        m_UIImages[slot].sprite = null;
        m_UIImages[slot].color = Color.clear;

        return true;
    }
    public void ClearAllSlots()
    {
        for (int i = 0; i < m_slots.Length; ++i)
            ClearSlot(i);
    }

    public bool SetShortcut(int slot, char shortcut)
    {
        if ((slot == 0) || ((slot - 1) > m_slots.Length))
            return false;

        m_UITexts[slot].text = shortcut.ToString();

        return true;
    }
    public bool ClearShortcut(int slot)
    {
        if ((slot == 0) || ((slot - 1) > m_slots.Length))
            return false;

        m_UITexts[slot].text = "";

        return true;
    }
    public void ClearAllShortcut()
    {
        for (int i = 0; i < m_UITexts.Length; ++i)
            ClearShortcut(i);
    }

    public void ClearAll()
    {
        ClearAllSlots();
        ClearAllShortcut();
    }
}
