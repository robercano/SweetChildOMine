using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveManager : MonoBehaviour {

    static public Color OuterBorderColor = new Color(52.0f / 255.0f, 80.0f / 255.0f, 72.0f / 255.0f);
    static public Color InnerBorderColor = new Color(66.0f / 255.0f, 100.0f / 255.0f, 93.0f / 255.0f);

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

    private int ColliderPoolAmount = 1000;
    private List<GameObject> m_colliderPool;

    // Use this for initialization
    void Start () {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(m_spriteRenderer);

        m_colliderPrefab = Resources.Load("CaveCollider") as GameObject;
        Assert.IsNotNull(m_colliderPrefab);

        m_collidersMap = new Dictionary<uint, GameObject>();
		m_texture = m_spriteRenderer.sprite.texture;

        InitColliderPool();
	    GenerateCaveColliders ();
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
            return;

        GameObject collider = null;

        if ((colors[LeftPixel].a == 0.0f) && (m_collidersMap.TryGetValue(GetColliderID(x - 1, y), out collider)))
        {
            collider.transform.localScale = new Vector3(collider.transform.localScale.x + 1.0f,
                collider.transform.localScale.y,
                collider.transform.localScale.z);
        }
        else if ((colors[DownPixel].a == 0.0f) && (m_collidersMap.TryGetValue(GetColliderID(x, y - 1), out collider)))
        {
            collider.transform.localScale = new Vector3(collider.transform.localScale.x,
                collider.transform.localScale.y + 1.0f,
                collider.transform.localScale.z);
        }
        else
        {
            if (GetColliderFromPool(out collider) == false)
            {
                Debug.Log("No more colliders in pool!");
                return;
            }

            collider.transform.position = new Vector3(m_spriteRenderer.bounds.min.x + x, m_spriteRenderer.bounds.min.y + y, 0.0f);

            CaveCollilder caveColliderScript = collider.GetComponent(typeof(CaveCollilder)) as CaveCollilder;

            if (colors[LeftPixel].a != 0.0f)
                caveColliderScript.AddDirection(CaveCollilder.ColliderDirection.Left);
            if (colors[RightPixel].a != 0.0f)
                caveColliderScript.AddDirection(CaveCollilder.ColliderDirection.Right);
            if (colors[UpPixel].a != 0.0f)
                caveColliderScript.AddDirection(CaveCollilder.ColliderDirection.Up);
            if (colors[DownPixel].a != 0.0f)
                caveColliderScript.AddDirection(CaveCollilder.ColliderDirection.Down);

            collider.GetComponent<SpriteRenderer>().color = OuterBorderColor;
        }
        m_collidersMap.Add(GetColliderID(x, y), collider);

    }

    public void RemoveCaveCollider(GameObject collider)
    {
        uint x = (uint)(collider.transform.position.x - m_spriteRenderer.bounds.min.x);
        uint y = (uint)(collider.transform.position.y - m_spriteRenderer.bounds.min.y);

        uint id = (uint)(y * m_texture.width + x);

        m_collidersMap.Remove(id);
        ReturnColliderToPool(collider);
    }
}
