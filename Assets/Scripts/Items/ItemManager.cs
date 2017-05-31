using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using com.kleberswf.lib.core;

public class ItemManager : Singleton<ItemManager> {
	public string[] PreloadItems = { "Silver", "Gold", "Warehouse", "PickAxe", "Generator" };

	private Dictionary<string, GameObject> m_preloadedPrefabs;

    protected override void Awake()
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

		Item item = itemInstance.GetComponent<Item> ();
        item.Amount = 1;
        return item;
	}

    public void DestroyItem(Item item)
    {
        GameObject.Destroy(item.gameObject);
    }
	#endregion /* Public interface */
}
