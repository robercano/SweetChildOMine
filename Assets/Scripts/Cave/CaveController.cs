using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveController : MonoBehaviour {

    public Color OuterBorderColor = new Color(52.0f / 255.0f, 80.0f / 255.0f, 72.0f / 255.0f);
    public Color InnerBorderColor = new Color(66.0f / 255.0f, 100.0f / 255.0f, 93.0f / 255.0f);

    public bool OptimizeColliders = false;

    public struct ColliderPosition
    {
        public ColliderPosition(int _x, int _y) { x = _x;  y = _y; }
        public int x, y;
    };
    private List<ColliderPosition> m_newColliders;

    private const int MatrixKernelSize = 3;
    private const int SidePixels = (MatrixKernelSize - 1) / 2;
    private const int CenterPixel = SidePixels * (MatrixKernelSize + 1);
    private const int LeftPixel = CenterPixel - 1;
    private const int RightPixel = CenterPixel + 1;
    private const int UpPixel = CenterPixel + MatrixKernelSize;
    private const int DownPixel = CenterPixel - MatrixKernelSize;

    private SpriteRenderer m_spriteRenderer;
	private GameObject m_colliderPrefab;
    private IDictionary<uint, GameObject> m_collidersMap;
    private Texture2D m_texture;

    private int ColliderPoolAmount = 2000;
    private List<GameObject> m_colliderPool;

    private bool m_textureHasChanged;

    private GameObject m_diggingContextMenuPrefab;
    private GameObject m_diggingContextMenuInstance;
    private DiggingContextMenu m_diggingContextMenu;

    private UIController m_UIController;
    private Vector2 m_digTarget;

    // Use this for initialization
    void Start () {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(m_spriteRenderer);

        m_colliderPrefab = Resources.Load("CaveCollider") as GameObject;
        Assert.IsNotNull(m_colliderPrefab);

        m_newColliders = new List<ColliderPosition>();
        m_collidersMap = new Dictionary<uint, GameObject>();

        /* Recreate the sprite so we can destroy the in-memory texture instead */
        Texture2D texture = GameObject.Instantiate(m_spriteRenderer.sprite.texture) as Texture2D;
        m_spriteRenderer.sprite = Sprite.Create(texture, m_spriteRenderer.sprite.rect, Vector2.zero, 1.0f);
        m_texture = m_spriteRenderer.sprite.texture;
        m_textureHasChanged = false;

        InitColliderPool();
	    GenerateCaveColliders ();

        m_diggingContextMenuPrefab = Resources.Load("UI/DiggingContextMenu") as GameObject;
        m_diggingContextMenuInstance = GameObject.Instantiate(m_diggingContextMenuPrefab);
        m_diggingContextMenu = m_diggingContextMenuInstance.GetComponent<DiggingContextMenu>();
        m_diggingContextMenu.ActionName = "Dig here";
        m_diggingContextMenu.OnAction = OnDigCave;
        m_diggingContextMenu.Disable();

        m_UIController = GameObject.Find("MainUI").GetComponent<UIController>();
    }

    void Update()
    {
        if (m_textureHasChanged)
        {
            m_textureHasChanged = false;
            m_texture.Apply(false, false);
        }

        foreach (ColliderPosition position in m_newColliders)
        {
            AddCaveCollider(position.x, position.y);
        }
        m_newColliders.Clear();
    }

    void InitColliderPool()
    {
        m_colliderPool = new List<GameObject>(ColliderPoolAmount);

        for (int i=0; i<ColliderPoolAmount; ++i)
        {
            GameObject collider = Object.Instantiate(m_colliderPrefab, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity) as GameObject;
            collider.SetActive(false);
            collider.name = "CaveCollider" + i;
            collider.transform.parent = transform;
            collider.tag = "CaveCollider";
            m_colliderPool.Add(collider);
        }
    }

    bool GetColliderFromPool(out GameObject collider)
    {
        if (m_colliderPool.Count == 0)
        {
            collider = null;
            return false;
        }

        collider = m_colliderPool[0];
        collider.SetActive(true);
        m_colliderPool.RemoveAt(0);
        return true;
    }

    void ReturnColliderToPool(GameObject collider)
    {
        collider.SetActive(false);
        m_colliderPool.Add(collider);
    }

    void GenerateCaveColliders()
	{
		for (int x = SidePixels; x < m_texture.width - SidePixels; ++x)
		{
			for (int y = SidePixels; y < m_texture.height - SidePixels; ++y)
			{
                AddCaveCollider(x, y);
			}
		}
	}

    uint GetColliderID(int x, int y)
    {
        return (uint)(y * m_texture.width + x);
    }

    public void AddCaveCollider(int x, int y)
    {
        Color[] colors = m_texture.GetPixels(x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);
        Assert.IsNotNull(colors);

        // Detect collider positions
        if (colors[CenterPixel].a != 0.0f ||
            (colors[LeftPixel].a == 0.0f &&
            colors[RightPixel].a == 0.0f &&
            colors[UpPixel].a == 0.0f &&
            colors[DownPixel].a == 0.0f))
        {
            /*if (test)
            {
                Debug.Log("OPs!! ("+x+", "+y+")");
                Debug.Log("Center:  " + colors[CenterPixel]);
                Debug.Log("LeftPixel:  " + colors[LeftPixel]);
                Debug.Log("RightPixel:  " + colors[RightPixel]);
                Debug.Log("UpPixel:  " + colors[UpPixel]);
                Debug.Log("DownPixel:  " + colors[DownPixel]);
            }*/

             return;
        }

        GameObject collider = null;

        if (m_collidersMap.TryGetValue(GetColliderID(x, y), out collider))
        {
            // Collider exists already, bail out
            return;
        }

        if (OptimizeColliders)
        {
            if ((colors[LeftPixel].a == 0.0f) && (m_collidersMap.TryGetValue(GetColliderID(x - 1, y), out collider)))
            {
                collider.transform.localScale = new Vector3(collider.transform.localScale.x + 1.0f, collider.transform.localScale.y, collider.transform.localScale.z);
            }
            else if ((colors[DownPixel].a == 0.0f) && (m_collidersMap.TryGetValue(GetColliderID(x, y - 1), out collider)))
            {
                collider.transform.localScale = new Vector3(collider.transform.localScale.x, collider.transform.localScale.y + 1.0f, collider.transform.localScale.z);
            }
        }
        
        /* If we don't have a collider yet, get one from the pool and use it */
        if (collider == null)
        {
            if (GetColliderFromPool(out collider) == false)
            {
                Debug.Log("No more colliders in pool!");
                return;
            }

            collider.transform.position = new Vector3(m_spriteRenderer.bounds.min.x + x, m_spriteRenderer.bounds.min.y + y, 0.0f);

            CaveCollider caveColliderScript = collider.GetComponent(typeof(CaveCollider)) as CaveCollider;

            if (colors[LeftPixel].a != 0.0f)
            {
                caveColliderScript.AddDirection(CaveCollider.ColliderDirection.Left);
            }
            if (colors[RightPixel].a != 0.0f)
            {
                caveColliderScript.AddDirection(CaveCollider.ColliderDirection.Right);
            }
            if (colors[UpPixel].a != 0.0f)
            {
                caveColliderScript.AddDirection(CaveCollider.ColliderDirection.Up);
            }
            if (colors[DownPixel].a != 0.0f)
            {
                caveColliderScript.AddDirection(CaveCollider.ColliderDirection.Down);
            }
            collider.GetComponent<SpriteRenderer>().color = OuterBorderColor;
        }
        m_collidersMap.Add(GetColliderID(x, y), collider);
    }

    void QueueNewCollider(int x, int y)
    {
        GameObject coll;

        if (m_collidersMap.TryGetValue(GetColliderID(x, y), out coll))
            return;

        m_newColliders.Add(new ColliderPosition(x, y));
        m_texture.SetPixel(x, y, Color.clear);
        m_textureHasChanged = true;
    }

    public void RemoveCaveCollider(GameObject collider)
    {
        int x = (int)(collider.transform.position.x - m_spriteRenderer.bounds.min.x);
        int y = (int)(collider.transform.position.y - m_spriteRenderer.bounds.min.y);

        uint id = (uint)(y * m_texture.width + x);

        //Debug.Log("Removing collider at (" + x + ", " + y + ")");

        CaveCollider caveColliderScript = collider.GetComponent(typeof(CaveCollider)) as CaveCollider;

        m_collidersMap.Remove(id);
        ReturnColliderToPool(collider);

        //Debug.Log("Collider direction " + (uint)caveColliderScript.m_direction);
        if (caveColliderScript.IsDirectionUp() && caveColliderScript.IsDirectionRight())
            QueueNewCollider(x + 1, y + 1);
        if (caveColliderScript.IsDirectionDown() && caveColliderScript.IsDirectionRight())
            QueueNewCollider(x + 1, y - 1);
        if (caveColliderScript.IsDirectionUp() && caveColliderScript.IsDirectionLeft())
            QueueNewCollider(x - 1, y + 1);
        if (caveColliderScript.IsDirectionDown() && caveColliderScript.IsDirectionLeft())
            QueueNewCollider(x - 1, y - 1);
        if (caveColliderScript.IsDirectionUp())
            QueueNewCollider(x, y + 1);
        if (caveColliderScript.IsDirectionDown())
            QueueNewCollider(x, y - 1);
        if (caveColliderScript.IsDirectionLeft())
            QueueNewCollider(x - 1, y);
        if (caveColliderScript.IsDirectionRight())
            QueueNewCollider(x + 1, y);
    }

    public void CancelPointerClick()
    {
        m_diggingContextMenu.Disable();
    }

    public bool HandlePointerClick(Vector2 mousePosition)
    {
        m_digTarget = Camera.main.ScreenToWorldPoint(mousePosition);
        m_digTarget = new Vector2(Mathf.Round(m_digTarget.x), Mathf.Round(m_digTarget.y));

		m_diggingContextMenu.SetPosition(m_digTarget);
        m_diggingContextMenu.Enable();
        return true;
    }

    public void OnDigCave()
    {
        m_diggingContextMenu.Disable();

        Miner miner = m_UIController.GetActiveMiner();
        if (miner != null)
            miner.DigCave(m_digTarget);
    }
}
