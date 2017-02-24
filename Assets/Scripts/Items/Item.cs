using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string Name;
    public Sprite Avatar
    {
        get
        {
            return m_spriteRenderer.sprite;
        }
    }
    public string Description;
    public int Amount;
    public int WeightPerUnit;
    public int TotalWeight
    {
        get
        {
            return Amount * WeightPerUnit;
        }
    }

    public GameObject BuildablePrefab;
    
	private SpriteRenderer m_spriteRenderer;

    protected virtual void Awake()
    {
		m_spriteRenderer = GetComponent<SpriteRenderer>();
        Hide();
    }

    #region /* Public interface */
    public void Show()
    {
        m_spriteRenderer.enabled = true;
    }

    public void Hide()
    {
        m_spriteRenderer.enabled = false;
    }
    #endregion /* Public interface */
};
