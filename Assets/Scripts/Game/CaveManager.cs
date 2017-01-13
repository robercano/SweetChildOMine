using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CaveManager : MonoBehaviour {

	private static bool StaticColliderGeneration = true;

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

		if (StaticColliderGeneration == true) {
			foreach (Transform child in transform) {
				m_colliderCount++;

				uint x = (uint)(child.transform.position.x - m_spriteRenderer.bounds.min.x);
				uint y = (uint)(child.transform.position.y - m_spriteRenderer.bounds.min.y);
				m_collidersMap.Add((uint)(y * texture.width + x), child.gameObject);
			}
		} else {
			m_colliderCount = GenerateCaveColliders (texture, m_colliderPrefab, m_spriteRenderer.bounds.min, gameObject.transform, out m_collidersMap);
		}
    }

	public static uint GenerateCaveColliders(Texture2D texture)
	{
		if (StaticColliderGeneration == false) {
			return 0;
		}

		GameObject colliderPrefab = Resources.Load("CaveCollider") as GameObject;
		GameObject sceneForeground = GameObject.Find ("SceneForeground");
		SpriteRenderer spriteRender = sceneForeground.GetComponent<SpriteRenderer> ();
		IDictionary<uint, GameObject> colliderMap;

		// Delete all children in SceneForeground
		while(sceneForeground.transform.childCount != 0) {
			GameObject.DestroyImmediate (sceneForeground.transform.GetChild(0).gameObject);
		}

		return CaveManager.GenerateCaveColliders (texture, colliderPrefab, spriteRender.bounds.min, sceneForeground.transform, out colliderMap);
	}

	private static uint GenerateCaveColliders(Texture2D texture, GameObject colliderPrefab, Vector3 spriteBounds,
		                                      Transform parent, out IDictionary<uint, GameObject> colliderMap)
	{
		uint count = 0;

		colliderMap = new Dictionary<uint, GameObject> ();

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

					if ((colors[LeftPixel].a != 0.0f) && (colliderMap.TryGetValue((uint)(y * texture.width + (x - 1)), out collider)))
					{
						collider.transform.localScale = new Vector3(collider.transform.localScale.x + 1.0f,
							collider.transform.localScale.y,
							collider.transform.localScale.z);
					}

					else if ((colors[DownPixel].a != 0.0f) && (colliderMap.TryGetValue((uint)((y - 1) * texture.width + x), out collider)))
					{
						collider.transform.localScale = new Vector3(collider.transform.localScale.x,
							collider.transform.localScale.y + 1.0f,
							collider.transform.localScale.z);
					}
					if (collider == null)
					{
						collider = Object.Instantiate(colliderPrefab, new Vector3(spriteBounds.x + x, spriteBounds.y + y, 0.0f), Quaternion.identity) as GameObject;
						collider.name = "CaveCollider" + count;
						collider.transform.parent = parent;
						count++;
					}

					colliderMap.Add((uint)(y * texture.width + x), collider);
				}
			}
		}
		return count;
	}
}
