using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveManager : MonoBehaviour {

    private SpriteRenderer m_spriteRenderer;

    private const int MatrixKernelSize = 3;
    private const int SidePixels = (MatrixKernelSize - 1) / 2;
    private const int CenterPixel = SidePixels * (MatrixKernelSize + 1);
    private const int LeftPixel = CenterPixel - 1;
    private const int RightPixel = CenterPixel + 1;
    private const int UpPixel = CenterPixel + MatrixKernelSize;
    private const int DownPixel = CenterPixel - MatrixKernelSize;
    private uint m_colliderCount = 0;
	private GameObject m_colliderPrefab;
    private IDictionary<uint, GameObject> m_collidersMap;

    // Use this for initialization
    void Start () {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(m_spriteRenderer);

        m_colliderPrefab = Resources.Load("CaveCollider") as GameObject;
        Assert.IsNotNull(m_colliderPrefab);

        m_collidersMap = new Dictionary<uint, GameObject>();

		Texture2D texture = m_spriteRenderer.sprite.texture;
		GenerateCaveColliders (texture);
    }

	void GenerateCaveColliders(Texture2D texture)
	{
		for (int x = SidePixels; x < texture.width - SidePixels; ++x)
		{
			for (int y = SidePixels; y < texture.height - SidePixels; ++y)
			{
				Color[] colors = texture.GetPixels(x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);

				if (colors == null)
				{
					continue;
				}

				// Detect collider positions
				if (colors[CenterPixel].a != 0.0f &&
					(colors[LeftPixel].a == 0.0f ||
						colors[RightPixel].a == 0.0f ||
						colors[UpPixel].a == 0.0f ||
						colors[DownPixel].a == 0.0f))
				{
					GameObject collider = null;

					if ((colors[LeftPixel].a != 0.0f) && (m_collidersMap.TryGetValue((uint)(y * texture.width + (x - 1)), out collider)))
					{
						collider.transform.localScale = new Vector3(collider.transform.localScale.x + 1.0f,
							collider.transform.localScale.y,
							collider.transform.localScale.z);
					}

					else if ((colors[DownPixel].a != 0.0f) && (m_collidersMap.TryGetValue((uint)((y - 1) * texture.width + x), out collider)))
					{
						collider.transform.localScale = new Vector3(collider.transform.localScale.x,
							collider.transform.localScale.y + 1.0f,
							collider.transform.localScale.z);
					}
					if (collider == null)
					{
						collider = Object.Instantiate(m_colliderPrefab, new Vector3(m_spriteRenderer.bounds.min.x + x,
							m_spriteRenderer.bounds.min.y + y, 0.0f), Quaternion.identity) as GameObject;
						collider.name = "CaveCollider" + m_colliderCount;
						collider.transform.parent = gameObject.transform;
						m_colliderCount++;
					}

					m_collidersMap.Add((uint)(y * texture.width + x), collider);
				}
			}
		}
	}
}
