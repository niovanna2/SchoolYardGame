using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPoints : MonoBehaviour
{
    TerrainData terrainData;
    public static SpawnPoints instance;

    public List<GameObject> spawnPoints;
    public GameObject seekerSpawnPoint;
    public GameObject spawnPointPref;

    public int maxPlayers = 16;
    public int radius = 50;
    public int distanceFromBoarder = 10;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }

        terrainData = Terrain.activeTerrain.terrainData;//gameObject.GetComponent<TerrianGenerator>().terrain.terrainData;
    }

    public void GetSpawnPoints(int seed)
    {
        Random.InitState(seed);

        seekerSpawnPoint = Instantiate(spawnPointPref);
        seekerSpawnPoint.transform.position = new Vector3(128, terrainData.size.y, 128);
        
        //get hidders spawn points; randomly spawned in the area outside of the seeker radius
        for(int i = 0; i < maxPlayers; i++)
        {
            int r = Random.Range(0, 5);
            if(r==0) //(10 - 78, depth, 10-78)
            {
                GameObject sp = Instantiate(spawnPointPref);
                sp.transform.position = new Vector3(Random.Range(distanceFromBoarder, (terrainData.heightmapWidth / 2) - radius), terrainData.size.y,
                    Random.Range(distanceFromBoarder, (terrainData.heightmapHeight / 2) - radius));
                spawnPoints.Add(sp);
            }
            else if (r==1)//(10 - 78, depth, 178,246)
            {
                GameObject sp = Instantiate(spawnPointPref);
                sp.transform.position = new Vector3(Random.Range(distanceFromBoarder, (terrainData.heightmapWidth / 2) - radius), terrainData.size.y,
                    Random.Range((terrainData.heightmapHeight / 2) + radius, terrainData.heightmapHeight - distanceFromBoarder));
                spawnPoints.Add(sp);
            }
            else if (r == 2)//(178,246, depth, 10 - 78)
            {
                GameObject sp = Instantiate(spawnPointPref);
                sp.transform.position = new Vector3(Random.Range((terrainData.heightmapWidth / 2) + radius, terrainData.heightmapWidth - distanceFromBoarder), terrainData.size.y,
                    Random.Range(distanceFromBoarder, (terrainData.heightmapHeight / 2) - radius));
                spawnPoints.Add(sp);
            }
            else if (r == 3)//(178,246, depth, 178,246)
            {
                GameObject sp = Instantiate(spawnPointPref);
                sp.transform.position = new Vector3(Random.Range((terrainData.heightmapWidth / 2) + radius, terrainData.heightmapWidth - distanceFromBoarder), terrainData.size.y,
                    Random.Range((terrainData.heightmapHeight / 2) + radius, terrainData.heightmapHeight - distanceFromBoarder));
                spawnPoints.Add(sp);
            }
        }

    }
}
