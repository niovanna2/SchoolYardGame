using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        pregame,
        running,
        endgame
    }
    public GameState gameState = GameState.pregame;
    GameObject[] players;

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

    public void Run()
    {
        gameState = GameState.running;
        players = GetPlayers();
    }

    private void EndGame()
    {
        gameState = GameState.endgame;
    }
}
