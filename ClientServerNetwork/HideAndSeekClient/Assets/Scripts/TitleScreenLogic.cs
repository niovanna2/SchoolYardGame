using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TitleScreenLogic : MonoBehaviour {

    public Text server;
    public Text port;
    public ExampleClient client;
    public GameManager manager;
    public GameObject titleScreen;

    private void Start()
    {
        client = FindObjectOfType<ExampleClient>();
        manager = FindObjectOfType<GameManager>();
        if(client.clientNet.IsConnected())
        {
            this.gameObject.SetActive(false);
        }
        if(manager.gameState == GameManager.GameState.endgame)
        {
            titleScreen.SetActive(false);
            manager.gameState = GameManager.GameState.pregame;
        }
    }

    public void Connect()
    {
        client.ConnectToServer(server.text, int.Parse(port.text));
    }
}
