using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropController : MonoBehaviour {

    private bool m_active;
    private SpriteRenderer m_spriteRenderer;
    
    // Use this for initialization
    void Awake()
    {
        m_active = false;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_active)
        {
            // TODO: Check if position is suitable
        }
    }

    public void StartDrag()
    {
        m_spriteRenderer.sortingLayerName = "UI";
        m_active = true;
    }

    public void FinishDrag()
    {
        m_spriteRenderer.sortingLayerName = "Items";
        m_active = false;

        // TODO: If position is suitable let the object leave, otherwise destroy the object
        Destroy(gameObject);
    }
 }
