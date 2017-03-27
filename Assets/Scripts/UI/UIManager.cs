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
    public Font[] Fonts;

    private Dictionary<string, GameObject> m_preloadedPrefabs;

    private CanvasScaler m_canvasScaler;

    private int m_screenWidth;
    private int m_screenHeight;

    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();

        m_preloadedPrefabs = new Dictionary<string, GameObject>();        
        m_canvasScaler = GetComponent<CanvasScaler>();

        foreach (Font font in Fonts)
        {
            font.material.mainTexture.filterMode = FilterMode.Point;
        }
        UpdateUISize();
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
            UpdateUISize();
        }
    }

    private void UpdateUISize()
    {
        m_screenWidth = Screen.width;
        m_screenHeight = Screen.height;

        int scale = Mathf.CeilToInt(m_screenHeight / (float)ReferenceHeight);

        m_canvasScaler.scaleFactor = scale;
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

    public void DestroyAllChildren()
    {
        foreach (Transform child in transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }
}
