using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

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

            if (m_slotItem != null)
            {
                m_slotImage.sprite = value.Avatar;
                m_slotImage.color = Color.white;
                m_descriptionPanel.Title = m_slotItem.Name;
                m_descriptionPanel.Description = m_slotItem.Description;
            }
            else
            {
                m_slotImage.color = Color.clear;
                m_descriptionPanel.Title = "";
                m_descriptionPanel.Description = "";
            }
        }
    }
    private Item m_slotItem;
    private Image m_slotImage;
    private RectTransform m_rectTransform;

    private GameObject m_descriptionPanelPrefab;
    private GameObject m_descriptionInstance;
    private InventoryDescriptionPanel m_descriptionPanel;
    private RectTransform m_descriptionInstanceRectTransform;

    private GameObject m_dragDropObject;
    private DragDropController m_dragDropObjectController;

    // Use this for initialization
    void Start()
    {
        m_slotImage = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();

        m_descriptionPanelPrefab = Resources.Load("InventoryDescriptionPanel") as GameObject;
        m_descriptionInstance = GameObject.Instantiate(m_descriptionPanelPrefab, transform, false);
        m_descriptionPanel = m_descriptionInstance.GetComponent<InventoryDescriptionPanel>();
        m_descriptionInstanceRectTransform = m_descriptionInstance.GetComponent<RectTransform>();
        m_descriptionInstanceRectTransform.anchoredPosition = new Vector2(m_rectTransform.sizeDelta.x / 2.0f, m_rectTransform.sizeDelta.y);

        m_dragDropObject = null;
    }

    private void ShowDialog()
    {
        if (m_slotItem != null)
        {
            m_descriptionPanel.Enable();
        }
            
    }
    private void HideDialog()
    {
        if (m_slotItem != null)
            m_descriptionPanel.Disable();
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
			// TODO: Check if recipe is valid

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
            m_dragDropObject = GameObject.Instantiate(m_slotItem.ObjectPrefab, mousePosition, Quaternion.identity);
            m_dragDropObjectController = m_dragDropObject.GetComponent<DragDropController>();
            m_dragDropObjectController.StartDrag();
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
