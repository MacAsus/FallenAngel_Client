using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Launcher : Photon.PunBehaviour
{

    string _gameVersion = "1";
    public PhotonLogLevel LogLevel = PhotonLogLevel.Informational;
    public byte MaxPlayersPerRoom = 4;
    private bool isConnecting;
    public Text networkState;
    private void Awake()
    {
        PhotonNetwork.logLevel = LogLevel;
        PhotonNetwork.autoJoinLobby = false;
        PhotonNetwork.automaticallySyncScene = true;
    }

    // Use this for initialization
    void Start()
    {
        Connect();
    }

    public void Connect()
    {
        networkState.text = "Now Connecting...";
        isConnecting = true;

        if (PhotonNetwork.connected)
        {
            PhotonNetwork.JoinRandomRoom();
        }
        else
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public override void OnConnectedToMaster()
    {
        networkState.text = "OnConnectedToMaster...";
        Debug.Log("PhotonDemo/Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting)
        {
            PhotonNetwork.JoinRandomRoom();
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        networkState.text = "OnDisconnectedFromPhoton...";
        Debug.LogWarning("PhotonDemo/Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        networkState.text = "OnPhotonRandomJoinFailed...";
        Debug.Log("PhotonDemo/Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        networkState.text = "OnJoinedRoom...";
        Debug.Log("PhotonDemo/Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            networkState.text = "Room Joined...";
            Debug.Log("We load the 'Room for 1' ");

            // PhotonNetwork.LoadLevel("Room for 1");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
