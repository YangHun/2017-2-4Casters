using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {


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
    int spawnCount = 10;

    public List<Player> Players
    {
        get
        {
            return _players;
        }
    }
    


	void Start () {

        //initialization
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject obj in objs)
        {
            _players.Add(obj.GetComponent<Player>());
        }


        currentState = startState;
	}
	
	void Update () {

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
            _spawner.Spawn(spawnCount);
            timer = 0.0f;
        }

        if (timer >= 10.0f)
        {
            nextState = State.CastPhase;
        }
    }

    void OnStateCastPhase()
    {
        if (isFirstFrame)
        {
            timer = 0.0f;
        }

        if (timer >= 30.0f)
        {
            nextState = State.MonsterPhase;
        }
    }
}
