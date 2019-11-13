using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public void BuildTerrain(int seed)
    {
        Debug.Log("BuildTerrain has been called with seed " + seed);
        try
        {
            GenerateTree.instance.GetTree(seed);
            TerrianGenerator.instance.GetTerrain(seed);
            PaintTerrain.instance.GetPaint(seed);
            SpawnPoints.instance.GetSpawnPoints(seed);
        }
        catch(Exception e)
        {
            Debug.Log(e.StackTrace);
            Debug.Log(e.Message);
        }
    }
}
