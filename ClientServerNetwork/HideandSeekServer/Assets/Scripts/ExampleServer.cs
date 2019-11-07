﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using LitJson;

public class ExampleServer : MonoBehaviour
{
    public static ExampleServer instance;

    public ServerNetwork serverNet;

    public int portNumber = 603;

    // Stores a player
    public class Player
    {
        public long clientId;
        public string playerName;
        public bool isReady;
        public bool isConnected;
        public ServerNetwork.NetworkObject playerObject;
    }
    public List<Player> players = new List<Player>();
    int currentActivePlayer;

    public enum GameState
    {
        pregame,
        running,
        endgame
    }
    public GameState gameState = GameState.pregame;

    // Use this for initialization
    void Awake()
    {
        instance = this;

        // Initialization of the server network
        ServerNetwork.port = portNumber;
        if (serverNet == null)
        {
            serverNet = GetComponent<ServerNetwork>();
        }
        if (serverNet == null)
        {
            serverNet = (ServerNetwork)gameObject.AddComponent(typeof(ServerNetwork));
            Debug.Log("ServerNetwork component added.");
        }

        //serverNet.EnableLogging("rpcLog.txt");
    }

    // A client has just requested to connect to the server
    void ConnectionRequest(ServerNetwork.ConnectionRequestInfo data)
    {
        Debug.Log("Connection request from " + data.username);

        // We either need to approve a connection or deny it
        if (gameState == GameState.pregame) //Make this bigger for the final game
        {
            Player newPlayer = new Player();
            newPlayer.clientId = data.id;
            newPlayer.playerName = data.username;
            newPlayer.isConnected = false;
            newPlayer.isReady = false;
            players.Add(newPlayer);

            serverNet.ConnectionApproved(data.id);
        }
        else
        {
            serverNet.ConnectionDenied(data.id);
        }        
    }

    void OnClientConnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                p.isConnected = true;
            }
            if (p.playerObject.isSeeking)
            {
                serverNet.CallRPC("PlayerIsSeeker", aClientId, p.playerObject.networkId, p.playerObject.networkId);
            }
            else
            {
                serverNet.CallRPC("PlayerIsNotSeeker", aClientId, p.playerObject.networkId, p.playerObject.networkId);
            }
        }

        /*
        serverNet.CallRPC("RPCTest", UCNetwork.MessageReceiver.AllClients, -1, 45);
        ServerNetwork.ClientData data = serverNet.GetClientData(serverNet.SendingClientId);
        serverNet.CallRPC("NewClientConnected", UCNetwork.MessageReceiver.AllClients, -1, aClientId, "bob");
        */
    }

    public void PlayerIsReady()
    {
        // Who called this RPC: serverNet.SendingClientId
        Debug.Log("Player is ready");

        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == serverNet.SendingClientId)
            {
                p.isReady = true;
            }
        }

        /*
        ServerNetwork.ClientData data = serverNet.GetClientData(serverNet.SendingClientId);
        serverNet.CallRPC("Whatever", UCNetwork.MessageReceiver.AllClients, -1);
        //serverNet.Instantiate("Player", Vector3.zero, Quaternion.identity);
        */
    }

    void OnClientDisconnected(long aClientId)
    {
        // Set the isConnected to true on the player
        foreach (Player p in players)
        {
            if (p.clientId == aClientId)
            {
                players.Remove(p);
                return;
            }
        }
    }

    public void PlayerIsSeeking(int networkId) //This RPC will tell the player they are seeking
    {
        serverNet.CallRPC("PlayerIsSeeker", UCNetwork.MessageReceiver.AllClients, networkId, networkId);
    }

    private void Update()
    {
        if (gameState == GameState.pregame)
        {
            // Are all of the players ready?
            bool allPlayersReady = true;
            if (players.Count > 1)
            {
                foreach (Player p in players)
                {
                    if (!p.isReady)
                    {
                        allPlayersReady = false;
                    }
                }
                if (allPlayersReady)
                {
                    gameState = GameState.running;
                    EveryoneIsReady();
                }
            }
        }
        else if (gameState == GameState.running)
        {
            foreach (Player playOb in players)
            {
                foreach (Player playOb2 in players)
                {
                    if (Vector3.Distance(playOb.playerObject.position, playOb2.playerObject.position) < .5f && playOb != playOb2)
                    {
                        Debug.Log("Players are touching");
                        if (playOb2.playerObject.isSeeking == true)
                        {
                            playOb.playerObject.isSeeking = true;
                            serverNet.CallRPC("PlayerIsSeeker", UCNetwork.MessageReceiver.AllClients, playOb.playerObject.networkId, playOb.playerObject.networkId);
                        }
                    }
                }
            }

            int hiders = players.Count;
            foreach (Player p in players)
            {
                if (p.playerObject.isSeeking)
                {
                    hiders--;
                }
            }
            if (hiders <= 0)
            {
                gameState = GameState.endgame;
                serverNet.CallRPC("EndGame", UCNetwork.MessageReceiver.AllClients, -1);
            }
        }
        else if (gameState == GameState.endgame)
        {

        }
    }

    public void EveryoneIsReady()
    {
        serverNet.CallRPC("Run", UCNetwork.MessageReceiver.AllClients, -1);
        players.TrimExcess();
        for (int i = 0; i < players.Count; i++)
        {
            if (i % 5 == 0)
            {
                serverNet.CallRPC("PlayerIsSeeker", UCNetwork.MessageReceiver.AllClients, players[i].playerObject.networkId, players[i].playerObject.networkId);
            }
            else
            {
                serverNet.CallRPC("PlayerIsNotSeeker", UCNetwork.MessageReceiver.AllClients, players[i].playerObject.networkId, players[i].playerObject.networkId);
            }
        }
    }

    public void Slap() //look a few spaces in front of the player and see if anyone is there
    {
        foreach (Player player1 in players)
        {
            if (player1.clientId == serverNet.SendingClientId)
            {
                foreach (Player player2 in players)
                {
                    if (Vector3.Distance(player1.playerObject.rotation * Vector3.forward, player2.playerObject.position) < 1.5f && player2 != player1)
                    {
                        Debug.Log("Slapped");
                        serverNet.CallRPC("PlayerIsSeeker", UCNetwork.MessageReceiver.AllClients, player2.playerObject.networkId, player2.playerObject.networkId);
                    }
                }
            }
        }
    }
}
