using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RoomWaitingGui : Photon.PunBehaviour
{

    public Text roomStateUI;
    public GameObject CanvasObject;

    public GameObject player1_name;
    public GameObject player1_image;
    public GameObject player2_name;
    public GameObject player2_image;
    public GameObject player3_name;
    public GameObject player3_image;
    public GameObject player4_name;
    public GameObject player4_image;


    List<User> users = new List<User>();
    public List<UserSlot> userSlots = new List<UserSlot>();

    /*****************
	 * Unity LifeCycle
	*****************/
    // Use this for initialization
    void Start()
    {
        // set player name to integer player count
        Debug.Log("======== My Id: " + PhotonNetwork.player.ID);

        _initUserSlot();
        UpdateUserCount();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void Awake()
    {
        PhotonNetwork.OnEventCall += this.OnStartEvent;
    }

    /*****************
	 * GUI Trigger
	 *****************/
    public void OnClickLeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public void OnClickConfig()
    {

    }

    // Only Works by Master Client
    public void OnClickStart()
    {
        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.IsVisible = false;
            SendStartMsg(); // send evt to others
        }
        else
        {
            Debug.Log("Client cannot Start!");
        }
    }
    public void OnClickCharacter()
    {
        CanvasObject.SetActive(false);
        SceneManager.LoadScene("SelectCharacter", LoadSceneMode.Additive);
    }

    /*****************
	 * Custom Method
	******************/
    private void SendStartMsg()
    {
        byte evCode = Events.STARTED_GAME_EVT;    // start event 0.
        RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, options);
    }

    private void OnStartEvent(byte eventcode, object content, int senderid)
    {
        // Debug.Log("OnStartEvent called");
        if (eventcode == Events.STARTED_GAME_EVT) // Master Client Started Game
        {
            // 현 플레이어의 직업 선택
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable (){{"job", userSlots[0].job}};
            PhotonNetwork.player.SetCustomProperties(hash);
            this.LoadSceneToInGame();
        }
        else if (eventcode == Events.SOMEONE_SELECTED_CHARACTER_EVT)
        {
            string job = (string)content; // "senderid" Selected job
            Debug.Log("Someone Selected Job!" + job + " " + senderid);
            setUserJob(job, senderid);
        } 
        else if(eventcode == Events.SOMEONE_SELECTED_WEAPON_EVT) {
            if(senderid == PhotonNetwork.player.ID) {
                CanvasObject.SetActive(true);
            }
            // int weaponNum = (int)content; // "senderid" Selected weaponNum
            Debug.Log("Someone Selected Weapon!");
            Debug.Log(content);
            // setUserWeapon(weaponNum, senderid);
        }
    }

    private void setUserJob(string job, int userID) {
        for(int i = 0; i < PhotonNetwork.room.PlayerCount; i++) {
            if(users[i].userID == userID) {
                Debug.Log(" I'll set" + i + "To " + job);
                userSlots[i].userName.GetComponent<Text>().text = job;
                userSlots[i].job = job;
                break;
            }
        }
    }

    private void setUserWeapon(int weaponNum, int userID) {
        for(int i = 0; i < PhotonNetwork.room.PlayerCount; i++) {
            if(users[i].userID == userID) {
                userSlots[i].userName.GetComponent<Text>().text += " " + weaponNum;
                userSlots[i].weaponNum = weaponNum;
                break;
            }
        }
    }

    private void LoadSceneToInGame()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }

    // when User Leave
    public override void OnLeftRoom()
    {
        Debug.Log("Room Left!");
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
    }

    private void UpdateUserCount()
    {
        users = new List<User>();
        Debug.Log("========UpdateUserCount Called ==========");
        _initUserSlot();

        // Add my Users data (because of joined At the end)
        var myUser = new User("IsMine!!!", PhotonNetwork.player.ID);
        users.Add(myUser);

        // Add Other Users data
        foreach (PhotonPlayer player in PhotonNetwork.otherPlayers)
        {
            var user = new User(player.NickName + "", player.ID);
            Debug.Log("player others: " + player.NickName);
            users.Add(user);
        }

        // Enable User Slot UI
        foreach (User user in users)
        {
            int idx = users.IndexOf(user);
            userSlots[idx].userName.GetComponent<Text>().text = user.userName;
            userSlots[idx].userImg.SetActive(true);
            userSlots[idx].userName.SetActive(true);
            userSlots[idx].isVisible = true;
        }

        // Disable User Slot UI
        for (int i = 0; i < Launcher.MaxPlayersPerRoom; i++)
        {
            if (!userSlots[i].isVisible)
            {
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


    /*****************
	 * Photon Event
	*****************/
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

    [PunRPC]
    private void StartRoomRPC()
    {
        SceneManager.LoadScene("InGame", LoadSceneMode.Single);
    }
}


public class User
{
    public User(string _userName, int _userID = -1)
    {
        userName = _userName;
        userID = _userID;
    }
    public string userName;
    public int userID;
}

public class UserSlot
{
    public UserSlot(GameObject _userName, GameObject _userImg, bool _isVisible = false)
    {
        userName = _userName;
        userImg = _userImg;
        isVisible = _isVisible;
        job = "";
        weaponNum = -1;
    }

    public GameObject userName;
    public GameObject userImg;
    public bool isVisible;
    public string job;
    public int weaponNum;
}