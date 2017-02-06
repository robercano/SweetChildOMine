using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectableObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string Name;

    private GameObject m_dialogPanelPrefab;
    private GameObject m_selectorPrefab;

    private GameObject m_dialogInstance;
    private DialogPanel m_dialogPanel;

    private GameObject m_selectorInstance;
    private Selector m_selector;

    private BoxCollider2D m_boxCollider;

    public void Awake()
    {
        m_dialogPanelPrefab = Resources.Load("DialogPanel") as GameObject;
        m_selectorPrefab = Resources.Load("Selector") as GameObject;

        m_boxCollider = GetComponent<BoxCollider2D>();

        m_dialogInstance = GameObject.Instantiate(m_dialogPanelPrefab, transform, false);
        m_dialogPanel = m_dialogInstance.GetComponent<DialogPanel>();

        m_selectorInstance = GameObject.Instantiate(m_selectorPrefab, transform, false);
        m_selector = m_selectorInstance.GetComponent<Selector>();

        m_dialogInstance.transform.localPosition = new Vector3(0.0f, 2.0f * m_boxCollider.bounds.extents.y + 5.0f, 0.0f);

        m_dialogInstance.SetActive(false);
        m_dialogPanel.SetText(Name);

        m_selectorInstance.SetActive(false);
        m_selector.SetBounds(m_boxCollider.bounds);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        m_dialogInstance.SetActive(true);
        m_selectorInstance.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        m_dialogInstance.SetActive(false);
        m_selectorInstance.SetActive(false);
    }

}
