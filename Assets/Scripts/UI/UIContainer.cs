using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;

public class UIContainer : MonoBehaviour, IDropHandler {

	public string Title;
    public bool EnablePeg;
    public bool EnableAmount;

    private Canvas m_canvas;

    private Text m_titleText;

    private ContainerSlot[] m_slots;
    private Inventory m_inventory;
    private AudioClip m_errorSound;
    private Image m_background;

    private AudioClip m_containerPop;

    void Awake()
    {
        m_canvas = GetComponent<Canvas>();

		m_slots = GetComponentsInChildren<ContainerSlot>();
		foreach (ContainerSlot slot in m_slots) {
			slot.ParentContainer = this;
		}

		m_inventory = null;

		m_errorSound = Resources.Load("Sounds/InventoryError") as AudioClip;
        m_containerPop = Resources.Load("Sounds/InventoryPopIn") as AudioClip;
        m_background = transform.FindDeepChild("Body").GetComponent<Image>();
    }

	void Start()
	{
		Transform titleBar = transform.FindDeepChild("TitleBar");
		if (Title != "")
		{
			titleBar.gameObject.SetActive(true);
			m_titleText = transform.FindDeepChild("Title").GetComponent<Text>();
			m_titleText.text = Title;
		}
		else
		{
			titleBar.gameObject.SetActive(false);
		}

		Transform peg = transform.FindDeepChild("Peg");
		peg.gameObject.SetActive(EnablePeg);

        if (EnableAmount == false)
        {
            foreach (ContainerSlot slot in m_slots)
            {
                slot.DisableAmount();
            }
        }
    }

    public void InventoryUpdate()
    {
        for (int i = 0; i < m_inventory.GetMaxSlots(); ++i)
        {
            Item item = m_inventory.GetItemAtSlot(i);
			if (SetSlot (i, item) == false) {
				Debug.Log ("ERROR setting slot");
			}
        }
    }

    IEnumerator CO_SignalError()
    {
        AudioSource.PlayClipAtPoint(m_errorSound, Camera.main.transform.position);

        int errorSteps = 20;
        int errorRepetition = 2;

        for (int i = 0; i < errorSteps; ++i)
        {
            float intensity = Mathf.Abs(Mathf.Cos(i * errorRepetition * Mathf.PI / errorSteps));

            m_background.color = new Color(1, intensity, intensity);
            yield return new WaitForSeconds(0.05f);
        }
        m_background.color = Color.white;
    }

    #region /* Public interface */
    public void SetWorldUI(bool worldUI)
    {
        if (worldUI)
        {
            m_canvas.renderMode = RenderMode.WorldSpace;
            //transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        }
        else
        {
            m_canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            transform.localScale = Vector3.one;
        }
    }

    public void SetInventory(Inventory inventory)
    {
        m_inventory = inventory;
        m_inventory.OnInventoryUpdate += InventoryUpdate;
        InventoryUpdate();
    }

    public bool SetSlot(int slot, Item item)
    {
        if (slot < 0 || slot >= m_slots.Length)
            return false;

        m_slots[slot].SlotItem = item;
        return true;
    }

    public bool SetSlotShortcut(int slot, char shortcut)
    {
        if (slot < 0 || slot >= m_slots.Length)
            return false;


        m_slots[slot].Shortcut = shortcut.ToString().ToUpper()[0];
        return true;
    }

    public void ClearAllSlots()
    {
        for (int i = 1; i < m_slots.Length; ++i)
            SetSlot(i, null);
    }

    public void ClearTitle()
    {
        m_titleText.text = "";
    }

    public void ClearAll()
    {
        ClearAllSlots();
        ClearTitle();
    }

	public bool TransferItem(Item item)
	{
		if (m_inventory == null) {
			return false;
		}

		if (m_inventory.TransferItem (item) == false)
        {
            return false;
        }

        UIController.Instance.Refresh();

        return true;
	}

    public bool AddItem(Item item)
    {
        if (m_inventory == null)
        {
            return false;
        }

        if (m_inventory.AddItem(item) == false)
        {
            return false;
        }

        UIController.Instance.Refresh();

        return true;
    }

    public void SignalError(string itemName)
    {
        bool firstTime = true;

        foreach (ContainerSlot slot in m_slots)
        {
			if (slot.SlotItem != null && slot.SlotItem.Name == itemName)
            {
                if (firstTime)
                {
                    AudioSource.PlayClipAtPoint(m_errorSound, Camera.main.transform.position);
                }
                slot.SignalEror();
                return;
            }
        }
        StartCoroutine(CO_SignalError());
    }

	public void OnDrop(PointerEventData eventData)
	{
		if (eventData.pointerDrag == null || m_inventory == null) {
			return;
		}

		ContainerSlot slot = eventData.pointerDrag.GetComponent<ContainerSlot> ();
		Item item = slot.SlotItem;

		if (slot.ParentContainer.TransferItem (item)) {
			if (m_inventory.AddItem (item) == false)
            {
                // Not all the items were added, add the rest back
                Assert.IsTrue(slot.ParentContainer.AddItem(item));
            }
			item.Hide ();
            AudioSource.PlayClipAtPoint(m_containerPop, Camera.main.transform.position);
		}

        UIController.Instance.Refresh();
	}

    #endregion  /* Public interface */
}
