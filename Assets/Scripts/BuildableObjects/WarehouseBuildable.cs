﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class WarehouseBuildable : BuildableObject {

    public Sprite[] ProgressSprites;

	private DragDropController m_dragDropController;
    private int m_progressPerSprite;
    private WarehouseController m_controller;

    // Use this for initialization
    void Start()
    {
        Assert.IsTrue(NumSteps > 0);

        m_progressPerSprite = (NumSteps * WorkPerStep) / (ProgressSprites.Length - 1);

        m_dragDropController = GetComponent<DragDropController>();
        m_dragDropController.OnDragDropFinished = OnActionBuild;

        m_controller = GetComponent<WarehouseController>();
    }

    public override void Update()
    {
        base.Update();

        int index = (m_currentWork / m_progressPerSprite);

        if (index >= 0 && index < ProgressSprites.Length)
            m_spriteRenderer.sprite = ProgressSprites[index];

        if (m_currentWork >= m_totalWork)
        {
            enabled = false;
            m_controller.enabled = true;
        }
    }
}
