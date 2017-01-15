using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveCollilder : MonoBehaviour {

    public Sprite white;

    [HideInInspector]
    public enum ColliderDirection
    {
        None = 0x0, Left = 0x1, Right = 0x2, Up = 0x4, Down = 0x8
    };
    private ColliderDirection m_direction;
    
	private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_boxCollider;
    private Texture2D m_texture;
    private GameObject m_caveManager;

    private static uint ColliderID = 0;
    private static uint m_colliderID;

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
    }

    void Start()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
        Assert.IsNotNull(m_spriteRenderer);

        m_boxCollider = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(m_boxCollider);

        m_texture = m_spriteRenderer.sprite.texture;
        m_caveManager = GameObject.Find("Cave");

        m_spriteRenderer.sprite = white;
        m_direction = ColliderDirection.None;
    }

    public uint GetID()
    {
        return m_colliderID;
    }

    public void SetDirection(ColliderDirection direction)
    {
        m_direction = direction;
    }

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.name == "DigCollider") {			
            //Debug.Log("CaveCollider: min=" + m_boxCollider.bounds.min + ", max=" + m_boxCollider.bounds.max);
            //Debug.Log("DigCollider: min=" + collider.bounds.min + ", max=" + collider.bounds.max);
            //Debug.Log(gameObject.name + " against " + collider.name);

            float minX = Mathf.Max(collider.bounds.min.x, m_boxCollider.bounds.min.x);
            float minY = Mathf.Max(collider.bounds.min.y, m_boxCollider.bounds.min.y);
            float maxX = Mathf.Min(collider.bounds.max.x, m_boxCollider.bounds.max.x);
            float maxY = Mathf.Min(collider.bounds.max.y, m_boxCollider.bounds.max.y);

            // Check if cave collider is actually outisde of the dig collider
            if (minX >= maxX || minY >= maxY)
                return;

            // Otherwise we have the points of intersection
            m_caveManager.SendMessage("RemoveCaveCollider", gameObject);
        }
	}
}
