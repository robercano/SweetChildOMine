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
    private Inventory m_inventory;

    // Use this for initialization
    void Start()
    {
        m_UITexts = GetComponentsInChildren<Text>();
        m_UIImages = GetComponentsInChildren<Image>();

        m_inventory = null;
    }

    void InventoryUpdate()
    {
        for (int i = 0; i < m_inventory.GetCount(); ++i)
        {
            Item item = m_inventory.GetItemAtSlot(i);
            if (SetSlot(i, item.Avatar, "x" + item.Amount.ToString()) == false)
                Debug.Log("ERROR setting slot");
        }
    }

    public void SetInventory(Inventory inventory)
    {
        m_inventory = inventory;
        m_inventory.m_inventoryUpdateDelegate = InventoryUpdate;
        InventoryUpdate();
    }

    public bool SetSlot(int slot, Sprite sprite, string text)
    {
        slot++;

        if (slot < 1 || slot >= m_UIImages.Length)
            return false;

        m_UIImages[slot].sprite = sprite;
        m_UIImages[slot].color = sprite == null ? Color.clear : Color.white;
        m_UITexts[slot].text = text;

        return true;
    }

    public void ClearAllSlots()
    {
        for (int i = 1; i < m_UIImages.Length; ++i)
            SetSlot(i, null, null);
    }

    public bool SetText(int slot, char shortcut)
    {
        slot++;

        if (slot < 1 || slot >= m_UIImages.Length)
            return false;

        m_UITexts[slot].text = shortcut.ToString();

        return true;
    }
      public void ClearAllTexts()
    {
        for (int i = 0; i < m_UITexts.Length; ++i)
            SetText(i, ' ');
    }

    public void ClearAll()
    {
        ClearAllSlots();
        ClearAllTexts();
    }
}
