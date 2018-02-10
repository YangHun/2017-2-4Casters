using System.Collections;
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
    public Dictionary<int, NetworkIdentity> players = new Dictionary<int, NetworkIdentity>();

    [SerializeField]
    List<string> playerName = new List<string>();
    [SerializeField]
    public List<bool> playerLoading = new List<bool>();
    public bool isLoading = true;
    bool isLocalClientLoading = false;
    const float sendRPCrate = 0.5f;

    //Game Flow FSM 
    const State startState = State.MonsterPhase;
    State currentState = State.MonsterPhase;
    State nextState = State.Null;
    bool isFirstFrame = true;
    float timer = 0.0f;

    public State CurrentState
    {
        get { return currentState; }
    }

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
            players.Add(p.GetComponent<IVPlayer>().id, p.GetComponent<NetworkIdentity>());
            Debug.Log(p.GetComponent<IVPlayer>().id + " / " + players[p.GetComponent<IVPlayer>().id]);
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

        if (!isLoading && isServer) // Load finished, run game
        {

            _game.RpcOnState(currentState, isFirstFrame, timer);

            if (isFirstFrame)
            {
                isFirstFrame = false;
            }

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

    public void SetNextState(State next)
    {
        if (isServer)
            RpcSetNextState(next);
    }

    [ClientRpc]
    void RpcSetNextState(State next)
    {
        if (next != currentState)
            nextState = next;
    }
    
   
}
