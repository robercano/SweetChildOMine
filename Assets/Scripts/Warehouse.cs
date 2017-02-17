using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Warehouse : BuildableObject {

    public Sprite[] ProgressSprites;

	private DragDropController m_dragDropController;
	private CharacterStatus m_characterStatus;

    private int m_progressPerSprite;
    private bool m_complete;

    // Use this for initialization
    void Start()
    {
        Assert.IsTrue(NumSteps > 0);

		m_progressPerSprite = (NumSteps * WorkPerStep) / (ProgressSprites.Length - 1);
        m_complete = false;

		m_dragDropController = GetComponent<DragDropController> ();
		m_dragDropController.OnDragDropFinished = OnDragDropFinished;

		m_characterStatus = GameObject.Find("CharacterStatus").GetComponent<CharacterStatus>(); 
    }

	public void OnDragDropFinished()
	{
		Miner miner = m_characterStatus.GetActiveMiner();
		if (miner != null) {
			miner.BuildStructure (this);
		}
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
