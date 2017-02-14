using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Inventory {

    public delegate void InventoryUpdate();
    public InventoryUpdate m_inventoryUpdateDelegate;

    private List<Item> m_inventorySlots;
    private Dictionary<string, int> m_inventoryMap;

    public Inventory(int maxSlots)
    {
        m_inventorySlots = new List<Item>(maxSlots);
        m_inventoryMap = new Dictionary<string, int>(maxSlots);
        m_inventoryUpdateDelegate = null;
    }

    public bool AddItem(Item item)
    {
        int slot;

        if (m_inventoryMap.TryGetValue(item.Name, out slot))
        {
            Item curItem = m_inventorySlots[slot];
            curItem.Amount += item.Amount;
            m_inventorySlots[slot] = curItem;

            if (m_inventoryUpdateDelegate != null)
                m_inventoryUpdateDelegate();

            GameObject.Destroy(item.gameObject);
            return true;
        }

        if (m_inventorySlots.Count == m_inventorySlots.Capacity)
        {
            GameObject.Destroy(item.gameObject);
            return false;
        }

        m_inventorySlots.Add(item);
        m_inventoryMap[item.Name] = m_inventorySlots.Count - 1;

        if (m_inventoryUpdateDelegate != null)
            m_inventoryUpdateDelegate();

        return true;
    }

    public Item GetItemAtSlot(int slot)
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
