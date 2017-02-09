using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinerInventory {

    private List<SelectableObject> m_inventorySlots;

    public MinerInventory(int maxSlots)
    {
        m_inventorySlots = new List<SelectableObject>(maxSlots);
    }

    public bool AddItem(SelectableObject item)
    {
        if (m_inventorySlots.Count == m_inventorySlots.Capacity)
            return false;

        m_inventorySlots.Add(item);
        return true;
    }

    public SelectableObject GetItemAtSlot(int slot)
    {
        if (slot >= m_inventorySlots.Count)
            return null;

        return m_inventorySlots[slot];
    }

    public bool RemoveItemAtSlot(int slot)
    {
        if (slot >= m_inventorySlots.Count)
            return false;

        m_inventorySlots.RemoveAt(slot);
        return true;
    }
}
