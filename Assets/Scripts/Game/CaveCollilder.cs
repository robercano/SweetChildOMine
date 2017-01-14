using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveCollilder : MonoBehaviour {

	private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_boxCollider;
    private Texture2D m_texture;
    private GameObject m_caveManager;

    void Start()
	{
		m_spriteRenderer = GetComponent<SpriteRenderer> ();
        Assert.IsNotNull(m_spriteRenderer);

        m_boxCollider = GetComponent<BoxCollider2D>();
        Assert.IsNotNull(m_boxCollider);

        m_texture = m_spriteRenderer.sprite.texture;
        m_caveManager = GameObject.Find("Cave");
    }

	void OnTriggerEnter2D(Collider2D collider)
	{
		if (collider.name == "DigCollider") {			
            Debug.Log("CaveCollider: min=" + m_boxCollider.bounds.min + ", max=" + m_boxCollider.bounds.max);
            Debug.Log("DigCollider: min=" + collider.bounds.min + ", max=" + collider.bounds.max);
            Debug.Log(gameObject.name + " against " + collider.name);

            float minX = Mathf.Max(collider.bounds.min.x, m_boxCollider.bounds.min.x);
            float minY = Mathf.Max(collider.bounds.min.y, m_boxCollider.bounds.min.y);
            float maxX = Mathf.Min(collider.bounds.max.x, m_boxCollider.bounds.max.x);
            float maxY = Mathf.Min(collider.bounds.max.y, m_boxCollider.bounds.max.y);

            // Check if cave collider is actually outisde of the dig collider
            if (minX >= maxX || minY >= maxY)
                return;

            // Otherwise we have the points of intersection
            m_caveManager.SendMessage("RemoveCaveCollider", gameObject);

            //m_spriteRenderer.color = new Color(0.0f, 1.0f, 0.0f, 1.0f);
        }
	}
}
