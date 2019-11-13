﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateTree : MonoBehaviour
{
    public static GenerateTree instance;
    public TerrianGenerator tg;
    TerrainData terrainData;
    List<GameObject> mapObjects;

    private void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void GetTree(int seed)
    {
        Random.InitState(seed);

        mapObjects = new List<GameObject>();
        terrainData = tg.terrain.terrainData;
        for (float x = 0; x < terrainData.heightmapWidth; x++)
        {
            for (float z = 0; z < terrainData.heightmapHeight; z++)
            {
                Terrain terrain = GetComponent<Terrain>();
                int r = Random.Range(0, 150);
                if (r == 0)//trees
                {
                    TreeInstance treeTemp = new TreeInstance();
                    
                    treeTemp.position = new Vector3(x / terrainData.heightmapWidth, 0, z / terrainData.heightmapHeight);
                    treeTemp.prototypeIndex = Random.Range(0,3);
                    treeTemp.widthScale = 1f;
                    treeTemp.heightScale = 1f;
                    treeTemp.color = Color.white;
                    treeTemp.lightmapColor = Color.white;
                    terrain.AddTreeInstance(treeTemp);
                    terrain.Flush();
                }

                int r1 = Random.Range(0, 50);
                if (r1 == 0)//flowers and grass
                {
                    TreeInstance treeTemp = new TreeInstance();

                    treeTemp.position = new Vector3(x / terrainData.heightmapWidth, 0, z / terrainData.heightmapHeight);
                    treeTemp.prototypeIndex = Random.Range(4, 14);
                    treeTemp.widthScale = 1f;
                    treeTemp.heightScale = 1f;
                    treeTemp.color = Color.white;
                    treeTemp.lightmapColor = Color.white;
                    terrain.AddTreeInstance(treeTemp);
                    terrain.Flush();
                }

                int r2 = Random.Range(0, 100);
                if (r2 == 0)//rocks
                {
                    TreeInstance treeTemp = new TreeInstance();

                    treeTemp.position = new Vector3(x / terrainData.heightmapWidth, 0, z / terrainData.heightmapHeight);
                    treeTemp.prototypeIndex = Random.Range(14, 18);
                    treeTemp.widthScale = 1f;
                    treeTemp.heightScale = 1f;
                    treeTemp.color = Color.white;
                    treeTemp.lightmapColor = Color.white;
                    terrain.AddTreeInstance(treeTemp);
                    terrain.Flush();
                }
            }
        }


        tg.UpdateMemoryStream();
    }

    public void ClearMap(int seed)
    {
        Random.InitState(seed);
        ArrayList newTrees = new ArrayList();
        terrainData = tg.terrain.terrainData;
        terrainData.treeInstances = (TreeInstance[])newTrees.ToArray(typeof(TreeInstance));
        mapObjects.Clear();
    }

    private void OnDestroy()
    {
        instance = null;
    }
}
