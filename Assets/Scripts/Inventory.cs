using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Inventory {

    public struct InventoryItem
    {
        public string name;
        public Sprite avatar;
        public int amount;

        public InventoryItem(string name, Sprite avatar, int amount)
        {
            this.name = name;
            this.avatar = avatar;
            this.amount = amount;
        }
    };
    public delegate void InventoryUpdate();
    public InventoryUpdate m_inventoryUpdateDelegate;

    private List<InventoryItem> m_inventorySlots;
    private Dictionary<string, int> m_inventoryMap;

    public Inventory(int maxSlots)
    {
        m_inventorySlots = new List<InventoryItem>(maxSlots);
        m_inventoryMap = new Dictionary<string, int>(maxSlots);
        m_inventoryUpdateDelegate = null;
    }

    public bool AddItem(InventoryItem item)
    {
        int slot;

        if (m_inventoryMap.TryGetValue(item.name, out slot))
        {
            InventoryItem curItem = m_inventorySlots[slot];
            curItem.amount += item.amount;
            m_inventorySlots[slot] = curItem;

            if (m_inventoryUpdateDelegate != null)
                m_inventoryUpdateDelegate();
            return true;
        }

        if (m_inventorySlots.Count == m_inventorySlots.Capacity)
            return false;

        m_inventorySlots.Add(item);
        m_inventoryMap[item.name] = m_inventorySlots.Count - 1;

        if (m_inventoryUpdateDelegate != null)
            m_inventoryUpdateDelegate();

        return true;
    }

    public InventoryItem? GetItemAtSlot(int slot)
    {
        if (slot >= m_inventorySlots.Count)
        {
            return null;
        }

        return m_inventorySlots[slot];
    }

    public int GetCount()
    {
        return m_inventorySlots.Count;
    }
}
