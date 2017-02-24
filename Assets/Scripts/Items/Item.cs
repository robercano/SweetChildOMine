﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Name;
    public Sprite Avatar
    {
        get
        {
            return m_spriteRenderer.sprite;
        }
    }
    public string Description;
    public int Amount;
    public int WeightPerUnit;
    public int TotalWeight
    {
        get
        {
            return Amount * WeightPerUnit;
        }
    }

    public GameObject BuildablePrefab;
	private SpriteRenderer m_spriteRenderer;

	private UIContainer m_materialInventory;

	private GameObject m_dragDropObject;
	private DragDropController m_dragDropObjectController;

	private CharacterStatus m_characterStatus;

    protected virtual void Awake()
    {
		m_spriteRenderer = GetComponent<SpriteRenderer>();

		m_materialInventory = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();

		m_characterStatus = GameObject.FindObjectOfType<CharacterStatus>();

		m_dragDropObject = null;
		m_dragDropObjectController = null;

        Hide();
    }

    #region /* Public interface */
    public void Show()
    {
        m_spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        m_spriteRenderer.enabled = false;
    }

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (BuildablePrefab != null)
		{
			Miner miner = m_characterStatus.GetActiveMiner ();
			if (miner == null)
				return;

			// TODO: Allocate resources for buildable here
			if (miner.CheckRecipeForBuildableObject (BuildablePrefab)) {
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint (eventData.position);
				m_dragDropObject = GameObject.Instantiate (BuildablePrefab, mousePosition, Quaternion.identity);
				m_dragDropObjectController = m_dragDropObject.GetComponent<DragDropController> ();
				m_dragDropObjectController.StartDrag ();
			} else {
				// TODO: optimize this
				BuildableObject buildable = BuildablePrefab.GetComponent<BuildableObject>();
				foreach (BuildableObject.RecipeItem recipeItem in buildable.Recipe)
				{
					m_materialInventory.SignalError(recipeItem.Ingredient);
				}
			}		
		}
		else
		{
			m_dragDropObject = gameObject;
			Show();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_dragDropObject == null) {
			return;
		}

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
		m_dragDropObject.transform.position = mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_dragDropObject == null) {
			return;
		}
		if (m_dragDropObjectController != null) {
			m_dragDropObjectController.FinishDrag ();
		}
		if (m_dragDropObject == gameObject) {
			Hide ();
		}
		m_dragDropObject = null;
		m_dragDropObjectController = null;
	}
    #endregion /* Public interface */
};
