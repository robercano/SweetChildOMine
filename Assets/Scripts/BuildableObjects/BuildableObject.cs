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
		public string Ingredient;
		public int Amount;
	}
	public RecipeItem[] Recipe;

    protected int m_totalWork;
    protected int m_currentWork;

    private BuildingActionMenu m_buildingContextMenu;

    AudioClip m_finishedSound;

    protected UIManager m_UIController;

    private bool m_hasMaterials;

    public override void Awake()
    {
        base.Awake();

        m_buildingContextMenu = UIManager.Instance.CreateUIElement<BuildingActionMenu>();
		m_buildingContextMenu.FollowGameObject (this.gameObject);

        m_totalWork = NumSteps * WorkPerStep;
        m_currentWork = 0;

        m_buildingContextMenu.Title = Name;
        m_buildingContextMenu.ActionName = ActionName;
        m_buildingContextMenu.OnAction = null;
        m_buildingContextMenu.OnRetrieveWorkLeft = OnRetrieveWorkLeft;

        m_finishedSound = Resources.Load("Sounds/FinishedBuilding") as AudioClip;

        m_UIController = GameObject.Find("MainUI").GetComponent<UIManager>();

        m_onSelectedDelegate = ShowMenu;
        m_onDeselectedDelegate = HideMenu;

        m_hasMaterials = false;
    }

    public void ShowMenu()
    {
        Miner miner = m_UIController.GetActiveMiner();
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

        Miner miner = m_UIController.GetActiveMiner();
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
        {
            m_buildingContextMenu = null;
            AudioSource.PlayClipAtPoint(m_finishedSound, Camera.main.transform.position);
        }

        return (m_currentWork < m_totalWork);
    }
}