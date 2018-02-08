using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum State { MonsterPhase, CastPhase, Null }

public class IVHostServer : NetworkBehaviour {

    //Managers
    Prototype.NetworkLobby.LobbyManager _lobby;
    IVGameManager _game;
    IVUIManager _ui;

    //Handling Clients
    [SerializeField]
    int playerNum;
    [SerializeField]
    List<NetworkIdentity> players = new List<NetworkIdentity>();


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

        playerNum = _lobby.lobbySlots.Length - 1; // +1 is server
        
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
