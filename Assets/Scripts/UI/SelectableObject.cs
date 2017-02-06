using System;
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

    private GameObject m_dialogPanelPrefab;
    private GameObject m_selectorPrefab;

    private GameObject m_dialogInstance;
    private DialogPanel m_dialogPanel;

    private GameObject m_selectorInstance;
    private Selector m_selector;

    protected BoxCollider2D m_boxCollider;

    private bool m_dialogEnabled;

    public virtual void Awake()
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

        m_dialogEnabled = true;
    }

    private void ShowDialog()
    {
        
        m_dialogInstance.SetActive(true);
        m_selectorInstance.SetActive(true);
    }
    private void HideDialog()
    {
        m_dialogInstance.SetActive(false);
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
            ShowMenu();
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
            HideMenu();
    }

    public virtual void ShowMenu()
    { /* Empty */ }

    public virtual void HideMenu()
    { /* Empty */ }
}

public class MineableObject : SelectableObject
{
    public int MineAction_NumItems
    {
        get
        {
            return m_actionContextMenu.NumItems;
        }
        set
        {
            m_actionContextMenu.NumItems = value;
        }
    }
    public string MineAction_Info
    {
        get
        {
            return m_actionContextMenu.Info;
        }
        set
        {
            m_actionContextMenu.Info = value;
        }
    }
    public string MineAction_ActionName
    {
        get
        {
            return m_actionContextMenu.Action;
        }
        set
        {
            m_actionContextMenu.Action = value;
        }
    }

    private GameObject m_actionContextMenuPrefab;
    private GameObject m_actionContextMenuInstance;
    private ActionContextMenu m_actionContextMenu;

    public override void Awake()
    {
        base.Awake();

        m_actionContextMenuPrefab = Resources.Load("ActionContextMenu") as GameObject;

        m_actionContextMenuInstance = GameObject.Instantiate(m_actionContextMenuPrefab, transform, false);
        m_actionContextMenu = m_actionContextMenuInstance.GetComponent<ActionContextMenu>();

        m_actionContextMenuInstance.transform.localPosition = new Vector3(0.0f, 2.0f * m_boxCollider.bounds.extents.y + 5.0f, 0.0f);

        m_actionContextMenuInstance.SetActive(false);
    }

    public override void ShowMenu()
    {
        DisableDialog();
        m_actionContextMenuInstance.SetActive(true);
    }
    public override void HideMenu()
    {
        m_actionContextMenuInstance.SetActive(false);
        EnableDialog();
    }

}