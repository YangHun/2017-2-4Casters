using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class TitleUIManager : MonoBehaviour {

	NetworkLobbyManager Nlobby;

	public void OnClickCreateGame(Button b)
	{
		Nlobby.OnLobbyStartServer();
		Nlobby.OnLobbyStartHost();

		return;
	}

	// Use this for initialization
	void Start () {
		Nlobby = GameObject.Find("NetworkSystem").GetComponent<NetworkLobbyManager>();
		Nlobby.showLobbyGUI = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
