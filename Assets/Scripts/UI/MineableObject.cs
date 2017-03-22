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
    public string MineableItem;

    protected int m_currentItems;

    private float m_remainingDamage;

    private MiningActionMenu m_actionContextMenu;

    private GameObject m_mineralDamagePrefab;

    private UIGameManager m_UIGameController;

    public override void Awake()
    {
        base.Awake();

        m_mineralDamagePrefab = Resources.Load("MineralDamagePopup") as GameObject;

        m_actionContextMenu = UIManager.Instance.CreateUIElement<MiningActionMenu>();
		m_actionContextMenu.FollowGameObject(this.gameObject);

        m_currentItems = MaxItems;

        m_remainingDamage = 0.0f;

        Item mineableItem = ItemManager.Instance.CreateItem(MineableItem);
        ItemManager.Instance.DestroyItem(mineableItem);

        m_actionContextMenu.Title = Name;
        m_actionContextMenu.ActionName = ActionName;
        m_actionContextMenu.OnAction = null;
        m_actionContextMenu.OnRetrieveMaxItems = OnRetrieveMaxItems;
        m_actionContextMenu.OnRetrieveCurrentItems = OnRetrieveCurrentItems;

        m_UIGameController = GameObject.Find("MainUI").GetComponent<UIGameManager>();

        m_onSelectedDelegate = ShowMenu;
        m_onDeselectedDelegate = CheckHideMenu;
    }

    void CheckHideMenu()
    {
        if (m_actionContextMenu.IsMouseOnObject() == false)
        {
            HideMenu();
        }
    }

    public void ShowMenu()
    {
        Miner miner = m_UIGameController.GetActiveMiner();
        if (miner != null)
        {
            m_actionContextMenu.OnAction = OnActionMine;

            int minerMaxItems = miner.MaterialInventory.RemainingAmount;
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

        Miner miner = m_UIGameController.GetActiveMiner();
        if (miner != null)
            minerMaxItems = miner.MaterialInventory.RemainingAmount;

        return Mathf.Min(minerMaxItems, m_currentItems);
    }
    protected int OnRetrieveCurrentItems()
    {
        return m_currentItems;
    }

    protected virtual void OnActionMine(int numItems)
    {
        HideMenu();

        Miner miner = m_UIGameController.GetActiveMiner();
        if (miner != null)
        {
            miner.MineMaterial(this, numItems);
        }
    }

    /* Public interface */
    public bool DoMine(float damage, int maxItems, int maxAmount, out Item extracted)
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
        if (amountToExtract > maxAmount)
        {
            amountToExtract = maxAmount;
            continueMining = false;
        }
        if (amountToExtract > m_currentItems)
        {
            amountToExtract = m_currentItems;
            continueMining = false;
        }

        if (amountToExtract == 0)
            return false;

        extracted = ItemManager.Instance.CreateItem(MineableItem);
        extracted.Amount = amountToExtract;

        m_currentItems -= extracted.Amount;
        m_remainingDamage -= (extracted.Amount * DamagePerItem);

        if (extracted.Amount > 0)
        {
            GameObject mineralDamageInstance = GameObject.Instantiate(m_mineralDamagePrefab);
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