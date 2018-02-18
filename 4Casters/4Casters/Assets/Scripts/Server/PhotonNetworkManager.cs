using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;

public class PhotonNetworkManager : MonoBehaviour {

    [SerializeField]
    Text connectionText;
    [SerializeField]
    Transform[] spawnPoint;
    [SerializeField]
    Camera sceneCamera;

	// Use this for initialization
	void Start () {
        PhotonNetwork.logLevel = PhotonLogLevel.Full;
        PhotonNetwork.ConnectUsingSettings("0.2");
	}
	
	// Update is called once per frame
	void Update () {
        connectionText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}


    void OnJoinedLobby()
    {
        RoomOptions ro = new RoomOptions() { IsVisible = true, MaxPlayers = 10 };
        PhotonNetwork.JoinOrCreateRoom("Mike", ro, TypedLobby.Default);
    }

    void OnJoinedRoom()
    {
        //        StartSpawnProcess(0f);
        Debug.Log("Joined!");
    }

    void StartSpawnProcess (float respawnTime)
    {
  //      sceneCamera.enabled = true;
  //      StartCoroutine()
    }

}
