using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoomWaitingGui : Photon.PunBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
		SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

	public void OnClickConfig() {

	}

	public void OnClickStart() {
		SceneManager.LoadScene("Scene1", LoadSceneMode.Single);
	}

	public override void OnJoinedRoom() {
		
	}
}