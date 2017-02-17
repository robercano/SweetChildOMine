using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InputManager : MonoBehaviour, IPointerClickHandler {

    private Miner m_miner;
    private CaveController m_caveController;

    void Start()
    {
        m_miner = null;
        m_caveController = GameObject.FindObjectOfType<CaveController>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        
        if (m_miner != null)
        {
            // First check if the cave will handle this click
            if (m_caveController.HandlePointerClick(eventData.position) == false)
            {
                // Otherwise redirect to the miner
                m_miner.OnInputEvent(eventData);
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
    }
}
