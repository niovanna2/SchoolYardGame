using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public void BuildTerrain(int seed)
    {
        gameObject.GetComponent<GenerateTree>().GetTree(seed);
        gameObject.GetComponent<TerrianGenerator>().GetTerrain(seed);
        gameObject.GetComponent<PaintTerrain>().GetPaint(seed);
    }
}
