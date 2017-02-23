using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIContainer : MonoBehaviour {

    public string Title;
    public bool EnablePeg;

    private Canvas m_canvas;

    private Text m_titleText;

    private ContainerSlot[] m_slots;
    private Inventory m_inventory;
    private AudioClip m_errorSound;
    private Image m_background;

    void Awake()
    {
        m_canvas = GetComponent<Canvas>();
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

        m_slots = GetComponentsInChildren<ContainerSlot>();

        m_inventory = null;

        m_errorSound = Resources.Load("Sounds/InventoryError") as AudioClip;

        m_background = transform.FindDeepChild("Body").GetComponent<Image>();
    }

    void InventoryUpdate()
    {
        for (int i = 0; i < m_inventory.GetMaxSlots(); ++i)
        {
            Item item = m_inventory.GetItemAtSlot(i);
			Debug.Log ("SetSlot pre: " + m_inventory);
			if (SetSlot (i, item) == false) {
				Debug.Log ("ERROR setting slot");
			}
			Debug.Log ("SetSlot post");
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
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
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
        m_inventory.m_inventoryUpdateDelegate = InventoryUpdate;
        InventoryUpdate();
    }

    public bool SetSlot(int slot, Item item, char shortcut = ' ')
    {
        if (slot < 0 || slot >= m_slots.Length)
            return false;

        m_slots[slot].SlotItem = item;
        m_slots[slot].Shortcut = shortcut;
        return true;
    }

    public void ClearAllSlots()
    {
        for (int i = 1; i < m_slots.Length; ++i)
            SetSlot(i, null);
    }

    public bool SetShortcut(int slot, char shortcut)
    {
        slot++;

        if (slot < 1 || slot >= m_slots.Length)
            return false;

        //m_UITexts[slot].text = shortcut.ToString();

        return true;
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
    #endregion  /* Public interface */
}
