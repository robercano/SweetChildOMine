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
    public GameObject MineableItem;

    protected int m_currentItems;

    private float m_remainingDamage;

    private int m_mineableItemWeight;

    private GameObject m_actionContextMenuPrefab;
    private GameObject m_actionContextMenuInstance;
    private MiningContextMenu m_actionContextMenu;

    private GameObject m_mineralDamagePrefab;

    private CharacterStatus m_characterStatus;

    public override void Awake()
    {
        base.Awake();

        m_actionContextMenuPrefab = Resources.Load("UI/MiningContextMenu") as GameObject;

        m_mineralDamagePrefab = Resources.Load("MineralDamagePopup") as GameObject;

        m_actionContextMenuInstance = GameObject.Instantiate(m_actionContextMenuPrefab, transform, false);
        m_actionContextMenu = m_actionContextMenuInstance.GetComponent<MiningContextMenu>();

        m_actionContextMenuInstance.transform.position = new Vector3(gameObject.transform.position.x,
                                                                     m_spriteRenderer.bounds.max.y + UIGlobals.PegDistanceToObject, 
                                                                     0.0f);

        m_currentItems = MaxItems;

        m_remainingDamage = 0.0f;

        m_mineableItemWeight = MineableItem.GetComponent<Item>().WeightPerUnit;

        m_actionContextMenu.Title = Name;
        m_actionContextMenu.ActionName = ActionName;
        m_actionContextMenu.OnAction = null;
        m_actionContextMenu.OnRetrieveMaxItems = OnRetrieveMaxItems;
        m_actionContextMenu.OnRetrieveCurrentItems = OnRetrieveCurrentItems;

        m_characterStatus = GameObject.Find("CharacterStatus").GetComponent<CharacterStatus>();

        m_onSelectedDelegate = ShowMenu;
        m_onDeselectedDelegate = HideMenu;
    }

    public void ShowMenu()
    {
        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
        {
            m_actionContextMenu.OnAction = OnActionMine;

            int minerMaxItems = miner.MaterialInventory.RemainingWeight / m_mineableItemWeight;
            m_actionContextMenu.SelectedNumItems = minerMaxItems;
        }
        else
        {
            m_actionContextMenu.SelectedNumItems = 0;
        }

        DisableDialog();
        m_actionContextMenu.Enable();
    }
    public void HideMenu()
    {
        m_actionContextMenu.Disable();
        EnableDialog();
    }

    protected int OnRetrieveMaxItems()
    {
        int minerMaxItems = 0;

        Miner miner = m_characterStatus.GetActiveMiner();
        if (miner != null)
            minerMaxItems = miner.MaterialInventory.RemainingWeight / m_mineableItemWeight;

        return Mathf.Min(minerMaxItems, m_currentItems);
    }
    protected int OnRetrieveCurrentItems()
    {
        return m_currentItems;
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
    public bool DoMine(float damage, int maxItems, int maxWeight, out Item extracted)
    {
        bool continueMining = true;
        extracted = null;

        if (m_currentItems == 0)
            return false;

        m_remainingDamage += damage;

        // Calculate expected number of items to extract
        int amountToExtract = Mathf.FloorToInt(m_remainingDamage / DamagePerItem);

        // Then adjust to the different limitations: maximum number of items allowed,
        // maximum weight allowed and maximum available number of items
        if (amountToExtract > maxItems)
        {
            amountToExtract = maxItems;
            continueMining = false;
        }
        if ((amountToExtract * m_mineableItemWeight) > maxWeight)
        {
            amountToExtract = maxWeight / m_mineableItemWeight;
            continueMining = false;
        }
        if (amountToExtract > m_currentItems)
        {
            amountToExtract = m_currentItems;
            continueMining = false;
        }

        if (amountToExtract == 0)
            return false;

        extracted = GameObject.Instantiate(MineableItem).GetComponent<Item>();
        extracted.Amount = amountToExtract;

        m_currentItems -= extracted.Amount;
        m_remainingDamage -= (extracted.Amount * DamagePerItem);

        if (extracted.Amount > 0)
        {
            GameObject mineralDamageInstance = GameObject.Instantiate(m_mineralDamagePrefab, transform, false);
            mineralDamageInstance.transform.position = new Vector2(gameObject.transform.position.x, m_spriteRenderer.bounds.max.y + 5.0f);

            MineralDamagePopup mineralDamagePopup = mineralDamageInstance.GetComponent<MineralDamagePopup>();

            mineralDamagePopup.Color = DamageColor;
            mineralDamagePopup.Speed = 10.0f;
            mineralDamagePopup.Seconds = 2.0f;
            mineralDamagePopup.UnitsExtracted = extracted.Amount;
        }

        return continueMining;
    }
}