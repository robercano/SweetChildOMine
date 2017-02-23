using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WarehouseController : SelectableObject {

    private GameObject m_containerPrefab;
    private GameObject m_containerInstance;
    private WidgetFader m_containerFader;
    private UIContainer m_containerController;

    void Start()
    {
        enabled = false;

        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_containerPrefab = Resources.Load("UI/GenericUIContainer6Slots") as GameObject;
        m_containerInstance = GameObject.Instantiate(m_containerPrefab, transform, false);
        m_containerInstance.GetComponent<Canvas>().renderMode = RenderMode.WorldSpace;
        m_containerInstance.transform.position = new Vector3(gameObject.transform.position.x, m_spriteRenderer.bounds.max.y + 5.0f, 0.0f);
        m_containerFader = m_containerInstance.GetComponent<WidgetFader>();
        m_containerController = m_containerInstance.GetComponent<UIContainer>();
        m_containerController.Title = "";

        m_onSelectedDelegate = ShowContainer;
        m_onDeselectedDelegate = HideContainer;

        m_containerFader.DisableImmediate();
    }

    void ShowContainer()
    {
        m_containerFader.Enable();
    }

    void HideContainer()
    {
        m_containerFader.Disable();
    }
}
