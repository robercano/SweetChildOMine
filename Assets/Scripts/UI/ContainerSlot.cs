﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class ContainerSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler,
                                            IBeginDragHandler, IDragHandler, IEndDragHandler {

	public UIContainer ParentContainer;

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

                if (m_slotAmount != null)
                {
                    m_slotAmount.text = "";
                }
                return;
            }

            // Set common fields
            m_slotImage.sprite = m_slotItem.InventoryAvatar;
            if (m_slotAmount != null)
            {
                m_slotAmount.text = "x" + m_slotItem.Amount.ToString();
            }

            // Non-buildable object
            if (m_slotItem.BuildablePrefab == null)
            {
                m_slotImage.color = Color.white;
                m_descriptionPanel.Title = m_slotItem.Name;
                m_descriptionPanel.Description = m_slotItem.Description;
                return;
            }

            // Buildable object
            m_buildableObject = m_slotItem.BuildablePrefab.GetComponent<BuildableObject>();
            Assert.IsNotNull(m_buildableObject);

            Miner miner = m_UIController.GetActiveMiner();
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
            if (m_buildableDescriptionPanel.Material != null)
                ItemManager.Instance.DestroyItem(m_buildableDescriptionPanel.Material);

            m_buildableDescriptionPanel.Title = m_slotItem.Name;
            m_buildableDescriptionPanel.Amount = m_buildableObject.Recipe[0].Amount;
            m_buildableDescriptionPanel.Material = ItemManager.Instance.CreateItem(m_buildableObject.Recipe[0].Ingredient);
        }
    }
    public char Shortcut
    {
        set
        {
            if (m_slotShortcut != null)
            {
                m_slotShortcut.text = value.ToString();
            }
        }
        get
        {
            if (m_slotShortcut != null)
            {
                return m_slotShortcut.text[0];
            }
            else
            {
                return ' ';
            }
        }
    }
    public bool EnableDragDrop;

    private Item m_slotItem;
    private Image m_slotImage;
    private Text m_slotShortcut;
    private Text m_slotAmount;

    private RectTransform m_rectTransform;

    private GameObject m_descriptionPanelPrefab;
    private GameObject m_descriptionInstance;
    private InventoryDescriptionPanel m_descriptionPanel;
    private RectTransform m_descriptionInstanceRectTransform;

    private GameObject m_buildableDescriptionPanelPrefab;
    private GameObject m_buildableDescriptionInstance;
    private BuildInventoryDialogPanel m_buildableDescriptionPanel;    
    private RectTransform m_buildableDescriptionInstanceRectTransform;

	private UIController m_UIController;
    private BuildableObject m_buildableObject;

    // Use this for initialization
    void Awake()
    {
        m_slotImage = GetComponent<Image>();
        m_rectTransform = GetComponent<RectTransform>();

        m_descriptionPanelPrefab = Resources.Load("UI/InventoryDescriptionPanel") as GameObject;
        m_descriptionInstance = GameObject.Instantiate(m_descriptionPanelPrefab, transform, false);
        m_descriptionPanel = m_descriptionInstance.GetComponent<InventoryDescriptionPanel>();
        m_descriptionInstanceRectTransform = m_descriptionInstance.GetComponent<RectTransform>();
        m_descriptionInstanceRectTransform.anchoredPosition = new Vector2(m_rectTransform.sizeDelta.x / 2.0f, 
                                                                          m_rectTransform.sizeDelta.y + UIGlobals.PegDistanceToObject);

        m_buildableDescriptionPanelPrefab = Resources.Load("UI/BuildInventoryDialogPanel") as GameObject;
        m_buildableDescriptionInstance = GameObject.Instantiate(m_buildableDescriptionPanelPrefab, transform, false);
        m_buildableDescriptionPanel = m_buildableDescriptionInstance.GetComponent<BuildInventoryDialogPanel>();
        m_buildableDescriptionInstanceRectTransform = m_buildableDescriptionPanel.GetComponent<RectTransform>();
        m_buildableDescriptionInstanceRectTransform.anchoredPosition = new Vector2(m_rectTransform.sizeDelta.x / 2.0f, 
                                                                                   m_rectTransform.sizeDelta.y + UIGlobals.PegDistanceToObject);

        m_UIController = GameObject.Find("MainUI").GetComponent<UIController>();

        Transform shortcutGO = transform.FindDeepChild("Shortcut");
        if (shortcutGO != null)
        {
            m_slotShortcut = shortcutGO.GetComponent<Text>();
        }
        else
        {
            m_slotShortcut = null;
        }

        Transform amountGO = transform.FindDeepChild("Amount");
        if (amountGO != null)
        {
            m_slotAmount = amountGO.GetComponent<Text>();
        }
        else
        {
            m_slotAmount = null;
        }

        SlotItem = null;
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
        if (m_slotItem == null || EnableDragDrop == false)
            return;
            
		m_slotItem.OnBeginDrag (eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
		if (m_slotItem == null || EnableDragDrop == false)
			return;

		m_slotItem.OnDrag (eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
		if (m_slotItem == null || EnableDragDrop == false)
			return;

		m_slotItem.OnEndDrag (eventData);
	}

    public void SignalEror()
    {
        StartCoroutine(CO_SignalError());
    }

    IEnumerator CO_SignalError()
    {
        int errorSteps = 20;
        int errorRepetition = 2;

        for (int i = 0; i < errorSteps; ++i)
        {
            float intensity = Mathf.Abs(Mathf.Cos(i * errorRepetition * Mathf.PI / errorSteps));

            m_slotImage.color = new Color(1, intensity, intensity);
            yield return new WaitForSeconds(0.05f);
        }
        m_slotImage.color = Color.white;
    }

    public void DisableAmount()
    {
        m_slotAmount.enabled = false;
    }

    public void EnableAmount()
    {
        m_slotAmount.enabled = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        throw new NotImplementedException();
    }
}
