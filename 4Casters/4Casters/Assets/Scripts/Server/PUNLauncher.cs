using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon;

public class PUNLauncher : PunBehaviour {

    [Tooltip("The Ui Panel to let the user enter name, connect and play")]
    public GameObject controlPanel;
    [Tooltip("The UI Label to inform the user that the connection is in progress")]
    public GameObject progressLabel;

    public PhotonLogLevel Loglevel = PhotonLogLevel.Informational;
    string _gameVersion = "1";
    public byte MaxPlayersPerRoom = 4;
    
    bool isConnecting = false;

    void Awake()
    {

        // #NotImportant
        // Force Full LogLevel
        PhotonNetwork.logLevel = PhotonNetwork.logLevel = Loglevel;

        // #Critical
        // we don't join the lobby. There is no need to join a lobby to get the list of rooms.
        PhotonNetwork.autoJoinLobby = false;

        // #Critical
        // this makes sure we can use PhotonNetwork.LoadLevel() on the master client and all clients in the same room sync their level automatically
        PhotonNetwork.automaticallySyncScene = true;
    }

    // Use this for initialization
    void Start()
    {
        //Connect();
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);
    }

    public void Connect()
    {
        isConnecting = true;

        progressLabel.SetActive(true);
        controlPanel.SetActive(false);

        // we check if we are connected or not, we join if we are , else we initiate the connection to the server.
        if (PhotonNetwork.connected)
        {
            // #Critical we need at this point to attempt joining a Random Room. If it fails, we'll get notified in OnPhotonRandomJoinFailed() and we'll create one.
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            // #Critical, we must first and foremost connect to Photon Online Server.
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }
    public override void OnConnectedToMaster()
    {
        Debug.Log("DemoAnimator/Launcher: OnConnectedToMaster() was called by PUN");

        if (isConnecting)
        // #Critical: The first we try to do is to join a potential existing room. If there is, good, else, we'll be called back with OnPhotonRandomJoinFailed()  
            PhotonNetwork.JoinRandomRoom();
    }
    public override void OnJoinedRoom()
    {
        
        Debug.Log("DemoAnimator/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        // #Critical: We only load if we are the first player, else we rely on  PhotonNetwork.automaticallySyncScene to sync our instance scene.
        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for 1' ");

            // #Critical
            // Load the Room Level. 
            PhotonNetwork.LoadLevel("Room for 1");
        }

    }

    public override void OnDisconnectedFromPhoton()
    {
        progressLabel.SetActive(false);
        controlPanel.SetActive(true);

        Debug.LogWarning("DemoAnimator/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log("DemoAnimator/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");

        // #Critical: we failed to join a random room, maybe none exists or they are all full. No worries, we create a new room.
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    
}
