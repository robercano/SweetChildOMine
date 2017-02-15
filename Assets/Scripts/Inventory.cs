﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Inventory {

    public delegate void InventoryUpdate();
    public InventoryUpdate m_inventoryUpdateDelegate;

    private List<Item> m_inventorySlots;
    private Dictionary<string, int> m_inventoryMap;

    public int CurrentWeight;
    public int RemainingWeight;

    public Inventory(int maxSlots, int maxWeight)
    {
        m_inventorySlots = new List<Item>(maxSlots);
        m_inventoryMap = new Dictionary<string, int>(maxSlots);
        m_inventoryUpdateDelegate = null;

        RemainingWeight = maxWeight;
        CurrentWeight = 0;
    }

    public bool AddItem(Item item)
    {
        int slot;

        if (item.Weight > RemainingWeight)
            return false;

        if (m_inventoryMap.TryGetValue(item.Name, out slot))
        {
            Item curItem = m_inventorySlots[slot];
            curItem.Amount += item.Amount;
            m_inventorySlots[slot] = curItem;
            CurrentWeight += item.Weight;
            RemainingWeight -= item.Weight;

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
        CurrentWeight += item.Weight;
        RemainingWeight -= item.Weight;

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
