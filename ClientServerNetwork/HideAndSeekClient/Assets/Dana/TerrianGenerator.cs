using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class TerrianGenerator : MonoBehaviour
{
    public int depth = 20;

    public int width = 256;
    public int height = 256;

    public float scale = 20f;

    public float offsetX = 100f;
    public float offsetY = 100f;

    public Terrain terrain;

    public MemoryStream memoryStream;

    public void GetTerrain(int seed)
    {
        Random.InitState(seed);

        offsetX = Random.Range(0f, 99999f);
        offsetY = Random.Range(0f, 99999f);
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    public void UpdateMemoryStream()
    {
        byte[] buffer = new byte[1024];
        memoryStream = new MemoryStream(buffer);

        BinaryWriter bw = new BinaryWriter(memoryStream);

        bw.Write(terrain);

        string s = Encoding.UTF8.GetString(buffer);
        //RPC send out terrain
    }

    TerrainData GenerateTerrain(TerrainData terrainData)
    {
        terrainData.heightmapResolution = width + 1;

        terrainData.size = new Vector3(width, depth, height);
        terrainData.SetHeights(0, 0, GenerateHeights());
        return terrainData;
    }

    float[,] GenerateHeights()
    {
        float[,] heights = new float[width, height];

        for(int x = 0; x<width; x++)
        {
            for(int y=0;y<height; y++)
            {
                heights[x, y] = CalculateHeight(x, y);
            }
        }
        return heights;
    }

    float CalculateHeight(int x, int y)
    {
        float xCoord = (float) x / width * scale + offsetX;
        float yCoord = (float) y / height * scale + offsetY;

        return Mathf.PerlinNoise(xCoord, yCoord);
    }
}
