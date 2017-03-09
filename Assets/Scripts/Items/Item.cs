using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Item : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public string Name;
    public Sprite Avatar
    {
        get
        {
            return m_spriteRenderer.sprite;
        }
    }
    public Sprite InventoryAvatar
    {
        get
        {
            if (m_inventoryAvatar == null)
            {
                return Avatar;
            }
            else
            {
                return m_inventoryAvatar;
            }
        }
        set
        {
            m_inventoryAvatar = value;
        }
    }
    public Sprite m_inventoryAvatar;
    public string Description;
    public int Amount
    {
        get
        {
            return m_amount;
        }
        set
        {
            m_amount = value;

            if (AmountAvatars.Length > 0)
            {
                int spriteIndex = Mathf.Min(m_amount / 10, AmountAvatars.Length - 1);
                m_spriteRenderer.sprite = AmountAvatars[AmountAvatars.Length - 1 - spriteIndex];
            }
        }
    }
    public Sprite[] AmountAvatars;

    public GameObject BuildablePrefab;
	private SpriteRenderer m_spriteRenderer;

	private UIContainer m_materialInventory;

	private GameObject m_dragDropObject;
	private DragDropController m_dragDropObjectController;

	private UIController m_UIController;
    private int m_amount;
    private AudioClip m_popSound;

    protected virtual void Awake()
    {
		m_spriteRenderer = GetComponent<SpriteRenderer>();

		m_materialInventory = GameObject.Find("InventoryContainer").GetComponent<UIContainer>();

		m_UIController = GameObject.Find("MainUI").GetComponent<UIController>();

		m_dragDropObject = null;
		m_dragDropObjectController = null;

        m_popSound = Resources.Load("Sounds/InventoryPopOut") as AudioClip;

        Hide();
    }

    #region /* Public interface */
    public void Show()
    {
        m_spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        m_spriteRenderer.enabled = false;
    }

	public void OnBeginDrag(PointerEventData eventData)
	{
		if (BuildablePrefab != null)
		{
			Miner miner = m_UIController.GetActiveMiner ();
            if (miner == null)
            {
                eventData.pointerDrag = null;
                return;
            }

			// TODO: Allocate resources for buildable here
			if (miner.CheckRecipeForBuildableObject (BuildablePrefab)) {
				Vector2 mousePosition = Camera.main.ScreenToWorldPoint (eventData.position);
				m_dragDropObject = GameObject.Instantiate (BuildablePrefab, mousePosition, Quaternion.identity);
				m_dragDropObjectController = m_dragDropObject.GetComponent<DragDropController> ();
				m_dragDropObjectController.StartDrag ();

                AudioSource.PlayClipAtPoint(m_popSound, Camera.main.transform.position);
			} else {
				// TODO: optimize this
				BuildableObject buildable = BuildablePrefab.GetComponent<BuildableObject>();
				foreach (BuildableObject.RecipeItem recipeItem in buildable.Recipe)
				{
					m_materialInventory.SignalError(recipeItem.Ingredient);
				}
                eventData.pointerDrag = null;
            }		
		}
		else
		{
			m_dragDropObject = gameObject;
            AudioSource.PlayClipAtPoint(m_popSound, Camera.main.transform.position);
            Show();
		}
	}

	public void OnDrag(PointerEventData eventData)
	{
		if (m_dragDropObject == null) {
			return;
		}

		Vector2 mousePosition = Camera.main.ScreenToWorldPoint(eventData.position);
		m_dragDropObject.transform.position = mousePosition;
	}

	public void OnEndDrag(PointerEventData eventData)
	{
		if (m_dragDropObject == null) {
			return;
		}
		if (m_dragDropObjectController != null) {
			m_dragDropObjectController.FinishDrag ();
		}
		if (m_dragDropObject == gameObject) {
			Hide ();
		}
		m_dragDropObject = null;
		m_dragDropObjectController = null;
	}
    #endregion /* Public interface */
};
