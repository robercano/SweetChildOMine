using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using com.kleberswf.lib.core;

public class UIController : Singleton<UIController> {

    private UIContainer m_weaponContainer;
    private UIContainer m_inventoryContainer;
    private UIContainer m_buildContainer;

    // Use this for initialization
    void Start()
    {
        m_weaponContainer = GameObject.Find("WeaponContainer").GetComponent<UIContainer>();
        m_inventoryContainer = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();
        m_buildContainer = GameObject.Find("BuildContainer").GetComponent<UIContainer>();
    }

    public void Refresh()
    {
        m_weaponContainer.InventoryUpdate();
        m_inventoryContainer.InventoryUpdate();
        m_buildContainer.InventoryUpdate();
    }
}
