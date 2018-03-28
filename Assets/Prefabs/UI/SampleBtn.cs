using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SampleBtn : MonoBehaviour {
	public Button button;
	public Text roomName;
	public Text roomState;

	private Item item;
    private RoomScrollList roomList;

	// Use this for initialization
	void Start () {
		
	}

	public void Setup(Item currentItem, RoomScrollList currentScrollList)
    {
        item = currentItem;
        roomName.text = item.roomName;
        roomState.text = item.roomState;
        roomList = currentScrollList;
        
    }
    
    public void HandleClick()
    {
        
    }
}
