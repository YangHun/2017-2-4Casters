using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//This script creates players both local player and other players.
public class PlayerSpawner : NetworkBehaviour {

	[SerializeField]
	GameObject playerPrefab;

	// Use this for initialization
	void Start()
	{
		//		GameObject player = GameObject.Instantiate(playerPrefab);           //spawn player with prefab
		Player[] players = FindObjectsOfType<Player>();
		foreach (Player player in players)
		{
			player.transform.SetParent(this.transform);
			player.transform.rotation = Quaternion.Euler(new Vector3(90, 0, 0));
			switch ((int)(Random.value * 4))
			{
				case 0:
					player.transform.position = new Vector3(5, 0.76f, 0);
					break;
				case 1:
					player.transform.position = new Vector3(-5, 0.76f, 0);
					break;
				case 2:
					player.transform.position = new Vector3(0, 0.76f, 5);
					break;
				case 3:
					player.transform.position = new Vector3(0, 0.76f, -5);
					break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
