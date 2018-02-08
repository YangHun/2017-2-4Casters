using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVGameManager : NetworkBehaviour {

    /*
    public void OnState(State t)
    {
        switch (t)
        {
            case State.MonsterPhase:
                //OnStateMonsterPhase();
                break;
            case State.CastPhase:
                //OnStateCastPhase();
                break;
            case State.Null:

                break;
        }
        
    }

*/


    NetworkIdentity _identity;
    NetworkLobbyManager _lobby;

    //Handling Player
    [SerializeField]
    List<IVPlayer> _players = new List<IVPlayer>();
    List<bool> _playerstatus = new List<bool>();

    public NetworkIdentity myPlayer = null;


    //Handling Game Flow FSM 
    public enum State { MonsterPhase, CastPhase, Null }

 //   const State startState = State.MonsterPhase;
    const State startState = State.Null;
    State currentState = State.Null;
    State nextState = State.Null;
    bool isFirstFrame = true;

    float timer = 0.0f;
    
    //Handling Monster
    [SerializeField]
    IVMonsterSpawner _spawner;

    public List<IVPlayer> Players
    {
        get
        {
            return _players;
        }
    }

    public State CurrentState
    {
        get
        {
            return currentState;
        }
    }

    private void Start()
    {
        Debug.Log("enter? " + gameObject.name);
        FindMyPlayer();
    }

    public override void OnStartServer()
    {
        Debug.Log("enter? " + gameObject.name);
        base.OnStartServer();
    }

    public override void OnStartClient()
    {
        Debug.Log("enter? " + gameObject.name);
        base.OnStartClient();
        
        _identity = GetComponent<NetworkIdentity>();
        _lobby = GetComponent<NetworkLobbyManager>();
        
        currentState = startState;
        
    }

    public void FindMyPlayer()
    {
        Debug.Log("enter here?");
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        foreach (GameObject p in players)
        {
            if (p.GetComponent<IVPlayer>().myPlayer)
            {
                myPlayer = p.GetComponent<IVPlayer>().identity;
            }
        }

        if (myPlayer == null)
            Debug.Log("cannot find my player!");
        else
            myPlayer.GetComponent<IVPlayer>().CmdClientReady(myPlayer);
        
        
    }

   

    [ClientRpc]
    public void RpcUpdatePlayerList(NetworkIdentity p)
    {
        Debug.Log("enter????????");
        Players.Add(p.GetComponent<IVPlayer>());
        p.GetComponent<IVPlayer>().id = Players.Count - 1;
    }

    void Update()
    {
        
        timer += Time.deltaTime;

        

        //OnState function is called on each frame
        switch (currentState)
        {
            case State.MonsterPhase:
                OnStateMonsterPhase();
                break;
            case State.CastPhase:
                OnStateCastPhase();
                break;
            case State.Null:

                break;
        }

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
        }
    }

    //OnState functions definition

    void OnStateMonsterPhase()
    {
        if (isFirstFrame)
        {
            foreach (IVPlayer p in Players)
                p.ResetPlayers();
            IVUIManager.I.ChangeRightButtonText("Attack");
            IVUIManager.I.ResetPlayerKeywordText();
            IVUIManager.I.RefreshInMonsterPhase();
            _spawner.Spawn();
            timer = 0.0f;
        }

        if (timer >= 30.0f || Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("State changed (-->Cast)");
            nextState = State.CastPhase;
        }
    }

    void OnStateCastPhase()
    {
        if (isFirstFrame)
        {
            IVUIManager.I.ChangeRightButtonText("Cast");
            IVUIManager.I.RefreshInCastPhase();
            _spawner.Release();
            timer = 0.0f;

        }

        if (timer >= 30.0f || Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("State changed (-->Monster)");
            nextState = State.MonsterPhase;
        }
    }
  
}
