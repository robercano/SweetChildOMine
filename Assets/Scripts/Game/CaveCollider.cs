using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveCollider : MonoBehaviour {

    public Sprite MaskSprite;
    public int ColliderLife = 2;
	public GameObject DustEffect;

    [HideInInspector]
    public enum ColliderDirection
    {
        None = 0x0, Left = 0x1, Right = 0x2, Up = 0x4, Down = 0x8
    };
    public ColliderDirection m_direction { get; set; }
    
	private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_boxCollider;
    private GameObject m_caveManager;

    private static uint ColliderID = 0;
    private static uint m_colliderID;

    private int m_life;

    public void AddDirection(ColliderDirection direction)
    {
        m_direction |= direction;
    }
    public bool IsDirectionLeft() { return (m_direction & ColliderDirection.Left) != 0; }
    public bool IsDirectionRight() { return (m_direction & ColliderDirection.Right) != 0; }
    public bool IsDirectionUp() { return (m_direction & ColliderDirection.Up) != 0; }
    public bool IsDirectionDown() { return (m_direction & ColliderDirection.Down) != 0; }

    void Awake()
    {
        m_colliderID = ColliderID++;
        m_direction = ColliderDirection.None;
    }

    void Start()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
        Assert.IsNotNull(m_spriteRenderer);

        m_boxCollider = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(m_boxCollider);

        m_caveManager = GameObject.Find("Cave");

        m_spriteRenderer.sprite = MaskSprite;
    }

    void OnEnable()
    {
        m_life = ColliderLife;
    }

    public uint GetID()
    {
        return m_colliderID;
    }

    public void PlayerHit(int hitPoints)
    {
        if (m_life <= 0) return;

        m_life -= hitPoints;
        m_life = (m_life < 0) ? 0 : m_life;

        m_spriteRenderer.color = new Color(m_spriteRenderer.color.r, m_spriteRenderer.color.g, m_spriteRenderer.color.b, m_life / 20.0f);

        if (m_life > 0) return;

		GameObject.Instantiate (DustEffect, transform.position, Quaternion.identity);
        m_caveManager.SendMessage("RemoveCaveCollider", gameObject);
    }
}
