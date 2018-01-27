using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IVGameManager : MonoBehaviour {
    IVUIManager _ui;

    //Handling Game Flow FSM 
    public enum State { MonsterPhase, CastPhase, Null }

    const State startState = State.MonsterPhase;
    State currentState = State.Null;
    State nextState = State.Null;
    bool isFirstFrame = true;

    float timer = 0.0f;

    //Handling Player
    List<IVPlayer> _players;

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


    void Start()
    {
		_players = new List<IVPlayer>();
        _ui = GetComponent<IVUIManager>();
        currentState = startState;
        OnStateMonsterPhase();
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

	public void registerPlayer(IVPlayer player)
	{
		if (_players.Contains(player))
			Debug.Log(player.name + " is already exists.");
		else
			_players.Add(player);
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
