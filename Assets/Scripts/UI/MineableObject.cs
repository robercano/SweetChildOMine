using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MineableObject : SelectableObject
{
    public int MaxItems;
    public float DamagePerItem;
    public Color DamageColor;
    public string ActionName;

    protected int m_currentItems;

    private float m_remindingDamage;

    private GameObject m_actionContextMenuPrefab;
    private GameObject m_actionContextMenuInstance;
    private ActionContextMenu m_actionContextMenu;

    private GameObject m_mineralDamagePrefab;

    private CharacterStatus m_characterStatus;

    public override void Awake()
    {
        base.Awake();

        m_actionContextMenuPrefab = Resources.Load("ActionContextMenu") as GameObject;

        m_mineralDamagePrefab = Resources.Load("MineralDamagePopup") as GameObject;

        m_actionContextMenuInstance = GameObject.Instantiate(m_actionContextMenuPrefab, transform, false);
        m_actionContextMenu = m_actionContextMenuInstance.GetComponent<ActionContextMenu>();

        m_actionContextMenuInstance.transform.localPosition = new Vector3(0.0f, 2.0f * m_boxCollider.bounds.extents.y + 5.0f, 0.0f);

        m_currentItems = MaxItems;

        m_remindingDamage = 0.0f;

        m_actionContextMenu.Title = Name;
        m_actionContextMenu.ActionName = ActionName;
        m_actionContextMenu.OnAction = OnActionMine;

        m_characterStatus = GameObject.Find("CharacterStatus").GetComponent<CharacterStatus>();
    }

    public override void ShowMenu()
    {
        DisableDialog();

        m_actionContextMenu.MaxNumItems = m_currentItems;
        m_actionContextMenu.Enable();
    }
    public override void HideMenu()
    {
        m_actionContextMenu.Disable();
        EnableDialog();
    }

    protected virtual void OnActionMine(int numItems)
    {
        HideMenu();

        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
        {
            miner.MineMaterial(this, numItems);
        }
    }

    /* Public interface */
    public bool DoMine(float damage, int maxIems, out int extracted)
    {
        extracted = 0;

        if (m_currentItems == 0)
            return false;

        m_remindingDamage += damage;

        extracted = Mathf.FloorToInt(m_remindingDamage / DamagePerItem);
        if (extracted > maxIems)
        {
            extracted = maxIems;
            m_remindingDamage = 0.0f;
        }
        else
        {
            m_remindingDamage -= extracted * DamagePerItem;
        }

        if (m_currentItems > extracted)
        {
            m_currentItems -= extracted;
        }
        else
        {
            extracted = m_currentItems;
            m_remindingDamage = 0.0f;
            m_currentItems = 0;
        }

        if (extracted > 0)
        {
            GameObject mineralDamageInstance = GameObject.Instantiate(m_mineralDamagePrefab, transform, false);
            mineralDamageInstance.transform.localPosition = new Vector2(0.0f, 2.0f * m_boxCollider.bounds.extents.y + 5.0f);

            MineralDamagePopup mineralDamagePopup = mineralDamageInstance.GetComponent<MineralDamagePopup>();

            mineralDamagePopup.Color = DamageColor;
            mineralDamagePopup.Speed = 10.0f;
            mineralDamagePopup.Seconds = 2.0f;
            mineralDamagePopup.UnitsExtracted = extracted;
        }

        return true;
    }
}