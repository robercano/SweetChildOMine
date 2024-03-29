﻿#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEditor;

public class CaveImporter :  AssetPostprocessor
{
    public Color m_outerBorder = new Color(52.0f / 255.0f, 80.0f / 255.0f, 72.0f / 255.0f);
    public Color m_innerBorder = new Color(66.0f / 255.0f, 100.0f / 255.0f, 93.0f / 255.0f);

    private const int MatrixKernelSize = 5;
	private const int SidePixels = (MatrixKernelSize - 1) / 2;
	private const int CenterPixel = SidePixels * (MatrixKernelSize + 1);
	private const int LeftPixel = CenterPixel - 1;
	private const int LeftLeftPixel = CenterPixel - 2;
	private const int RightPixel = CenterPixel + 1;
	private const int RightRightPixel = CenterPixel + 2;
	private const int UpPixel = CenterPixel - MatrixKernelSize;
	private const int UpUpPixel = CenterPixel - 2*MatrixKernelSize;
	private const int DownPixel = CenterPixel + MatrixKernelSize;
	private const int DownDownPixel = CenterPixel + 2*MatrixKernelSize;

    void OnPreprocessTexture()
	{
		TextureImporter textureImporter = (TextureImporter)assetImporter;

		textureImporter.isReadable = true;
		textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
	}

	void OnPostprocessTexture (Texture2D texture)
	{
        string lowerCaseAssetPath = assetPath.ToLower ();
		if (lowerCaseAssetPath.IndexOf ("/foregrounds/") == -1)
			return;

		RemoveCaveBorders (texture);
        //AddCaveBorders (texture);
        //AddCaveBorders(texture, m_innerBorder);

        //GameObject cave = GameObject.Find("Cave");
        //Assert.IsNotNull(cave);

		//CaveManager.GenerateCaveColliders (texture);
	}

    void RemoveCaveBorders(Texture2D texture)
	{
		RemoveCaveBorders (texture, m_outerBorder);
		RemoveCaveBorders (texture, m_innerBorder);
	}

    void RemoveCaveBorders(Texture2D texture, Color color)
    {
        Texture2D newTexture = Object.Instantiate(texture) as Texture2D;

        for (int x = SidePixels; x < texture.width - SidePixels; ++x)
        {
            for (int y = SidePixels; y < texture.height - SidePixels; ++y)
            {
                Color[] colors = texture.GetPixels(x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);

                if (colors == null)
                {
                    continue;
                }

                if (colors[CenterPixel] == color &&
                    (colors[LeftPixel].a == 0.0f ||
                     colors[RightPixel].a == 0.0f ||
                     colors[UpPixel - 1].a == 0.0f ||
                     colors[UpPixel].a == 0.0f ||
                     colors[UpPixel + 1].a == 0.0f ||
                     colors[UpPixel].a == 0.0f ||
                     colors[DownPixel - 1].a == 0.0f ||
                     colors[DownPixel].a == 0.0f ||
                     colors[DownPixel + 1].a == 0.0f))
                {
                    newTexture.SetPixel(x, y, Color.clear);
                }
            }
        }
        texture.LoadRawTextureData(newTexture.GetRawTextureData());
    }
#if DISABLED
    void AddCaveBorders(Texture2D texture)
	{
		AddCaveBorders (texture, m_innerBorder);
		AddCaveBorders (texture, m_outerBorder);
	}

	void AddCaveBorders(Texture2D texture, Color color)
	{
		Texture2D newTexture = Object.Instantiate (texture) as Texture2D;

		for (int x = SidePixels; x < texture.width - SidePixels; ++x) {
			for (int y = SidePixels; y < texture.height - SidePixels; ++y) {
				Color[] colors = texture.GetPixels (x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);

				if (colors == null) {
					continue;
				}

				// Detect collider positions
				if (colors [CenterPixel].a == 0.0f) {
					if (colors[LeftPixel].a != 0.0f ||
                        colors[RightPixel].a != 0.0f ||
                        colors[UpPixel - 1].a != 0.0f ||
                        colors[UpPixel].a != 0.0f ||
                        colors[UpPixel + 1].a != 0.0f ||
                        colors[UpPixel].a != 0.0f ||
                        colors[DownPixel - 1].a != 0.0f ||
                        colors[DownPixel].a != 0.0f ||
                        colors[DownPixel + 1].a != 0.0f) {
					    newTexture.SetPixel (x, y, color);
					}
				}
			}
		}
		texture.LoadRawTextureData (newTexture.GetRawTextureData ());
	}
    void DetectVoxelPositions(Texture2D texture)
	{
		for (int x = SidePixels; x < texture.width - SidePixels; ++x) {
			for (int y = SidePixels; y < texture.height - SidePixels; ++y) {
				Color[] colors = texture.GetPixels (x - SidePixels, y - SidePixels, MatrixKernelSize, MatrixKernelSize);

				if (colors == null) {
					continue;
				}

				// Detect voxel positions
				if (colors [CenterPixel].a != 0.0f &&
					(colors [LeftPixel].a == 0.0f ||
						colors [RightPixel].a == 0.0f ||
						colors [UpPixel].a == 0.0f ||
						colors [DownPixel].a == 0.0f)) {
					texture.SetPixel (x, y, Color.red);
                    GameObject voxel = Object.Instantiate(m_colliderPrefab, new Vector3(x, y, 0.0f), Quaternion.identity) as GameObject;
                    voxel.name = texture.name + m_colliderCount;
                    m_colliderCount++;
				}
			}
		}
	}
#endif
}

#endif // UNITY_EDITOR