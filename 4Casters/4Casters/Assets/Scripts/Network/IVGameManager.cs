using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVGameManager : NetworkBehaviour {
    IVUIManager _ui;
    NetworkIdentity _identity;
    NetworkLobbyManager _lobby;

    //Handling Player
    [SerializeField]
    List<IVPlayer> _players = new List<IVPlayer>();
    [SerializeField]
    SyncListBool _playerstatus = new SyncListBool();

    //Handling Game Flow FSM 
    public enum State { MonsterPhase, CastPhase, Null }

    const State startState = State.MonsterPhase;
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

    public override void OnStartClient()
    {
        base.OnStartClient();

        _ui = GetComponent<IVUIManager>();
        _identity = GetComponent<NetworkIdentity>();
        _lobby = GetComponent<NetworkLobbyManager>();

        IVPlayer[] players = FindObjectsOfType<IVPlayer>();
        _players.Clear();               //stash given arguments to make the array with network behaviour
        if (players.Length > 0)
        {
            foreach (IVPlayer player in players)
            {
                _players.Add(player);
                _playerstatus.Add(false);
            }
        }
        currentState = startState;
    }

    [Command]
    public void CmdClientConnected(int i)
    {
        _playerstatus[i] = true;
    }

    void Update()
    {
        
        timer += Time.deltaTime;

        if (_playerstatus.Contains(false))
        {
            if (isServer)
            {
                for (int i = 0; i < _players.Count; i++)
                {
                    if (!_playerstatus[i])
                    {
                        _players[i].RpcClientConnected(i);
                    }
                }
                if (_playerstatus.Contains(false))
                {
                    Debug.Log("some players are not connected yet-->waiting..");
                    return;
                }
            }
            else
            {
                return;
            }
            
        }

        

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
            _ui.ChangeRightButtonText("Attack");
            _ui.ResetPlayerKeywordText();
            _ui.RefreshInMonsterPhase();
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
            _ui.ChangeRightButtonText("Cast");
            _ui.RefreshInCastPhase();
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
