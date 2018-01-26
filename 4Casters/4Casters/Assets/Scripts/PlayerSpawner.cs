using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//This script creates players both local player and other players.
public class PlayerSpawner : NetworkBehaviour {

	[SerializeField]
	GameObject playerPrefab;

	// Use this for initialization
	void Start () {
		GameObject player = GameObject.Instantiate(playerPrefab);			//spawn player with prefab
		int a = Random.value > 0.5f ? 1 : -1;
		int b = Random.value > 0.5f ? 1 : -1;								//set the pos with random generator
		player.transform.position = new Vector3(-5 * a, 0.76f * b, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
