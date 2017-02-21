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

	[Serializable]
	public class RecipeItem {
		public Item Ingredient;
		public int Amount;
	}
	public RecipeItem[] Recipe;

    protected int m_totalWork;
    protected int m_currentWork;

    private GameObject m_buildingContextMenuPrefab;
    private GameObject m_buildingContextMenuInstance;
    private BuildingContextMenu m_buildingContextMenu;

    protected CharacterStatus m_characterStatus;

    private bool m_hasMaterials;

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

        m_hasMaterials = false;
    }

    public void ShowMenu()
    {
        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null && 
            miner.BuildableTarget != this &&
            (m_currentWork < m_totalWork))
        {
            m_buildingContextMenu.OnAction = OnActionBuild;
            m_buildingContextMenu.Enable();
            DisableDialog();
        }
    }
    public void HideMenu()
    {
        if (m_buildingContextMenu)
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
        if (miner == null)
            return;

        if (m_hasMaterials == false)
        {
            if (miner.RequestMaterialsForStructure(this) == false)
                return;

            m_hasMaterials = true;
        }
        miner.BuildStructure(this);
    }

    /* Public interface */
    public bool DoBuild(int workAmount, out int progress)
    {
        if (m_currentWork + workAmount > m_totalWork)
            m_currentWork = m_totalWork;
        else
            m_currentWork += workAmount;

        progress = 100 * m_currentWork / m_totalWork;

        if (m_currentWork >= m_totalWork)
            m_buildingContextMenu = null;

        return (m_currentWork < m_totalWork);
    }
}