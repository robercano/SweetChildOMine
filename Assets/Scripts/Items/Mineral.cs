﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Mineral : MineableObject {
    public Sprite[] AmountSprites;

    private int m_amountPerSprite;

    // Use this for initialization
    void Start()
    {
        Assert.IsTrue(MaxItems > 0);

        m_amountPerSprite = MaxItems / AmountSprites.Length;
    }

    public override void Update()
    {
        base.Update();

        if (m_currentItems == 0)
        {
            gameObject.SetActive(false);
            Destroy(gameObject);
        }

        int index = AmountSprites.Length - ((m_currentItems - 1) / m_amountPerSprite) - 1;

        if (index >= 0 && index < AmountSprites.Length)
            m_spriteRenderer.sprite = AmountSprites[index];
    }
}
