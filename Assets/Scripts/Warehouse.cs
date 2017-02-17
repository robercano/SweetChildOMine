using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Warehouse : BuildableObject {

    public Sprite[] ProgressSprites;

    private int m_progressPerSprite;
    private bool m_complete;

    // Use this for initialization
    void Start()
    {
        Assert.IsTrue(NumSteps > 0);

        m_progressPerSprite = (NumSteps * WorkPerStep) / ProgressSprites.Length;
        m_complete = false;
    }

    public override void Update()
    {
        base.Update();

        if (m_complete == false)
        {
            int index = (m_currentWork / m_progressPerSprite);

            if (index >= 0 && index < ProgressSprites.Length)
                m_spriteRenderer.sprite = ProgressSprites[index];
        }
        if (m_currentWork >= m_totalWork)
            m_complete = true;
    }
}
