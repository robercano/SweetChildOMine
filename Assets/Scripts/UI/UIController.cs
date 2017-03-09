using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kleberswf.lib.core;

public class UIController : Singleton<UIController> {

    private RectTransform m_rectTransform;

    private UIContainer m_weaponContainer;
    private UIContainer m_inventoryContainer;
    private UIContainer m_buildContainer;

    private GameObject m_characterStatusPrefab;
    private RectTransform m_characterStatusRectTransform;
    private Dictionary<Miner, CharacterStatus> m_characterStatusDict;

    private Miner m_activeMiner;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        m_rectTransform = GetComponent<RectTransform>();

        m_weaponContainer = GameObject.Find("WeaponContainer").GetComponent<UIContainer>();
        m_inventoryContainer = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();
        m_buildContainer = GameObject.Find("BuildContainer").GetComponent<UIContainer>();

        m_characterStatusPrefab = Resources.Load("UI/CharacterStatus") as GameObject;
        m_characterStatusRectTransform = m_characterStatusPrefab.GetComponent<RectTransform>();

        m_characterStatusDict = new Dictionary<Miner, CharacterStatus>(5);
        m_activeMiner = null;
    }

    private CharacterStatus AddMiner(Miner miner)
    {
        GameObject statusInstance = GameObject.Instantiate(m_characterStatusPrefab, transform);
        statusInstance.GetComponent<RectTransform>().anchoredPosition = new Vector2(18.0f + m_characterStatusDict.Count * (m_characterStatusRectTransform.sizeDelta.x + 10.0f), -60.0f);
        statusInstance.GetComponent<RectTransform>().localScale = Vector3.one;

        statusInstance.name = "CharacterStatus" + miner.Name.Replace(" " , "");

        CharacterStatus characterStatus = statusInstance.GetComponent<CharacterStatus>();

        characterStatus.SetActiveMiner(miner);

        m_characterStatusDict.Add(miner, characterStatus);

        return characterStatus;
    }

    public void Refresh()
    {
        m_weaponContainer.InventoryUpdate();
        m_inventoryContainer.InventoryUpdate();
        m_buildContainer.InventoryUpdate();
    }
    
    public void SetActiveMiner(Miner miner)
    {
        CharacterStatus characterStatus = null;

        if (m_characterStatusDict.TryGetValue(miner, out characterStatus) == false)
        {
            characterStatus = AddMiner(miner);
        }

        m_weaponContainer.SetInventory(miner.WeaponInventory);
        m_inventoryContainer.SetInventory(miner.MaterialInventory);
        m_buildContainer.SetInventory(miner.BuildInventory);

        m_activeMiner = miner;
    }

    public Miner GetActiveMiner()
    {
        return m_activeMiner;
    }
}
