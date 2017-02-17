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
                m_dialogPanel.SetText(m_slotItem.Name + "\n" + m_slotItem.Description);
            }
            else
            {
                m_slotImage.color = Color.clear;
                m_dialogPanel.SetText(" ");
            }
        }
    }
    private Item m_slotItem;
    private Image m_slotImage;
    private RectTransform m_rectTransform;

    private GameObject m_dialogPanelPrefab;
    private GameObject m_dialogInstance;
    private DialogPanel m_dialogPanel;

    private GameObject m_dragDropObject;
    private DragDropController m_dragDropObjectController;

    // Use this for initialization
    void Start()
    {
        m_slotImage = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();

        m_dialogPanelPrefab = Resources.Load("DialogPanel") as GameObject;
        m_dialogInstance = GameObject.Instantiate(m_dialogPanelPrefab, transform, false);
        m_dialogPanel = m_dialogInstance.GetComponent<DialogPanel>();

        m_dialogInstance.transform.position = new Vector3(gameObject.transform.position.x, m_rectTransform.sizeDelta.y + 5.0f, 0.0f);

        m_dragDropObject = null;
    }

    private void ShowDialog()
    {
        if (m_slotItem != null)
            m_dialogPanel.Enable();
    }
    private void HideDialog()
    {
        if (m_slotItem != null)
            m_dialogPanel.Disable();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        //ShowDialog();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //HideDialog();
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
