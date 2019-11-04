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
            GetPlayers();
            if(CheckFlags())
            {
                Run();
            }
        }
        if(gameState == GameState.running)
        {
            int hiders = players.Length;
            foreach(GameObject p in players)
            {
                if(p.GetComponent<Player>().seeking)
                {
                    hiders--;
                }
            }
            if(hiders <= 0)
            {
                EndGame();
            }
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

    private bool CheckFlags()
    {
        int yesses = 0;
        foreach(GameObject p in players)
        {
            if(p.GetComponent<Player>().ready)
            {
                yesses++;
            }
            else
            {
                return false;
            }
        }
        if (yesses > 0)
        {
            return true;
        }
        else
            return false;
    }
}
