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

	// When User Entered
	public override void OnJoinedRoom() {
		
	}

	// when User Leave
	public override void OnLeftRoom() {

	}

	public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerConnected() " + other.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient);
            // LoadArena();
        }
    }

    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        Debug.Log("OnPhotonPlayerDisconnected() " + other.NickName);

        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient);

            // LoadArena();
        }
    }

}