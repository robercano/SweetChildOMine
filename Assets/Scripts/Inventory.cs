using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Inventory {

    public delegate void InventoryUpdate();
    public InventoryUpdate OnInventoryUpdate;

    public int RemainingAmount
    {
        get
        {
            int remainingAmount = m_inventorySlots.Capacity * m_maxAmountPerSlot;

            foreach (Item item in m_inventorySlots)
            {
                remainingAmount -= item.Amount;
            }
            return remainingAmount;
        }
    }
    public Inventory(Item.Type itemType, int maxSlots, int maxAmountPerSlot)
    {
        m_itemType = itemType;
        m_inventorySlots = new List<Item>(maxSlots);
        OnInventoryUpdate = null;

        m_maxAmountPerSlot = maxAmountPerSlot;
    }

    private List<Item> m_inventorySlots;
    private int m_maxAmountPerSlot;
    private Item.Type m_itemType;

    /**
        Adds a new Item to the inventory. If the number of items to add
        does not fit in a single slot, the Item is distributed among
        different slots. The remaining number of items, if any, is returned
        through the only parameter in the function. The return value indicates
        wheter all items have been added (true) or not (false)

        @param newItem  New item to add to the inventory

        @return true if all items where added to the inventory, false otherwise.
                When this functions returns false, newItem.Amount contains the
                remaing items that were not added
    */
    public bool AddItem(Item newItem)
    {
        if (newItem.ItemType != m_itemType)
        {
            return false;
        }

        // Add the item amount to existing slots
        foreach (Item item in m_inventorySlots)
        {
            if (item.Name != newItem.Name)
            {
                continue;
            }

            int amountToTransfer = newItem.Amount;
            if ((item.Amount + amountToTransfer) > m_maxAmountPerSlot)
            {
                amountToTransfer = m_maxAmountPerSlot - item.Amount;
            }

            newItem.Amount -= amountToTransfer;
            item.Amount += amountToTransfer;

            if (newItem.Amount == 0)
            {
                break;
            }
        }

        // Add the rest to new slots
        while (newItem.Amount != 0 && (m_inventorySlots.Count != m_inventorySlots.Capacity))
        {
            Item itemToAdd = ItemManager.Instance.CreateItem(newItem.Name);
            if (newItem.Amount > m_maxAmountPerSlot)
            {
                itemToAdd.Amount = m_maxAmountPerSlot;
            }
            else
            {
                itemToAdd.Amount = newItem.Amount;
            }
            m_inventorySlots.Add(itemToAdd);

            newItem.Amount -= itemToAdd.Amount;
        }

		RefreshInventory ();

        // If not all amount was transferred, signal to caller
        if (newItem.Amount != 0)
        {
            return false;
        }

       ItemManager.Instance.DestroyItem(newItem);
        return true;
    }

	public bool TransferItem(Item item)
	{
		if (m_inventorySlots.Remove(item) == false)
        {
            return false;
        }

        RefreshInventory();
        return true;
	}

	public bool RemoveItemAmount(string name, ref int amount)
	{
        bool itemFound = false;

        for (int i = m_inventorySlots.Count-1; amount > 0 && i >= 0; --i)
        {
            Item item = m_inventorySlots[i];
            if (item.Name == name)
            {
                itemFound = true;

                if (amount >= item.Amount)
                {
                    amount -= item.Amount;
                    m_inventorySlots.RemoveAt(i);
                    ItemManager.Instance.DestroyItem(item);
                }
                else
                {
                    item.Amount -= amount;
                    amount = 0;
                }
            }
        }

        if (itemFound)
        {
            RefreshInventory();
        }
		return itemFound;
	}

    bool IsItemInInventory(Item item)
    {
        return m_inventorySlots.Contains(item);
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
        List<Item> foundItems = m_inventorySlots.FindAll(item => item.Name == name);

        int amount = 0;
        foreach (Item item in foundItems)
        {
            amount += item.Amount;
        }
        return amount;
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
