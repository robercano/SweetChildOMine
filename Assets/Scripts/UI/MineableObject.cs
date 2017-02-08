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

    private CharacterStatus m_characterStatus;

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
        m_actionContextMenu.OnAction = OnActionMine;

        m_characterStatus = GameObject.Find("CharacterStatus").GetComponent<CharacterStatus>();
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

    protected virtual void OnActionMine(int numItems)
    {
        HideMenu();

        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
        {
            miner.MineMaterial(this, numItems);
        }
    }

    /* Public interface */
    public int DoMine(int numItems)
    {
        if (m_currentItems > numItems)
        {
            m_currentItems -= numItems;
        }
        else
        {
            numItems = m_currentItems;
            m_currentItems = 0;
        }
        return numItems;
    }
}