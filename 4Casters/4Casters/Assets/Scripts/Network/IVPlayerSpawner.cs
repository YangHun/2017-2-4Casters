using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

///////Player Position System/////////
//		0 - West	1 - North		//
//		2 - East	3 - South		//
//			  1						//
//		0			2				//
//			  3						//
//////////////////////////////////////


public class IVPlayerSpawner : NetworkBehaviour
{

	[SerializeField]
	GameObject playerPrefab;

	public static bool[] isPosOccupied = new bool[4];				//true if player object exists in position represented by number

	// Use this for initialization
	void Start()
	{
		//GameObject player = GameObject.Instantiate(playerPrefab);           //spawn player with prefab	//It has been ended by network
		
		IVPlayer[] players = FindObjectsOfType<IVPlayer>();
		foreach (IVPlayer player in players)
		{
			int p;
			do {
				p = (int)(Random.value * 4);
			} while (isPosOccupied[p]);
			switch (p)
			{
			case 0:
				player.InitializeTransform(this.transform, new Vector3(5, 0.76f, 0));
				break;
			case 1:
				player.InitializeTransform(this.transform, new Vector3(-5, 0.76f, 0));
				break;
			case 2:
				player.InitializeTransform(this.transform, new Vector3(0, 0.76f, 5));
				break;
			case 3:
				player.InitializeTransform(this.transform, new Vector3(0, 0.76f, -5));
				break;
			}
			isPosOccupied[p] = true;
		}
		
	}

	// Update is called once per frame
	void Update()
	{

	}
}
