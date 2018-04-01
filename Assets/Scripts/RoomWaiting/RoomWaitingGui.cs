﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomWaitingGui : Photon.PunBehaviour
{

    public Text roomStateUI;

    public GameObject player1_name;
    public GameObject player1_image;
    public GameObject player2_name;
    public GameObject player2_image;
    public GameObject player3_name;
    public GameObject player3_image;
    public GameObject player4_name;
    public GameObject player4_image;

    public List<UserSlot> userSlots = new List<UserSlot>();

    // Use this for initialization
    void Start()
    {        
        // set player name to integer player count
        Debug.Log("======== My Id: " + PhotonNetwork.playerName);

        _initUserSlot();
        UpdateUserCount();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickConfig()
    {

    }

    public void OnClickStart()
    {
        SceneManager.LoadScene("Scene1", LoadSceneMode.Single);
    }

    // when User Leave
    public override void OnLeftRoom()
    {
        Debug.Log("Room Left!");
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    // Triggered when Player enter
    public override void OnPhotonPlayerConnected(PhotonPlayer other)
    {
        // PhotonNetwork.NickName = "직업이름";
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerConnected isMasterClient " + PhotonNetwork.isMasterClient + " : " + other.ID);
        }
        UpdateUserCount();
    }

    // Triggered when Player leave
    public override void OnPhotonPlayerDisconnected(PhotonPlayer other)
    {
        if (PhotonNetwork.isMasterClient)
        {
            Debug.Log("OnPhotonPlayerDisonnected isMasterClient " + PhotonNetwork.isMasterClient);
        }

        UpdateUserCount();
    }

    private void UpdateUserCount()
    {
        List<User> users = new List<User>();
        Debug.Log("========UpdateUserCount Called ==========");
        _initUserSlot();

        // Add my Users data (because of joined At the end)
        var myUser = new User("IsMine!!!");
        users.Add(myUser);

        // Add Other Users data
        foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
        {
            var user = new User(player.NickName + "");
            Debug.Log("player others: " + player.NickName);
            users.Add(user);
        }

        

        // Enable User Slot UI
        foreach (User user in users) {
            int idx = users.IndexOf(user);
            userSlots[idx].userName.GetComponent<Text>().text = user.userName;
            userSlots[idx].userImg.SetActive(true);
            userSlots[idx].userName.SetActive(true);
            userSlots[idx].isVisible = true;
        }

        // Disable User Slot UI
        for (int i = 0; i < Launcher.MaxPlayersPerRoom; i++)
        {
            if(!userSlots[i].isVisible) {
                // Debug.Log("I'll disabled index " + i);
                userSlots[i].userImg.SetActive(false);
                userSlots[i].userName.SetActive(false);
                userSlots[i].userName.GetComponent<Text>().text = "";
            }
        }

        roomStateUI.text = PhotonNetwork.room.PlayerCount + " / " + PhotonNetwork.room.MaxPlayers;
    }

    private void _initUserSlot()
    {
        Debug.Log("========_initUserSlot Called ==========");
        userSlots.Clear();
        
        UserSlot slot_user1 = new UserSlot(player1_name, player1_image);
        UserSlot slot_user2 = new UserSlot(player2_name, player2_image);
        UserSlot slot_user3 = new UserSlot(player3_name, player3_image);
        UserSlot slot_user4 = new UserSlot(player4_name, player4_image);

        userSlots.Add(slot_user1);
        userSlots.Add(slot_user2);
        userSlots.Add(slot_user3);
        userSlots.Add(slot_user4);
    }

}


public class User
{
    public User(string _userName)
    {
        userName = _userName;
    }
    public string userName;
}

public class UserSlot
{
    public UserSlot(GameObject _userName, GameObject _userImg, bool _isVisible = false)
    {
        userName = _userName;
        userImg = _userImg;
        isVisible = _isVisible;
    }

    public GameObject userName;
    public GameObject userImg;
    public bool isVisible;
}