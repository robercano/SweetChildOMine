using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using com.kleberswf.lib.core;
using System;
using System.Linq;

public class UIManager : Singleton<UIManager> {

    public int ReferenceWidth;
    public int ReferenceHeight;

    private Dictionary<string, GameObject> m_preloadedPrefabs;

    private CanvasScaler m_canvasScaler;

    private UIContainer m_weaponContainer;
    private UIContainer m_inventoryContainer;
    private UIContainer m_buildContainer;

    private GameObject m_characterStatusPrefab;
    private RectTransform m_characterStatusRectTransform;
    private Dictionary<Miner, CharacterStatus> m_characterStatusDict;

    private Miner m_activeMiner;

    private int m_screenWidth;
    private int m_screenHeight;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        m_preloadedPrefabs = new Dictionary<string, GameObject>();
        
        m_canvasScaler = GetComponent<CanvasScaler>();

        m_weaponContainer = GameObject.Find("WeaponContainer").GetComponent<UIContainer>();
        m_inventoryContainer = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();
        m_buildContainer = GameObject.Find("BuildContainer").GetComponent<UIContainer>();

        m_characterStatusPrefab = Resources.Load("UI/CharacterStatus") as GameObject;
        m_characterStatusRectTransform = m_characterStatusPrefab.GetComponent<RectTransform>();

        m_characterStatusDict = new Dictionary<Miner, CharacterStatus>(5);
        m_activeMiner = null;

        updateUISize();
        PreloadPrefabs();
    }

    GameObject PreloadPrefab(string name)
    {
        GameObject obj = Resources.Load("UI/" + name) as GameObject;
        if (obj != null)
        {
            m_preloadedPrefabs[name] = obj;
        }
        return obj;
    }

    void PreloadPrefabs()
    {
        var type = typeof(UIElement);
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => type.IsAssignableFrom(p) && p.IsSealed );

        foreach (Type t in types)
        {
            if (PreloadPrefab(t.Name) == null)
            {
                Debug.LogError("ERROR preloading prefab: " + t.Name);
            }
        }

    }

    void Update()
    {
        if (m_screenWidth != Screen.width || m_screenHeight != Screen.height)
        {
            updateUISize();
        }
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

    private void updateUISize()
    {
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        int scale = (int)Mathf.Round(m_screenHeight / ReferenceHeight);

        m_canvasScaler.scaleFactor = scale;
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

    public T CreateUIElement<T>(Transform parent = null) where T : UIElement
    {
        GameObject UIPrefab = null;
        string nam = typeof(T).Name;
        if (m_preloadedPrefabs.TryGetValue(typeof(T).Name, out UIPrefab) == false)
        {
            /* Try to load it now */
            Debug.LogWarning("Prefab was not preloaded: " + typeof(T).Name);
            UIPrefab = PreloadPrefab(typeof(T).Name);
        }
        if (UIPrefab == null)
        {
            return null;
        }

        GameObject UIInstance = GameObject.Instantiate(UIPrefab);
        Assert.IsNotNull(UIInstance);

        T UIComponent = UIInstance.GetComponent<T>();

        UIComponent.SetParent(parent);
        return UIComponent;
    }

    public void DestroyItem(Item item)
    {
        GameObject.Destroy(item.gameObject);
    }
}
