﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WarehouseController : SelectableObject, IDropHandler {

	public int MaxWeight;
	public int MaxSlots;
    public bool AlreadyBuilt;

	private Inventory m_warehouseInventory;
    private GameObject m_containerPrefab;
    private GameObject m_containerInstance;
	private WidgetFader m_containerFader;
    private UIContainer m_containerController;
    private bool m_showContainer;
    private Image[] m_slots;

    void Start()
    {
        if (AlreadyBuilt)
        {
            GetComponent<SpriteOutline>().enabled = false;
            GetComponent<WarehouseBuildable>().enabled = false;
            GetComponent<DragDropController>().enabled = false;
        }
        else
        {
            this.enabled = false;
        }

        m_spriteRenderer = GetComponent<SpriteRenderer>();

		m_warehouseInventory = new Inventory (MaxSlots, MaxWeight);

        m_containerPrefab = Resources.Load("UI/GenericUIContainer6Slots") as GameObject;
        m_containerInstance = GameObject.Instantiate(m_containerPrefab, transform, false);
        m_containerFader = m_containerInstance.GetComponent<WidgetFader>();
        m_containerController = m_containerInstance.GetComponent<UIContainer>();
		m_containerController.SetInventory (m_warehouseInventory);

        m_containerController.SetWorldUI(true);
        m_containerInstance.transform.position = new Vector3(gameObject.transform.position.x, 
                                                             m_spriteRenderer.bounds.max.y + UIGlobals.PegDistanceToObject , 
                                                             0.0f);
        m_containerController.Title = "";

        m_onSelectedDelegate = ShowContainer;
        m_onDeselectedDelegate = HideContainer;
        m_showContainer = false;

        m_slots = transform.GetComponentsInChildren<Image>();
        m_containerFader.DisableImmediate();

        m_warehouseInventory.OnInventoryUpdate += RefreshSlots;
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

    public void RefreshSlots()
    {
        for (int i=0; i<m_warehouseInventory.GetMaxSlots(); ++i)
        {
            Item item = m_warehouseInventory.GetItemAtSlot(i);
            if (item != null) {
                m_slots[i].sprite = item.Avatar;
                m_slots[i].color = Color.white;
            }
            else
            {
                m_slots[i].sprite = null;
                m_slots[i].color = Color.clear;
            }
        }
    }
	public void OnDrop(PointerEventData eventData)
	{
		m_containerController.OnDrop (eventData);
	}
}
