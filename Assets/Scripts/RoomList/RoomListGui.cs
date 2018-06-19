using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomListGui : Photon.PunBehaviour
{

    public static string _gameVersion = "1";
    public PhotonLogLevel LogLevel = PhotonLogLevel.ErrorsOnly;
    public static byte MaxPlayersPerRoom = 4;
    private static bool isConnecting = false;


    /*****************
	 * Unity LifeCycle
	*****************/
    void Awake()
    {
        PhotonNetwork.logLevel = LogLevel;
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.automaticallySyncScene = true;   
        Connect();
    }

    // Use this for initialization
    void Start()
    {
        if(PhotonNetwork.connectionState == ConnectionState.Connected) {
            GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().CompleteLoading();
        }

        SetUserNickName();
    }

    // Update is called once per frame
    void Update()
    {

    }

    /*****************
	 * GUI Trigger
	 *****************/
    public void OnClickCreate()
    {
        SceneManager.LoadScene("CreateRoom", LoadSceneMode.Single);
    }

    public void OnClickFastJoin()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    /*****************
	 * Custom Method
	******************/
    public static void Connect()
    {
        // networkState.text = "Now Connecting...";
        isConnecting = true;

        if (!PhotonNetwork.connected)
        {
            PhotonNetwork.ConnectUsingSettings(_gameVersion);
        }
    }

    public void OnClickBack() {
        SceneManager.UnloadSceneAsync("RoomList");
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }


    /*****************
	 * Photon Event
	*****************/
    public override void OnJoinedLobby()
    {
        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().SetTextStatus("Joining Lobby");
        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().SetSliderPercentage(20);
    }

    public void OnStatusChanged(ExitGames.Client.Photon.StatusCode statusCode) {
        Debug.Log("으아아");
    }

    public override void OnFailedToConnectToPhoton(DisconnectCause cause)
    {
        Debug.Log(cause);
    }

    public override void OnConnectedToMaster()
    {
        // networkState.text = "OnConnectedToMaster...";
        Debug.Log("=========Launcher: OnConnectedToMaster() was called by PUN=========");
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

    public override void OnPhotonCreateRoomFailed(object[] codeAndMsg)
    {
        // networkState.text = "OnPhotonCreateRoomFailed...";
        Debug.Log("Launcher:OnPhotonCreateRoomFailed() was called by PUN.");
    }

    void SetUserNickName() {
        string prefsEmail = PlayerPrefs.GetString("email"); // PlayerPref에 저장하고 씬 이동
        if (!string.IsNullOrEmpty(prefsEmail)) // already login
        {
            PhotonNetwork.player.NickName = prefsEmail;
        }
    }
}