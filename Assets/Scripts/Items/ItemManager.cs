using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemManager {
	public string[] PreloadItems = { "Silver", "Gold", "Warehouse", "PickAxe" };

	private Dictionary<string, GameObject> m_preloadedPrefabs;

	/* Singleton */
	static readonly ItemManager instance = new ItemManager();
	public static ItemManager Instance {
		get {
			return instance;
		}
	}

	static ItemManager() {}
	private ItemManager()
	{
        m_preloadedPrefabs = new Dictionary<string, GameObject>();
        PreloadPrefabs();

    }

    GameObject PreloadPrefab(string name)
    {
        GameObject item = Resources.Load("Items/" + name) as GameObject;
        if (item != null)
        {
            m_preloadedPrefabs[name] = item;
        }      
        return item;
    }

	void PreloadPrefabs()
	{
		foreach (string itemName in PreloadItems) {
            if (PreloadPrefab(itemName) == null)
            {
                Debug.LogError("ERROR preloading prefab: " + itemName);
            }
        }
	}

	#region /* Public interface */
	public Item CreateItem(string name)
	{
		GameObject itemPrefab = null;
		if (m_preloadedPrefabs.TryGetValue (name, out itemPrefab) == false) {
            /* Try to load it now */
            Debug.LogWarning("Prefab was not preloaded: " + name);
            itemPrefab = PreloadPrefab(name);
		}
        if (itemPrefab == null)
        {
            return null;
        }

		GameObject itemInstance = GameObject.Instantiate (itemPrefab);
		Assert.IsNotNull (itemInstance);

		return itemInstance.GetComponent<Item> ();
	}

    public void DestroyItem(Item item)
    {
        GameObject.Destroy(item.gameObject);
    }
	#endregion /* Public interface */
}
