using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Mineral : MineableObject {
    public Sprite[] AmountSprites;

    private SpriteRenderer m_spriteRenderer;
    private int m_amountPerSprite;

    // Use this for initialization
    void Start()
    {
        Assert.IsTrue(MaxItems > 0);

        m_amountPerSprite = MaxItems / AmountSprites.Length;
        m_spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        base.Update();

        int index = AmountSprites.Length - ((m_currentItems - 1) / m_amountPerSprite) - 1;

        if (index >= 0 && index < AmountSprites.Length)
            m_spriteRenderer.sprite = AmountSprites[index];
    }
}
