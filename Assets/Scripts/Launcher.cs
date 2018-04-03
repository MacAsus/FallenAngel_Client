using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Launcher : Photon.PunBehaviour
{

    public string _gameVersion = "1";
    public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;
    public static byte MaxPlayersPerRoom = 4;
    private bool isConnecting;
    private void Awake()
    {
        if (!PhotonNetwork.connected) {
            PhotonNetwork.logLevel = LogLevel;
            PhotonNetwork.autoJoinLobby = true;
            PhotonNetwork.automaticallySyncScene = true;
        }

        Connect();
    }

    // Use this for initialization
    void Start()
    {
        
    }

    public  void Connect()
    {
        // networkState.text = "Now Connecting...";
        isConnecting = true;

        if (!PhotonNetwork.connected) {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void joinRandomRoom() {
        PhotonNetwork.JoinRandomRoom();
    }

    public void CreateRoom(string roomName) {
        if(isConnecting) {
            PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
        } else {
            Debug.Log("Photon Does not connected");
        }
    }

    public override void OnConnectedToMaster()
    {
        // networkState.text = "OnConnectedToMaster...";
        Debug.Log("Launcher: OnConnectedToMaster() was called by PUN");
        if (isConnecting)
        {
            
        }
    }

    public override void OnDisconnectedFromPhoton()
    {
        // networkState.text = "OnDisconnectedFromPhoton...";
        Debug.LogWarning("Launcher: OnDisconnectedFromPhoton() was called by PUN");
    }

    public override void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        // networkState.text = "OnPhotonRandomJoinFailed...";
        Debug.Log("Launcher:OnPhotonRandomJoinFailed() was called by PUN. No random room available, so we create one.\nCalling: PhotonNetwork.CreateRoom(null, new RoomOptions() {maxPlayers = 4}, null);");
        PhotonNetwork.CreateRoom(null, new RoomOptions() { MaxPlayers = MaxPlayersPerRoom }, null);
    }

    public override void OnJoinedRoom()
    {
        // networkState.text = "OnJoinedRoom...";
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            // networkState.text = "Room Joined...";
            Debug.Log("We load the 'Room for 1' ");
            // PhotonNetwork.LoadLevel("Room for 1");
        }

        SceneManager.LoadScene("RoomWaiting", LoadSceneMode.Single);
    }

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg) {
        // networkState.text = "OnPhotonCreateRoomFailed...";
        Debug.Log("Launcher:OnPhotonCreateRoomFailed() was called by PUN.");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
