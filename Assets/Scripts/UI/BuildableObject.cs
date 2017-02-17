using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildableObject : SelectableObject
{
    public int NumSteps;
    public int WorkPerStep;
    public string ActionName;

    protected int m_totalWork;
    protected int m_currentWork;

    private GameObject m_buildingContextMenuPrefab;
    private GameObject m_buildingContextMenuInstance;
    private BuildingContextMenu m_buildingContextMenu;

    private CharacterStatus m_characterStatus;

    public override void Awake()
    {
        base.Awake();

        m_buildingContextMenuPrefab = Resources.Load("BuildingContextMenu") as GameObject;

        m_buildingContextMenuInstance = GameObject.Instantiate(m_buildingContextMenuPrefab, transform, false);
        m_buildingContextMenu = m_buildingContextMenuInstance.GetComponent<BuildingContextMenu>();

        m_buildingContextMenuInstance.transform.position = new Vector3(gameObject.transform.position.x, m_spriteRenderer.bounds.max.y + 5.0f, 0.0f);

        m_totalWork = NumSteps * WorkPerStep;
        m_currentWork = 0;

        m_buildingContextMenu.Title = Name;
        m_buildingContextMenu.ActionName = ActionName;
        m_buildingContextMenu.OnAction = null;
        m_buildingContextMenu.OnRetrieveWorkLeft = OnRetrieveWorkLeft;

        m_characterStatus = GameObject.Find("CharacterStatus").GetComponent<CharacterStatus>();

        m_onSelectedDelegate = ShowMenu;
        m_onDeselectedDelegate = HideMenu;
    }

    public void ShowMenu()
    {
        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
        {
            m_buildingContextMenu.OnAction = OnActionBuild;
        }

        DisableDialog();
        m_buildingContextMenu.Enable();
    }
    public void HideMenu()
    {
        m_buildingContextMenu.Disable();
        EnableDialog();
    }

    protected int OnRetrieveWorkLeft()
    {
        return m_totalWork -  m_currentWork;
    }

    protected virtual void OnActionBuild()
    {
        HideMenu();

        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
        {
            miner.BuildStructure(this);
        }
    }

    /* Public interface */
    public bool DoBuild(int workAmount, out int progress)
    {
        if (m_currentWork + workAmount > m_totalWork)
            m_currentWork = m_totalWork;
        else
            m_currentWork += workAmount;

        progress = 100 * m_currentWork / m_totalWork;

        return (m_currentWork < m_totalWork);
    }
}