using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item {
	public string roomName;
	public string roomState;

    public Item(string _roomName, string _roomState) {
        roomName = _roomName;
        roomState = _roomState;
    }
}

public class RoomScrollList : Photon.PunBehaviour {
    public static RoomInfo[] rooms = { };
	public List<Item> roomList;
	public Transform contentPanel;
	public RoomObjectPool roomObjectPool;

    /*****************
	 * Unity LifeCycle
	*****************/
    void Awake() {
        RefreshDisplay();
        Debug.Log("RoomScroll에서 본 룸 갯수" + rooms.Length);
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    /*****************
	 * Custom Method
	******************/
    void RefreshDisplay() {
        RemoveButtons ();
        // AddButtons ();
    }

    // AddButtons
	private void RemoveButtons()
    {
        while (contentPanel.childCount > 0) 
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            roomObjectPool.ReturnObject(toRemove);
        }
    }

    // AddButtons
	private void AddButtons()
    {
        for (int i = 0; i < roomList.Count; i++)
        {
            Item item = roomList[i];
            GameObject newButton = roomObjectPool.GetObject();
            newButton.transform.SetParent(contentPanel);

            SampleBtn sampleBtn = newButton.GetComponent<SampleBtn>();
            sampleBtn.Setup(item, this);
        }
    }

	void AddItem(Item itemToAdd, RoomScrollList roomScrollList)
    {
        roomScrollList.roomList.Add (itemToAdd);
    }

    private void RemoveItem(Item itemToRemove, RoomScrollList roomScrollList)
    {
        for (int i = roomScrollList.roomList.Count - 1; i >= 0; i--) 
        {
            if (roomScrollList.roomList[i] == itemToRemove)
            {
                roomScrollList.roomList.RemoveAt(i);
            }
        }
    }
    
    /*****************
	 * Photon Event
	*****************/
    public override void OnReceivedRoomListUpdate() {
        Debug.Log("Room Count" + PhotonNetwork.countOfRooms);
        rooms = PhotonNetwork.GetRoomList();;
        roomList.Clear(); // clear room list
        foreach(var room in rooms) {
            Debug.Log("Found room: " + room);
            Item item = new Item(room.Name, room.PlayerCount + " / " + room.MaxPlayers);
            roomList.Add(item);
        }
        Debug.Log("Total rooms count: " + rooms.Length);

        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().SetTextStatus("Network Loading Completed");
        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().SetSliderPercentage(100);
        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().DisableSlider();
        GameObject.FindWithTag("GUI").GetComponent<LoadingGui>().EnableScreen();

        RemoveButtons();
        AddButtons ();
    }
}
