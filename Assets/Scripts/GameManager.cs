using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Photon.PunBehaviour
{
    static public GameManager Instance;
    // public GameObject playerPrefab;

    // Use this for initialization
    void Start()
    {
        Instance = this;

        if (Player.LocalPlayerInstance == null)
        {
            Debug.Log("We are Instantiating LocalPlayer from " + SceneManager.GetActiveScene().name);
            // PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
        }
        else
        {
            Debug.Log("Ignoring scene load for " + SceneManager.GetActiveScene().name);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
	public static RoomInfo[] GetRoomList() {
        Debug.Log("Get Room List Called");
        return PhotonNetwork.GetRoomList();
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    private void LoadArena()
    {
        if (!PhotonNetwork.isMasterClient)
        {
            Debug.LogError("PhotonNetwork : Trying to Load a level but we are not the master Client");
        }
        Debug.Log("PhotonNetwork : Loading Level : " + PhotonNetwork.room.PlayerCount);
        PhotonNetwork.LoadLevel("Room for " + PhotonNetwork.room.PlayerCount);
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

	public void OnClickConfig()
	{
		Debug.Log("OnClickConfig() called");
	}


}

