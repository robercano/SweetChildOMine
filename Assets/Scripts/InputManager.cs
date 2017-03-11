using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using com.kleberswf.lib.core;

public class InputManager : Singleton<InputManager>, IPointerClickHandler {

    public float DoubleClickTime = 0.01f;
    private Miner m_miner;
    private CaveController m_caveController;

    private UIManager m_UIController;
    private UIContainer m_weaponContainer;
    private UIContainer m_buildContainer;

    private float m_lastClickTime;

    private DragDropInterface m_attachedObject;
    private bool m_attachedObjectFirstTime = true;
    private SpriteRenderer m_attachedObjectSpriteRenderer;

    public enum InputEvent
    {
        LeftClick, DoubleLeftClick, Space
    }

    protected override void Awake()
    {
        base.Awake();

        m_miner = null;
        m_caveController = GameObject.FindObjectOfType<CaveController>();
        m_lastClickTime = Time.time;

        m_UIController = GameObject.Find("MainUI").GetComponent<UIManager>();

        m_weaponContainer = GameObject.Find("WeaponContainer").GetComponent<UIContainer>();
        m_buildContainer = GameObject.Find("BuildContainer").GetComponent<UIContainer>();
    }

    void Start()
    {
        m_weaponContainer.SetSlotShortcut(0, '1');
        m_weaponContainer.SetSlotShortcut(1, '2');
        m_weaponContainer.SetSlotShortcut(2, '3');
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_miner == null)
            return;

        switch (eventData.button)
        {
            case PointerEventData.InputButton.Right:
                m_caveController.HandlePointerClick(eventData.position);
                return;
            case PointerEventData.InputButton.Left:
                if (m_attachedObject != null)
                {
                    eventData.pointerDrag = m_attachedObject.gameObject;
                    m_attachedObject.OnEndDrag(eventData);
                    m_attachedObject = null;
                }
                else
                {
                    m_caveController.CancelPointerClick();

                    if ((Time.time - m_lastClickTime) < DoubleClickTime)
                    {
                        m_miner.OnInputEvent(InputEvent.DoubleLeftClick);
                    }
                    else
                    {
                        m_miner.OnInputEvent(InputEvent.LeftClick);
                        m_lastClickTime = Time.time;
                    }
                }
                break;
        }
    }

    void OnGUI()
    {
        if (m_miner != null)
        {
            if (Event.current.type == EventType.KeyDown &&
				Event.current.keyCode == KeyCode.Space)
            {
                m_miner.OnInputEvent(InputEvent.Space);
            }
        }
        if (m_attachedObject != null)
        {
            PointerEventData eventData = new PointerEventData(EventSystem.current);

            eventData.position = Input.mousePosition;

            eventData.pointerDrag = m_attachedObject.gameObject;

            if (m_attachedObjectFirstTime)
            {
                m_attachedObjectFirstTime = false;
                m_attachedObject.OnBeginDrag(eventData);
            }
            m_attachedObject.OnDrag(eventData);
        }
    }

    public void SetActiveMiner(Miner miner)
    {
        m_miner = miner;

        m_UIController.SetActiveMiner(miner);
    }

    public void AttachObjectToMouse(DragDropInterface obj)
    {
        m_attachedObject = obj;
        m_attachedObjectFirstTime = true;

        m_attachedObjectSpriteRenderer = obj.GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Return))
        {
            if (m_miner != null)
            {
                m_miner.DeactivateMiner();
                m_miner = null;
            }
        }
        if (Input.GetKey(KeyCode.Escape))
        {
            if (m_attachedObject != null)
            {
                PointerEventData eventData = new PointerEventData(EventSystem.current);

                eventData.pointerDrag = null;
                eventData.position = Input.mousePosition;

                m_attachedObject.OnEndDrag(eventData);
                m_attachedObject = null;
            }
        }
    }
}
