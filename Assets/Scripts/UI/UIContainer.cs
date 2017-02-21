using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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
            m_title.text = Name;
        }
    }

    private Text m_title;
    private ContainerSlot[] m_slots;
    private Inventory m_inventory;

    // Use this for initialization
    void Start()
    {
        m_title = transform.FindDeepChild("Title").GetComponent<Text>();
        m_slots = GetComponentsInChildren<ContainerSlot>();

        m_inventory = null;
    }

    void InventoryUpdate()
    {
        for (int i = 0; i < m_inventory.GetCount(); ++i)
        {
            Item item = m_inventory.GetItemAtSlot(i);
            if (SetSlot(i, item) == false)
                Debug.Log("ERROR setting slot");
        }
    }

    public void SetInventory(Inventory inventory)
    {
        m_inventory = inventory;
        m_inventory.m_inventoryUpdateDelegate = InventoryUpdate;
        InventoryUpdate();
    }

    public bool SetSlot(int slot, Item item, char shortcut = ' ')
    {
        if (slot < 0 || slot >= m_slots.Length)
            return false;

        m_slots[slot].SlotItem = item;
        m_slots[slot].Shortcut = shortcut;
        return true;
    }

    public void ClearAllSlots()
    {
        for (int i = 1; i < m_slots.Length; ++i)
            SetSlot(i, null);
    }

    public bool SetShortcut(int slot, char shortcut)
    {
        slot++;

        if (slot < 1 || slot >= m_slots.Length)
            return false;

        //m_UITexts[slot].text = shortcut.ToString();

        return true;
    }
      public void ClearTitle()
    {
        m_title.text = "";
    }

    public void ClearAll()
    {
        ClearAllSlots();
        ClearTitle();
    }
}
