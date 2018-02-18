using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PUNRandomMatchmaker : PunBehaviour {

	// Use this for initialization
	void Start () {

        PhotonNetwork.ConnectUsingSettings("0.1");

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.connectionStateDetailed.ToString());
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    void OnPhotonRandomJoinFailed()
    {
        Debug.Log("Can't join random room!");
        PhotonNetwork.CreateRoom(null);
    }

}
