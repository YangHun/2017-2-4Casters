using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    UIManager _ui;

    //Handling Game Flow FSM 
    public enum State { MonsterPhase, CastPhase, Null }

    const State startState = State.MonsterPhase;
    State currentState = State.Null;
    State nextState = State.Null;
    bool isFirstFrame = true;

    float timer = 0.0f;

    //Handling Player
    [SerializeField]
    List<Player> _players;

    //Handling Monster
    [SerializeField]
    MonsterSpawner _spawner;
    
    public List<Player> Players
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


	void Start () {

        //initialization
        if (Players.Count == 0)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
            foreach (GameObject obj in objs)
            {
                _players.Add(obj.GetComponent<Player>());
            }
			_ui.RefreshInMonsterPhase();
        }
        _ui = GetComponent<UIManager>();
        currentState = startState;
	}
	
	void Update () {

        timer += Time.deltaTime;

        //OnState function is called on each frame
        switch (currentState)
        {
            case State.MonsterPhase:
                OnStateMonsterPhase();
				_ui.RefreshInMonsterPhase();
                break;
            case State.CastPhase:
                OnStateCastPhase();
				_ui.RefreshInCastPhase();
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
            foreach (Player p in Players)
                p.ResetPlayers();
            _ui.ChangeRightButtonText("Attack");
            _ui.ResetPlayerKeywordText();
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
