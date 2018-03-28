using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CreateRoomGui : Photon.PunBehaviour {

	// Use this for initialization
	public Text displayText;

	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickCancel() {
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

	public void OnClickCreate() {
        string roomName = displayText.text;
		if(PhotonNetwork.connected) {
            CreateRoom(roomName);
        } else {
			
		}
        
    }

	public void CreateRoom(string roomName) {
        PhotonNetwork.CreateRoom(roomName, new RoomOptions() { MaxPlayers = Launcher.MaxPlayersPerRoom, IsVisible = true }, null);
        // TypedLobby.Default
    }

	public override void OnJoinedRoom()
    {
        // networkState.text = "OnJoinedRoom...";
        Debug.Log("Launcher: OnJoinedRoom() called by PUN. Now this client is in a room.");

        if (PhotonNetwork.room.PlayerCount == 1)
        {
            Debug.Log("We load the 'Room for ' " + PhotonNetwork.room.Name);
        }

        // set player name to integer player count
        PhotonNetwork.playerName = PhotonNetwork.room.PlayerCount+"";

        SceneManager.LoadScene("RoomWaiting", LoadSceneMode.Single);
    }
}
