﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public string Name;
    public bool MouseOnObject
    {
        get; private set;
    }

    public delegate void OnSelectectedDelegate();
    public OnSelectectedDelegate m_onSelectedDelegate;

    public delegate void OnDeselectedDelegate();
    public OnDeselectedDelegate m_onDeselectedDelegate;

    private GameObject m_dialogPanelPrefab;
    private GameObject m_selectorPrefab;

    private GameObject m_dialogInstance;
    private DialogPanel m_dialogPanel;

    private GameObject m_selectorInstance;
    private Selector m_selector;

    //protected BoxCollider2D m_boxCollider;
    protected SpriteRenderer m_spriteRenderer;

    private bool m_dialogEnabled;

    public virtual void Awake()
    {
        m_dialogPanelPrefab = Resources.Load("UI/DialogPanel") as GameObject;
        m_selectorPrefab = Resources.Load("Selector") as GameObject;

       // m_boxCollider = GetComponent<BoxCollider2D>();
        m_spriteRenderer = GetComponent<SpriteRenderer>();

        m_dialogInstance = GameObject.Instantiate(m_dialogPanelPrefab, transform, false);
        m_dialogPanel = m_dialogInstance.GetComponent<DialogPanel>();

        m_selectorInstance = GameObject.Instantiate(m_selectorPrefab, transform, false);
        m_selector = m_selectorInstance.GetComponent<Selector>();

        m_dialogInstance.transform.position = new Vector3(gameObject.transform.position.x, 
                                                          m_spriteRenderer.bounds.max.y + UIGlobals.PegDistanceToObject,
                                                          0.0f);

        m_dialogPanel.DisableImmediate();
        m_dialogPanel.SetText(Name);

        m_selectorInstance.SetActive(false);
        m_selector.SetBounds(m_spriteRenderer.bounds);

        m_dialogEnabled = true;

        m_onSelectedDelegate = null;
        m_onDeselectedDelegate = null;
    }

    private void ShowDialog()
    {
        m_dialogPanel.Enable();
        m_selectorInstance.SetActive(true);
    }
    private void HideDialog()
    {
        m_dialogPanel.Disable();
        m_selectorInstance.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MouseOnObject = true;

        if (m_dialogEnabled)
            ShowDialog();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MouseOnObject = false;

        HideDialog();
    }

    void IPointerClickHandler.OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            if (m_onSelectedDelegate != null)
                m_onSelectedDelegate();
    }

    public void EnableDialog()
    {
        m_dialogEnabled = true;
    }

    public void DisableDialog()
    {
        m_dialogEnabled = false;
        HideDialog();
    }

    public virtual void Update()
    {
        if (Input.GetMouseButton(0) && (MouseOnObject == false))
            if (m_onDeselectedDelegate != null)
                m_onDeselectedDelegate();
    }
}