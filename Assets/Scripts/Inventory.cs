using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Inventory {

    public delegate void InventoryUpdate();
    public InventoryUpdate OnInventoryUpdate;

    private List<Item> m_inventorySlots;
    private Dictionary<string, int> m_inventoryMap;

    public int CurrentWeight;
    public int RemainingWeight;

    public Inventory(int maxSlots, int maxWeight)
    {
        m_inventorySlots = new List<Item>(maxSlots);
        m_inventoryMap = new Dictionary<string, int>(maxSlots);
        OnInventoryUpdate = null;

        RemainingWeight = maxWeight;
        CurrentWeight = 0;
    }

    public bool AddItem(Item item)
    {
        int slot;

        if (item.TotalWeight > RemainingWeight)
            return false;

        if (m_inventoryMap.TryGetValue(item.Name, out slot))
        {
            Item curItem = m_inventorySlots[slot];
            curItem.Amount += item.Amount;
            m_inventorySlots[slot] = curItem;
            CurrentWeight += item.TotalWeight;
            RemainingWeight -= item.TotalWeight;

			RefreshInventory ();

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
        CurrentWeight += item.TotalWeight;
        RemainingWeight -= item.TotalWeight;

		RefreshInventory ();

        return true;
    }

	public bool TransferItem(Item item)
	{
		Item curItem = GetItemByName (item.Name);
		if (curItem != item) {
			return false;
		}

		CurrentWeight -= item.Amount*item.WeightPerUnit;
		RemainingWeight += item.Amount*item.WeightPerUnit;

		m_inventoryMap.Remove(item.Name);
		m_inventorySlots.Remove(item);

		RefreshInventory();
		return true;
	}

	public bool RemoveItemAmount(string name, ref int amount)
	{
		Item item = GetItemByName (name);
		if (item == null)
			return false;
		if (item.Amount >= amount) {
			item.Amount -= amount;
		} else {
			amount = item.Amount;
			item.Amount = 0;
		}

        CurrentWeight -= amount*item.WeightPerUnit;
        RemainingWeight += amount*item.WeightPerUnit;

        // If amount is zero, remove the item from the inventory
        if (item.Amount == 0)
        {
            m_inventoryMap.Remove(item.Name);
            m_inventorySlots.Remove(item);
            ItemManager.Instance.DestroyItem(item);
        }

        RefreshInventory();
		return true;
	}

	public Item GetItemByName(string name)
	{
		int slotIndex;
		if (m_inventoryMap.TryGetValue (name, out slotIndex) == false)
			return null;
		return m_inventorySlots [slotIndex];
	}

    public Item GetItemAtSlot(int slot)
    {
        if (slot >= m_inventorySlots.Count)
        {
            return null;
        }

        return m_inventorySlots[slot];
    }

	public int GetItemAmount(string name)
	{
		Item item = GetItemByName (name);
		if (item == null)
			return 0;
		return item.Amount;
	}

    public int GetCount()
    {
        return m_inventorySlots.Count;
    }
    public int GetMaxSlots()
    {
        return m_inventorySlots.Capacity;
    }

	public void RefreshInventory()
	{
		if (OnInventoryUpdate != null)
			OnInventoryUpdate();
	}
}
