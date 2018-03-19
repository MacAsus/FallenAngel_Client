using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using Quobject.SocketIoClientDotNet.Client; // SocketIO Client Import
using Newtonsoft.Json;

[Serializable]
public class UserProtocol
{
	public int clientId;
    public int serverId;
    public float x;
    public float y;
    public int room;

    public UserProtocol(int _clientId, int _serverId, int _x, int _y, int _room)
    {
		clientId = _clientId;
        serverId = _serverId;
        x = _x;
        y = _y;
        room = _room;
    }

}

public class UserListProtocol {
	public List<UserProtocol> userList; 
}



public class GeneralScript : MonoBehaviour
{
    // For Server Member variables Static
    public string serverURL = "http://localhost:8081";
    protected static Socket socket = null;
    public static bool isEnter = false;

    // // For Server Member variables Non-Static
    public string chatLog = "";
    public int roomNum = 1;
    public Queue<UserProtocol> moveMsgQ = new Queue<UserProtocol>();
    Dictionary<int, UserProtocol> idHash = new Dictionary<int, UserProtocol>();

    // For client
    public static GeneralScript Instance = null;



    [Range(0, 3)]
    public static int selectedObj = 0;
    private int prevNum;
    [Range(0, 4)]
    public static int objNum = 0;

    public GameObject tempObj;
    private GameObject[] tempArr = new GameObject[4];

    // initialize user map
    public void initUserMap()
    {
        UserProtocol user = new UserProtocol(-1, -1, 0, 0, -1);
        for (int i = 0; i < 4; i++)
        {
            idHash.Add(i, user);
        }
    }

    //포지션받기
    public Vector3 GetObjPos(int tempIndex)
    {
        return tempArr[tempIndex].transform.position;
    }
    //포지션set
    public void SetObjPos(int tempIndex, Vector3 tempVec)
    {
		Debug.Log("SetObjPos : " +tempVec.x + " : " + tempVec.y);
        tempArr[tempIndex].transform.position = tempVec;
    }
    //오브젝트 id 받기
    public int GetObjIndex(int tempIndex)
    {
        if (tempIndex < objNum)
        {
            return tempArr[tempIndex].GetComponent<objTempScripts>().objIndex;
        }
        else
        {
            return -1;
        }
    }

    // 유져 위치 전송
    public static void SendUserPos(int objNum, float xPos, float yPos)
    {
        if (socket != null && isEnter == true)
        {
            string info = "[" + objNum + ", " + xPos + ", " + yPos + "]";
            socket.Emit("userPos", info); // [objNum, xPos, yPos]
        }
    }



    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        openServerConn();
        prevNum = objNum;

        for (int i = 0; i < 4; i++)
        {
            tempArr[i] = Instantiate(tempObj, Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 10)), Quaternion.identity);
            tempArr[i].GetComponent<objTempScripts>().objIndex = i;
			tempArr[i].GetComponent<objTempScripts>().moveEnalbe = false;
        }
    }

    void OnDestroy()
    {
        closeServerConn();
        Debug.Log("Destroy Called");
    }

    void Update()
    {
        if (objNum != prevNum)
        {
			for(int i = 0; i < 4; i++) {
				tempArr[i].GetComponent<objTempScripts>().moveEnalbe = false;
			}

			tempArr[selectedObj].GetComponent<objTempScripts>().moveEnalbe = true;
			prevNum = objNum;
        }

		if (moveMsgQ.Count != 0)
        {
            UserProtocol user = moveMsgQ.Dequeue();
            Vector3 v = new Vector3(user.x, user.y);
            SetObjPos(user.clientId, v);
        }
    }

    void openServerConn()
    {
        if (socket == null)
        {
            socket = IO.Socket(serverURL);
            socket.On(Socket.EVENT_CONNECT, () =>
            {
                Debug.Log("Server Connected");
                globalChat("Hello!"); // Todo: Connect with UI Btn
                enterRoom(roomNum); // Todo: Connect with UI Btn
                listingRoom(); // Todo: Connect with UI Btn
            });

            socket.On("allPlayers", (data) =>
            {
                string str = data.ToString();
				List<UserProtocol> users = JsonConvert.DeserializeObject<UserListProtocol>(str).userList;

				
				Debug.Log("Total User Count: " + users.Count);
				for(int i=0; i < users.Count; i++) {
					UserProtocol user = users[i];
					user.clientId = objNum;
					// hash 추가
					idHash[objNum] = user;

					// user 위치 업데이트, queue에 넣어두면 업데이트 된다.
					moveMsgQ.Enqueue(user);
					Debug.Log(user.clientId + " Queued");
					objNum++;
				}

				// 현재 유져 선택
				selectedObj = users.Count - 1;
            });

			socket.On("newPlayer", (data) => {
				string str = data.ToString();
				UserProtocol user = JsonConvert.DeserializeObject<UserProtocol>(str);
				user.clientId = objNum++;
				moveMsgQ.Enqueue(user);
				Debug.Log("newPlayer: " + user.serverId);
			});

            socket.On("globalChat", (data) =>
            {
                Debug.Log("globalChat: " + data);
            });

            socket.On("listRoom", (data) =>
            {
                Debug.Log("room List: " + data);
            });

            socket.On("userPos", (data) =>
            {
                string str = data.ToString();
                UserProtocol user = JsonConvert.DeserializeObject<UserProtocol>(str);
				// user 위치 업데이트, queue에 넣어두면 업데이트 된다.
                moveMsgQ.Enqueue(user);

                // Debug.Log("got userPos");
                Debug.Log("ID: " + user.serverId + "Moved to x: " + user.x + " y: " + user.y);
            });

            socket.On("remove", (data) =>
            {
                string str = data.ToString();
                UserProtocol user = JsonConvert.DeserializeObject<UserProtocol>(str);
                UserProtocol thatUser = idHash[user.serverId];
                thatUser.room = -1;
                thatUser.x = -1000;
                thatUser.y = -1000;
                thatUser.serverId = -1;
				thatUser.clientId = -1;
            });
        }
    }

    void globalChat(string msg)
    {
        socket.Emit("globalChat", msg);
    }

    void listingRoom()
    {
        socket.Emit("listRoom", "");
    }

    void enterRoom(int roomNum)
    {
        if (socket != null)
        {
            Debug.Log("입장 try success");
            socket.Emit("newPlayer", roomNum.ToString());
            isEnter = true;
        }
        else
        {
            Debug.Log("입장 try fail");
        }
    }

    void roomList()
    {
        if (socket != null)
        {
            socket.Emit("listRoom", "");
        }
    }

    void closeServerConn()
    {
        if (socket != null)
        {
            socket.Emit("disconnect", "");
            socket.Disconnect();
            socket = null;
        }
    }
}