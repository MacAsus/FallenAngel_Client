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

    List<GameObject> userPhotos = new List<GameObject>();
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
            this.startGame();
        }
        else if (eventcode == Events.SOMEONE_SELECTED_CHARACTER_EVT)
        {
            string job = (string)content; // "senderid" Selected job
            Debug.Log("Someone Selected Job!" + job + " " + senderid);
            setUserJob(job, senderid);

            // 현 플레이어의 직업 선택을 네트워크에 적용
            if (senderid == PhotonNetwork.player.ID)
            {
                ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "job", job } };
                PhotonNetwork.player.SetCustomProperties(hash);
                CanvasObject.SetActive(true);
            }
        }
    }

    private void setUserJob(string job, int userID)
    {
        for (int i = 0; i < PhotonNetwork.room.PlayerCount; i++)
        {
            if (userSlots[i].userID == userID)
            {
                Debug.Log(" I'll set" + i + "To " + job);
                userSlots[i].userName.GetComponent<Text>().text = job;
                userSlots[i].job = job;
                Sprite jobSprite = Resources.Load("Character/Attacker-OnlyImg") as Sprite;
                Debug.Log("jobSprite is" + jobSprite);
                userPhotos[i].GetComponent<Image>().sprite = jobSprite;
                break;
            }
        }
    }

    private void startGame() {
        // If Someone does not select job, then cannot start
        if(checkIsAllPlayerSelectJob()) {
            this.LoadSceneToInGame();
        } else {
            Alert alert = new Alert("Alert", "All Player Should Select their own job");
            AlertTriggers.TriggerAlert(alert);
            Debug.Log("직업선택 안 해서 고 불가함");
        }
    }

    private bool checkIsAllPlayerSelectJob() {
        bool isAllPlayerSelectedJob = true;

        foreach(PhotonPlayer player in PhotonNetwork.playerList) {
            if (!player.CustomProperties.ContainsKey("job") || string.IsNullOrEmpty((string)player.CustomProperties["job"])) {
                isAllPlayerSelectedJob = false;
            }
        }
        return isAllPlayerSelectedJob;
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

        // Clear User Custom Properties
        ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable() { { "job", null } };
        PhotonNetwork.player.SetCustomProperties(hash);
    }

    private void UpdateUserCount()
    {
        List<PhotonPlayer> playerQueue = new List<PhotonPlayer>();

        Debug.Log("========UpdateUserCount Called ==========");
        _initUserSlot();
        // Add my User data (because of joined At the end)
        playerQueue.Add(PhotonNetwork.player);

        // Add Other User
        foreach (PhotonPlayer otherPlayer in PhotonNetwork.otherPlayers)
        {
            playerQueue.Add(otherPlayer);
        }

        for (int i = 0; i < playerQueue.Count; i++)
        {
            PhotonPlayer player = playerQueue[i];

            if (player.CustomProperties.ContainsKey("job") && !string.IsNullOrEmpty((string)player.CustomProperties["job"]))
            {
                userSlots[i].job = (string)player.CustomProperties["job"];
                userSlots[i].userName.GetComponent<Text>().text = userSlots[i].job;
                Debug.Log("직업이 이썽요!" + userSlots[i].job);
            }
            else
            {
                userSlots[i].userName.GetComponent<Text>().text = "Player " + player.ID;
            }

            userSlots[i].userID = player.ID;
            userSlots[i].userImg.SetActive(true);
            userSlots[i].userName.SetActive(true);
            userSlots[i].isVisible = true;
            Debug.Log("Player " + player.ID);
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

    // Todo: Unity Array로 인스펙터에서 받아오기
    private void _initUserSlot()
    {
        Debug.Log("========_initUserSlot Called ==========");
        userSlots.Clear();

        UserSlot slot_user1 = new UserSlot(-1, player1_name, player1_image);
        UserSlot slot_user2 = new UserSlot(-1, player2_name, player2_image);
        UserSlot slot_user3 = new UserSlot(-1, player3_name, player3_image);
        UserSlot slot_user4 = new UserSlot(-1, player4_name, player4_image);

        userSlots.Add(slot_user1);
        userSlots.Add(slot_user2);
        userSlots.Add(slot_user3);
        userSlots.Add(slot_user4);

        userPhotos.Add(player1_image);
        userPhotos.Add(player2_image);
        userPhotos.Add(player3_image);
        userPhotos.Add(player4_image);
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


public class UserSlot
{
    public UserSlot(int _userID, GameObject _userName, GameObject _userImg, bool _isVisible = false)
    {
        userName = _userName;
        userImg = _userImg;
        isVisible = _isVisible;
        job = "";
        weaponNum = -1;
    }

    public int userID;
    public GameObject userName;
    public GameObject userImg;
    public bool isVisible;
    public string job;
    public int weaponNum;
}