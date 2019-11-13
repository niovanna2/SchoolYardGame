using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public void BuildTerrain(int seed)
    {
        Debug.Log("BuildTerrain has been called with seed " + seed);
        clearMap(seed);
        getTerrain(seed);
        getTree(seed);
        getPaint(seed);
        getSpawns(seed);
    }

    private void getTerrain(int seed)
    {
        TerrianGenerator.instance.GetTerrain(seed);
    }
    private void getTree(int seed)
    {
        GenerateTree.instance.GetTree(seed);
    }
    private void getPaint(int seed)
    {
        PaintTerrain.instance.GetPaint(seed);
    }
    private void getSpawns(int seed)
    {
        SpawnPoints.instance.GetSpawnPoints(seed);
    }
    private void clearMap(int seed)
    {
        GenerateTree.instance.ClearMap(seed);
    }
}
