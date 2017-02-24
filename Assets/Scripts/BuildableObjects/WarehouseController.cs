using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class WarehouseController : SelectableObject, IDropHandler {

    private GameObject m_containerPrefab;
    private GameObject m_containerInstance;
    private WidgetFader m_containerFader;
    private UIContainer m_containerController;
    private bool m_showContainer;

    void Start()
    {
        enabled = false;

        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_containerPrefab = Resources.Load("UI/GenericUIContainer6Slots") as GameObject;
        m_containerInstance = GameObject.Instantiate(m_containerPrefab, transform, false);
        m_containerFader = m_containerInstance.GetComponent<WidgetFader>();
        m_containerController = m_containerInstance.GetComponent<UIContainer>();

        m_containerController.SetWorldUI(true);
        m_containerInstance.transform.position = new Vector3(gameObject.transform.position.x, 
                                                             m_spriteRenderer.bounds.max.y + UIGlobals.PegDistanceToObject , 
                                                             0.0f);
        m_containerController.Title = "";

        m_onSelectedDelegate = ShowContainer;
        m_onDeselectedDelegate = HideContainer;
        m_showContainer = false;

        m_containerFader.DisableImmediate();
    }

    void ShowContainer()
    {
        m_showContainer = !m_showContainer;

        if (m_showContainer)
        {
            DisableDialog();
            m_containerFader.Enable();
        }
        else
        {
            EnableDialog();
            m_containerFader.Disable();
        }
    }

    void HideContainer()
    {
    }

    public void OnDrop(PointerEventData eventData)
    {
        //throw new NotImplementedException();
    }
}
