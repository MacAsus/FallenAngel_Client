using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Item {
	public string roomName;
	public string roomState;
}

public class RoomScrollList : MonoBehaviour {
	public List<Item> roomList;
	public Transform contentPanel;
	public RoomObjectPool roomObjectPool;


	// Use this for initialization
	void Start () {
		RefreshDisplay();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// AddButtons
	private void AddRooms() {
		for(int i = 0; i < roomList.Count; i++) {
			Item item = roomList[i];
			GameObject newButton = roomObjectPool.GetObject();
			newButton.transform.SetParent(contentPanel);
			SampleBtn sampleBtn = newButton.GetComponent<SampleBtn>();
			sampleBtn.Setup(item, this);
		}
	}

	void RefreshDisplay() {
        // RemoveButtons ();
        AddButtons ();
    }

	private void RemoveButtons()
    {
        while (contentPanel.childCount > 0) 
        {
            GameObject toRemove = transform.GetChild(0).gameObject;
            roomObjectPool.ReturnObject(toRemove);
        }
    }

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
}
