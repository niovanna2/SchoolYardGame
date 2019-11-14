using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] string pregame;
    [SerializeField] string running;
    [SerializeField] string endgame;
    public enum GameState
    {
        pregame,
        running,
        endgame
    }
    public GameState gameState = GameState.pregame;
    GameObject[] players;
    GameObject myPlayer;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.pregame)
        {            
        }
        if(gameState == GameState.running)
        {            
        }        
        if(gameState == GameState.endgame)
        {
            //do stuff
        }
    }

    GameObject[] GetPlayers()
    {
        return GameObject.FindGameObjectsWithTag("Player");
    }

    GameObject GetMyPlayer()
    {
        foreach(var p in GetPlayers())
        {
            if(p.GetComponent<NetworkSync>().owned)
            {
                return p;
            }
        }
        return null;
    }

    public void Run()
    {
        Debug.Log("Run has been called");
        gameState = GameState.running;
        //players = GetPlayers();
        myPlayer = GetMyPlayer();
        SceneManager.LoadScene(running);

        if (myPlayer.GetComponent<Player>().seeking)
        {
            transform.position = SpawnPoints.instance.seekerSpawnPoint.transform.position;
        }
        else
        {
            int slot = myPlayer.GetComponent<NetworkSync>().GetId() % SpawnPoints.instance.spawnPoints.Count;
            myPlayer.transform.position = SpawnPoints.instance.spawnPoints[slot].transform.position;
        }
    }

    public void EndGame()
    {
        myPlayer.transform.position = new Vector3(0, 20, 0);
        gameState = GameState.endgame;
        SceneManager.LoadScene(pregame);
    }
}
