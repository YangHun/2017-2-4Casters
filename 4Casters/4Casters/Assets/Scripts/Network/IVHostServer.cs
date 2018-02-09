﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

using Prototype.NetworkLobby;

public enum State { MonsterPhase, CastPhase, Null }

public class IVHostServer : NetworkBehaviour {

    //Managers
    Prototype.NetworkLobby.LobbyManager _lobby;
    IVGameManager _game;
    IVUIManager _ui;

    //Handling Clients
    [SerializeField]
    int playerNum = 0;
    [SerializeField]
    List<NetworkIdentity> players = new List<NetworkIdentity>();

    [SerializeField]
    List<string> playerName = new List<string>();
    [SerializeField]
    public List<bool> playerLoading = new List<bool>();
    public bool isLoading = true;
    bool isLocalClientLoading = false;
    const float sendRPCrate = 0.5f;

    //Game Flow FSM 
    const State startState = State.MonsterPhase;
    State currentState = State.Null;
    State nextState = State.Null;
    bool isFirstFrame = true;
    float timer = 0.0f;

    // Use this for initialization
    public override void OnStartServer()
    {
        Debug.Log("enter? " + gameObject.name);
        if (!isServer)
        {
            Destroy(gameObject);
            return;
        }

        _lobby = GameObject.Find("LobbyManager").GetComponent<Prototype.NetworkLobby.LobbyManager>();
        _game = GameObject.Find("Manager").GetComponent<IVGameManager>();
        _ui = GameObject.Find("Manager").GetComponent<IVUIManager>();
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        _lobby = GameObject.Find("LobbyManager").GetComponent<Prototype.NetworkLobby.LobbyManager>();
        _game = GameObject.Find("Manager").GetComponent<IVGameManager>();
        _ui = GameObject.Find("Manager").GetComponent<IVUIManager>();

        for (int i = 0; i < _lobby.lobbySlots.Length; i++)
        {
            if (_lobby.lobbySlots[i] == null)
                break;
            else
            {
                playerNum++;
                playerName.Add(_lobby.lobbySlots[i].GetComponent<LobbyPlayer>().playerName);
                playerLoading.Add(false);
            }
        }

        GameObject[] Players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in Players)
        {
            p.GetComponent<IVPlayer>().id = playerName.IndexOf(p.GetComponent<IVPlayer>().playerName);
        }

        _ui.InitLoadingCanvas(playerName);
    }

    private void Start()
    {
        Debug.Log("enter? " + gameObject.name);
    }

    // Update is called once per frame
    void Update () {
        timer += Time.deltaTime;


        if (isFirstFrame)
        {
            isFirstFrame = false;
        }
    }

    private void FixedUpdate()
    {
        // if next state is set, update current.
        if (nextState != State.Null)
        {
            currentState = nextState;
            nextState = State.Null;
            isFirstFrame = true;
            timer = 0.0f;
        }
    }

    //------------------------------------------------
    // Command
    //------------------------------------------------

    [Command]
    public void CmdPlayerReady(NetworkIdentity p)
    {
        Debug.Log("Enter?");

        //NetworkIdentity p = _game.myPlayer;

        if (p == null)
        {
            Debug.Log(_game);
            Debug.Log(_game.myPlayer);
        }

        if (players.Count < playerNum)
        {
            players.Add(p);
            p.GetComponent<IVPlayer>().id = players.Count - 1;
            Debug.Log((players.Count - 1) + " player added");
        }
        else
        {
            Debug.Log("player list is full");
        }
    }
    //------------------------------------------------
    // Rpc
    //------------------------------------------------

    [ClientRpc]
    public void RpcPlayerReady()
    {
        Debug.Log("Enter?");
        
        CmdPlayerReady(_game.myPlayer);
    }
}
