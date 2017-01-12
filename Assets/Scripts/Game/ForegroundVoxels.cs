using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class ForegroundVoxels : MonoBehaviour {

    private SpriteRenderer m_spriteRenderer;

    private const int MatrixKernelSize = 3;
    private const int SidePixels = (MatrixKernelSize - 1) / 2;
    private const int CenterPixel = SidePixels * (MatrixKernelSize + 1);
    private const int LeftPixel = CenterPixel - 1;
    private const int RightPixel = CenterPixel + 1;
    private const int UpPixel = CenterPixel - MatrixKernelSize;
    private const int DownPixel = CenterPixel + MatrixKernelSize;
    private uint m_voxelCount = 0;
    private GameObject m_voxelPrefab;

    // Use this for initialization
    void Start () {
        m_spriteRenderer = GetComponent<SpriteRenderer>();
        Assert.IsNotNull(m_spriteRenderer);

        m_voxelPrefab = Resources.Load("Voxel") as GameObject;
        Assert.IsNotNull(m_voxelPrefab);

        Texture2D texture = m_spriteRenderer.sprite.texture;

        IDictionary<uint, GameObject> dict = new Dictionary<uint, GameObject>();

        for (int x = SidePixels; x < texture.width - SidePixels; ++x)
        {
            for (int y = SidePixels; y < texture.height - SidePixels; ++y)
            {
                Color[] colors = texture.GetPixels(x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);

                if (colors == null)
                {
                    continue;
                }

                // Detect voxel positions
                if (colors[CenterPixel].a != 0.0f &&
                    (colors[LeftPixel].a == 0.0f ||
                     colors[RightPixel].a == 0.0f ||
                     colors[UpPixel].a == 0.0f ||
                     colors[DownPixel].a == 0.0f))
                {
                    GameObject voxel = null;

                    if (colors[LeftPixel].a != 0.0f)
                    {
                        if (dict.TryGetValue((uint)(y * texture.width + (x - 1)), out voxel))
                        {
                            voxel.transform.localScale = new Vector3(voxel.transform.localScale.x + 1.0f,
                                                                     voxel.transform.localScale.y,
                                                                     voxel.transform.localScale.z);
                            return;
                        }
                    }
                    else if (colors[DownPixel].a != 0.0f)
                    {
                        if (dict.TryGetValue((uint)((y - 1)* texture.width + x), out voxel))
                        {
                            voxel.transform.localScale = new Vector3(voxel.transform.localScale.x,
                                                                     voxel.transform.localScale.y + 1.0f,
                                                                     voxel.transform.localScale.z);
                            return;
                        }

                    }
                    if (voxel == null)
                    {
                        voxel = Object.Instantiate(m_voxelPrefab, new Vector3(m_spriteRenderer.bounds.min.x + x,
                                                                              m_spriteRenderer.bounds.min.y + y, 0.0f), Quaternion.identity) as GameObject;
                        voxel.name = texture.name + "Voxel" + m_voxelCount;
                        voxel.transform.parent = gameObject.transform;
                        m_voxelCount++;

                        //dict.Add((uint)(y * texture.width + x), voxel);
                    }
                    
                    
                    
                }
            }
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
