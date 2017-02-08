using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MineableObject : SelectableObject
{
    public int MaxItems;
    public string ActionName;

    protected int m_currentItems;

    private GameObject m_actionContextMenuPrefab;
    private GameObject m_actionContextMenuInstance;
    private ActionContextMenu m_actionContextMenu;

    public override void Awake()
    {
        base.Awake();

        m_actionContextMenuPrefab = Resources.Load("ActionContextMenu") as GameObject;

        m_actionContextMenuInstance = GameObject.Instantiate(m_actionContextMenuPrefab, transform, false);
        m_actionContextMenu = m_actionContextMenuInstance.GetComponent<ActionContextMenu>();

        m_actionContextMenuInstance.transform.localPosition = new Vector3(0.0f, 2.0f * m_boxCollider.bounds.extents.y + 5.0f, 0.0f);

        m_currentItems = MaxItems;

        m_actionContextMenu.Title = Name;
        m_actionContextMenu.ActionName = ActionName;
        m_actionContextMenu.OnAction = OnMine;
    }

    public override void ShowMenu()
    {
        DisableDialog();

        m_actionContextMenu.MaxNumItems = m_currentItems;
        m_actionContextMenu.Enable();
    }
    public override void HideMenu()
    {
        m_actionContextMenu.Disable();
        EnableDialog();
    }

    protected virtual void OnMine(int numItems)
    {
        Debug.Log("Mining " + numItems + " items");
        HideMenu();
        m_currentItems -= numItems;
        if (m_currentItems < 0)
            m_currentItems = 0;
    }
}