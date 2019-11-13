using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class PaintTerrain : MonoBehaviour
{
    public static PaintTerrain instance;
    float unit;

    TerrainData terrainData;
    public GenerateTree tree;
    public int distanceBetween = 1000;
    public int count = 0;

    private void Start()
    {
        if(instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    public void GetPaint(int seed)
    {
        Random.InitState(seed);
        terrainData = Terrain.activeTerrain.terrainData;
        AssignSplatMap();
        
        unit = 1f / (terrainData.size.x - 1);
    }

    void AssignSplatMap()
    {
        // Splatmap data is stored internally as a 3d array of floats, so declare a new empty array ready for your custom splatmap data:
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int y = 0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                // Normalise x/y coordinates to range 0-1 
                float y_01 = (float)y / (float)terrainData.alphamapHeight;
                float x_01 = (float)x / (float)terrainData.alphamapWidth;

                // Setup an array to record the mix of texture weights at this point
                float[] splatWeights = new float[terrainData.alphamapLayers];

                // get height and slope at corresponding point
                float height = GetHeightAtPoint(x_01 * terrainData.size.x, y_01 * terrainData.size.z);
                float slope = GetSlopeAtPoint(x_01 * terrainData.size.x, y_01 * terrainData.size.z);

                //====Rules for applying different textures===========================
                splatWeights[0] = 1 - slope; // decreases with slope (ground texture)
                
                /*if(1-slope>0.3 && count <=0)
                {
                    tree.SpawnTree(x,y);
                    count = distanceBetween;
                }
                count--;*/
                splatWeights[1] = slope; // increases with slope (rocky texture)

                splatWeights[2] = ( // apply 75% only to "Mesa" uplands (NOTE: the first two textures sum 1, so 1.5 corresponds to 80%)
                    height > 0.5f * terrainData.heightmapHeight && // higher terrain
                    slope < 0.3f) // plain terrain
                        ? 1.5f : 0f;

                splatWeights[3] = ( // apply 50% only to valley floor (NOTE: textures 2 and 3 never coexist, so 1 corresponds to 50%))
                    height < 0.5f * terrainData.size.y && // lower terrain
                    slope < 0.3f) // plain terrain
                    ? 1f : 0f;
                //====================================================================

                // Sum of all textures weights must add to 1, so calculate normalization factor from sum of weights
                float z = splatWeights.Sum();

                // Loop through each terrain texture
                for (int i = 0; i < terrainData.alphamapLayers; i++)
                {

                    // Normalize so that sum of all texture weights = 1
                    splatWeights[i] /= z;

                    // Assign this point to the splatmap array
                    splatmapData[y, x, i] = splatWeights[i];
                    // NOTE: Alpha map array dimensions are shifted in relation to heightmap and world space (y is x and x is y or z)
                }
            }
        }

        // Finally assign the new splatmap to the terrainData:
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    float GetSlopeAtPoint(float pointX, float pointZ, bool scaleToRatio = true)
    {
        float factor = (scaleToRatio) ? 90f : 1f;
        return terrainData.GetSteepness(unit * pointX, unit * pointZ) / 90f; // x and z coordinates must be scaled
    }

    float GetHeightAtPoint(float pointX, float pointZ, bool scaleToTerrain = false)
    {
        float height = terrainData.GetInterpolatedHeight(unit * pointX, unit * pointZ);

        // x and z coordinates must be scaled with "unit"
        if (scaleToTerrain)
            return height / terrainData.size.y;
        else
            return height;
    }


    /* [System.Serializable]
     public class SplatHeights
     {
         public int textureIndex;
         public int startingHeight;
     }

     public SplatHeights[] splatHeights;
     public TerrianGenerator tg;

     void Start()
     {
         TerrainData terrainData = Terrain.activeTerrain.terrainData;
         float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight,terrainData.alphamapLayers];
         Debug.Log(terrainData.alphamapWidth);
         Debug.Log(terrainData.alphamapHeight);

         for (int y = 0; y<terrainData.alphamapHeight;y++)
         {
             for(int x=0;x<terrainData.alphamapWidth; x++)
             {
                 float terrainHeight = terrainData.GetHeight(y, x);
                 float[] splat = new float[splatHeights.Length];

                 for(int i = 0; i<splatHeights.Length;i++)
                 {
                     if(terrainHeight>=splatHeights[i].startingHeight)
                     {
                         splat[i] = 1;
                     }
                 }

                 for (int j = 0; j < splatHeights.Length; j++)
                 {
                     splatmapData[x, y, j] = splat[j];
                 }
             }
         }

         terrainData.SetAlphamaps(0, 0, splatmapData);
     }*/
}
