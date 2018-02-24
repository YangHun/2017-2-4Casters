using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class IVGameManager : NetworkBehaviour {

    NetworkIdentity _identity;
    NetworkLobbyManager _lobby;
    [SerializeField]
    IVUIManager _ui;

    public List<IVPlayer> Players = new List<IVPlayer>();
    public NetworkIdentity myPlayer = null;

    //Handling Monster
    [SerializeField]
    IVMonsterSpawner _spawner;
    

    private void Start()
    {
        Debug.Log("enter? " + gameObject.name);

        GameObject[] plyrs = GameObject.FindGameObjectsWithTag("Player");
        foreach (GameObject p in plyrs)
        {
            Players.Add(p.GetComponent<IVPlayer>());
        }

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
        
    }
    

    [ClientRpc]
    public void RpcOnState(State currentState, bool isFirstFrame, float timer) {

        //Debug.Log(currentState);

        //OnState function is called on each frame
        switch (currentState)
        {
            case State.MonsterPhase:
                OnStateMonsterPhase(isFirstFrame, timer);
                break;
            case State.CastPhase:
                OnStateCastPhase(isFirstFrame, timer);
                break;
            case State.EndPhase:
				_ui.EndGame(Players[0].playerName);
				currentState = State.Null;
				break;
            case State.Null:
                break;
        }
        
    }
    
    //OnState functions definition

    void OnStateMonsterPhase(bool isFirstFrame, float timer)
    {
        if (isFirstFrame)
        {
            _ui._loading.gameObject.SetActive(false);
            foreach (IVPlayer p in Players)
                p.ResetPlayers();
            _ui.ChangeRightButtonText("Attack");
            _ui.ResetPlayerKeywordText();
            _ui.RefreshInMonsterPhase();
            _ui.InitMonsterButtons();
            _spawner.Spawn();
			RefreshSkill();
            timer = 0.0f;
        }

        if (timer >= 30.0f || Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("State changed (-->Cast)");
            GameObject.Find("Host Server").GetComponent<IVHostServer>().SetNextState(State.CastPhase);
        }
    }

    void OnStateCastPhase(bool isFirstFrame, float timer)
    {
        if (isFirstFrame)
        {
            _ui.ChangeRightButtonText("Cast");
            _ui.RefreshInCastPhase();
            _ui.InitMonsterButtons();
            _spawner.Release();
			RefreshAttack();
            timer = 0.0f;

        }

        if (timer >= 30.0f || Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("State changed (-->Monster)");
            GameObject.Find("Host Server").GetComponent<IVHostServer>().SetNextState(State.MonsterPhase);
        }
    }
  
	void RefreshAttack()
	{
		if (!isServer) return;
		GameObject g = GameObject.Find("Bullets");
		Transform[] c = g.GetComponentsInChildren<Transform>();
		foreach(Transform o in c)
			NetworkServer.Destroy(o.gameObject);
	}
  
	void RefreshSkill()
	{
		if (!isServer) return;
		GameObject g = GameObject.Find("Skills");
		Transform[] c = g.GetComponentsInChildren<Transform>();
		foreach(Transform o in c)
			NetworkServer.Destroy(o.gameObject);
	}
}
