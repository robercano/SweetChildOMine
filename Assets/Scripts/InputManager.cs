using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerClickHandler {

    private Miner m_miner;

    void Start()
    {
        m_miner = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (m_miner != null)
            m_miner.OnInputEvent(eventData);
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
                m_miner.DisableUpdateStatus();
                m_miner = null;
            }
        }
    }

    void OnGUI()
    {
        Event e = Event.current;

        if (e.isKey)
            Debug.Log("Received key event");
    }
}
