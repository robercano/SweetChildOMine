using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Mineral : MonoBehaviour, IPointerClickHandler {
    public int m_currentAmount;

    public int Amount;
    public Sprite[] AmountSprites;

    private SpriteRenderer m_spriteRenderer;
    private int m_amountPerSprite;

    // Use this for initialization
    void Start()
    {
        m_currentAmount = Amount;
        m_amountPerSprite = Amount / AmountSprites.Length;

        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public int Extract(int amount)
    {
         if (amount >= m_currentAmount)
        {
            m_currentAmount -= amount;
        }
        else
        {
            amount = m_currentAmount;
            m_currentAmount = 0;
        }
        return amount;
    }

    void Update()
    {
        int index = AmountSprites.Length - ((m_currentAmount - 1) / m_amountPerSprite) - 1;

        if (index >= 0 && index < AmountSprites.Length)
            m_spriteRenderer.sprite = AmountSprites[index];
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            ShowMenu();
    }

    public void ShowMenu()
    {
        Debug.Log("Show menu");
    }
}
