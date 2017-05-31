using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragDropController : MonoBehaviour {

    public float SnapValue = 1;
    public LayerMask RaycastMask;

	public delegate void OnDragDropFinishedDelegate();
	public OnDragDropFinishedDelegate OnDragDropFinished;

    private SpriteRenderer m_spriteRenderer;
    private BoxCollider2D m_boxCollider;
    private SpriteOutline m_spriteOutline;

    private GameObject m_cave;
    private Texture2D m_caveTexture;
    
    // Use this for initialization
    void Awake()
    {
		Debug.Log ("Awake DragDrop " + name);
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        m_boxCollider = GetComponent<BoxCollider2D>();
        m_spriteOutline = GetComponent<SpriteOutline>();

        m_cave = GameObject.Find("Cave");
        m_caveTexture = m_cave.GetComponent<SpriteRenderer>().sprite.texture;
    }

    // Update is called once per frame
    void Update()
    {
        SnapToGrid();

        if (CheckValidPosition())
        {
            m_spriteOutline.color = new Color(0.0f, 1.0f, 0.0f, 0.5f);
        }
        else
        {
            m_spriteOutline.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
        }
    }

    void SnapToGrid()
    {
        float snapInverse = 1 / SnapValue;
        float x, y;

        // if snapValue = .5, x = 1.45 -> snapInverse = 2 -> x*2 => 2.90 -> round 2.90 => 3 -> 3/2 => 1.5
        // so 1.45 to nearest .5 is 1.5
        x = Mathf.Round(transform.position.x * snapInverse) / snapInverse;
        y = Mathf.Round(transform.position.y * snapInverse) / snapInverse;

        transform.position = new Vector2(x, y);
    }

    bool CheckValidPosition()
    {
        Vector2 minCorner = new Vector2(Mathf.FloorToInt(m_boxCollider.bounds.min.x), Mathf.FloorToInt(m_boxCollider.bounds.min.y));
        Vector2 maxCorner = new Vector2(Mathf.FloorToInt(m_boxCollider.bounds.max.x), Mathf.FloorToInt(m_boxCollider.bounds.max.y));

        // Check if corners are inside the cave
        Color pixel = m_caveTexture.GetPixel((int)minCorner.x, (int)maxCorner.y);
        if (pixel.a != 0)
        {
            return false;
        }
        pixel = m_caveTexture.GetPixel((int)maxCorner.x, (int)maxCorner.y);
        if (pixel.a != 0)
        {
            return false;
        }
        pixel = m_caveTexture.GetPixel((int)minCorner.x, (int)minCorner.y);
        if (pixel.a != 0)
        {
            return false;
        }
        pixel = m_caveTexture.GetPixel((int)maxCorner.x, (int)minCorner.y);
        if (pixel.a != 0)
        {
            return false;
        }

        // Now check if bottom is touching the floor
        m_boxCollider.enabled = false;
        RaycastHit2D hit1 = Physics2D.Raycast(minCorner, Vector2.down, 2.5f, RaycastMask);
        RaycastHit2D hit2 = Physics2D.Raycast(new Vector2(maxCorner.x, minCorner.y), Vector2.down, 2.5f, RaycastMask);
        m_boxCollider.enabled = true;

        if (hit1.collider == null || hit2.collider == null)
        {
            return false;
        }
        return true;
    }

    public void StartDrag()
    {
        m_spriteRenderer.sortingLayerName = "UI";
    }

    public void FinishDrag(bool destroyAnyways = false)
    {
        m_spriteRenderer.sortingLayerName = "Items";

        // TODO: If position is suitable let the object live, otherwise destroy the object
        if (destroyAnyways || !CheckValidPosition())
        {
            Destroy(gameObject);
        }
        else
        {
			if (OnDragDropFinished != null)
				OnDragDropFinished ();
            m_spriteOutline.enabled = false;
            this.enabled = false;
        }
    }
 }
