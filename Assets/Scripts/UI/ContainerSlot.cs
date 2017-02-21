using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ContainerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    public Item SlotItem
    {
        get
        {
            return m_slotItem;
        }
        set
        {
            m_slotItem = value;

            if (m_slotItem == null)
            {
                m_slotImage.color = Color.clear;
                m_descriptionPanel.Title = "";
                m_descriptionPanel.Description = "";
                m_buildableDescriptionPanel.Title = "";
                return;
            }

            // Set common fields
            m_slotImage.sprite = m_slotItem.Avatar;


            // Non-buildable object
            if (m_slotItem.ObjectPrefab == null)
            {
                m_slotImage.color = Color.white;
                m_descriptionPanel.Title = m_slotItem.Name;
                m_descriptionPanel.Description = m_slotItem.Description;
                return;
            }

            // Buildable object
            m_buildableObject = m_slotItem.ObjectPrefab.GetComponent<BuildableObject>();
            Assert.IsNotNull(m_buildableObject);

            Miner miner = m_characterStatus.GetActiveMiner();
            Assert.IsNotNull(miner);

            if (miner.CheckRecipeForBuildableObject(m_buildableObject))
            {
                m_slotImage.color = Color.white;
                m_buildableDescriptionPanel.GreenStatus();
            }
            else
            {
                m_slotImage.color = new Color(1, 1, 1, 0.5f);
                m_buildableDescriptionPanel.RedStatus();
            }

            // TODO: Iterate the recipe and show all elements!
            m_buildableDescriptionPanel.Title = m_slotItem.Name;
            m_buildableDescriptionPanel.Amount = m_buildableObject.Recipe[0].Amount;
            m_buildableDescriptionPanel.Material = m_buildableObject.Recipe[0].Ingredient.Avatar;
        }
    }
    private Item m_slotItem;
    private Image m_slotImage;
    private RectTransform m_rectTransform;

    private GameObject m_descriptionPanelPrefab;
    private GameObject m_descriptionInstance;
    private InventoryDescriptionPanel m_descriptionPanel;
    private RectTransform m_descriptionInstanceRectTransform;

    private GameObject m_buildableDescriptionPanelPrefab;
    private GameObject m_buildableDescriptionInstance;
    private BuildInventoryDialogPanel m_buildableDescriptionPanel;    
    private RectTransform m_buildableDescriptionInstanceRectTransform;

    private GameObject m_dragDropObject;
    private DragDropController m_dragDropObjectController;

	private CharacterStatus m_characterStatus;
    private BuildableObject m_buildableObject;

    // Use this for initialization
    void Awake()
    {
        m_slotImage = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();

        m_descriptionPanelPrefab = Resources.Load("InventoryDescriptionPanel") as GameObject;
        m_descriptionInstance = GameObject.Instantiate(m_descriptionPanelPrefab, transform, false);
        m_descriptionPanel = m_descriptionInstance.GetComponent<InventoryDescriptionPanel>();
        m_descriptionInstanceRectTransform = m_descriptionInstance.GetComponent<RectTransform>();
        m_descriptionInstanceRectTransform.anchoredPosition = new Vector2(m_rectTransform.sizeDelta.x / 2.0f, m_rectTransform.sizeDelta.y);

        m_buildableDescriptionPanelPrefab = Resources.Load("BuildInventoryDialogPanel") as GameObject;
        m_buildableDescriptionInstance = GameObject.Instantiate(m_buildableDescriptionPanelPrefab, transform, false);
        m_buildableDescriptionPanel = m_buildableDescriptionInstance.GetComponent<BuildInventoryDialogPanel>();
        m_buildableDescriptionInstanceRectTransform = m_buildableDescriptionPanel.GetComponent<RectTransform>();
        m_buildableDescriptionInstanceRectTransform.anchoredPosition = new Vector2(m_rectTransform.sizeDelta.x / 2.0f, m_rectTransform.sizeDelta.y);

        m_dragDropObject = null;
		m_characterStatus = GameObject.FindObjectOfType<CharacterStatus>();
    }

    private void ShowDialog()
    {
        if (m_slotItem != null)
        {
            if (m_buildableObject != null)
            {
                m_buildableDescriptionPanel.Enable();
            }
            else
            {
                m_descriptionPanel.Enable();
            }
        }
            
    }
    private void HideDialog()
    {
        if (m_slotItem != null)
        {
            if (m_buildableObject != null)
            {
                m_buildableDescriptionPanel.Disable();
            }
            else
            {
                m_descriptionPanel.Disable();
            }
        }
        }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ShowDialog();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideDialog();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (m_slotItem != null && m_slotItem.ObjectPrefab != null)
        {
			Miner miner = m_characterStatus.GetActiveMiner ();
			if (miner == null)
				return;
			
			if (miner.CheckRecipeForBuildableObject (m_slotItem.ObjectPrefab)) {
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint (eventData.position);
				m_dragDropObject = GameObject.Instantiate (m_slotItem.ObjectPrefab, mousePosition, Quaternion.identity);
				m_dragDropObjectController = m_dragDropObject.GetComponent<DragDropController> ();
				m_dragDropObjectController.StartDrag ();
			} else {
				// TODO: make inventory go red and play error sound
			}		
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (m_dragDropObject)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            m_dragDropObject.transform.position = mousePosition;
        }
            
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (m_dragDropObject)
        {
            m_dragDropObjectController.FinishDrag();
            m_dragDropObject = null;
            m_dragDropObjectController = null;
        }
    }
}
