using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerClickHandler {

    public float DoubleClickTime = 0.01f;
    private Miner m_miner;
    private CaveController m_caveController;

    private float m_lastClickTime;

    public enum InputEvent
    {
        LeftClick, DoubleLeftClick, Space
    }

    void Start()
    {
        m_miner = null;
        m_caveController = GameObject.FindObjectOfType<CaveController>();
        m_lastClickTime = Time.time;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_miner == null)
            return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // First check if the cave will handle this click
            if (m_caveController.HandlePointerClick(eventData.position))
                return;

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
    }

    void OnGUI()
    {
        if (m_miner != null)
        {
            if (Event.current.type == EventType.KeyDown)
            {
                m_miner.OnInputEvent(InputEvent.Space);
            }
        }
    }

    public void SetActiveMiner(Miner miner)
    {
        m_miner = miner;
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
            Application.Quit();
        }
    }
}
