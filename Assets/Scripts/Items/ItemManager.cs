using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ItemManager {
	public string[] PreloadItems = { "Silver" };

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
		
	}

	void PreloadPrefabs()
	{
		foreach (string itemName in PreloadItems) {
			GameObject item = Resources.Load ("Items/" + itemName) as GameObject;
			Assert.IsNotNull (item);

			m_preloadedPrefabs [itemName] = item;
		}
	}

	#region /* Public interface */
	public Item CreateItem(string name)
	{
		GameObject itemPrefab = null;
		if (m_preloadedPrefabs.TryGetValue (name, out itemPrefab) == false) {
			return null;
		}

		GameObject itemInstance = GameObject.Instantiate (itemPrefab);
		Assert.IsNotNull (itemInstance);

		return itemInstance.GetComponent<Item> ();
	}
	#endregion /* Public interface */
}
