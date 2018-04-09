using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectWeapon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// 네트워크에서 선택한 캐릭터 받아옴

	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*****************
	 * GUI Trigger
	 *****************/
	public void ChoiceFirstWeapon() {
		SendCharacterSelectedMsg();
		SceneManager.UnloadSceneAsync("SelectWeapon");
		
	}

	public void ChoiceSecondWeapon() {
		SendCharacterSelectedMsg();
		SceneManager.UnloadSceneAsync("SelectWeapon");
	}

	/*****************
	 * Custom Method
	******************/
    private void SendCharacterSelectedMsg()
    {
        byte evCode = Events.SOMEONE_SELECTED_CHARACTER_EVT;    // Someone Selected Character & Weapon
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evCode, null, reliable, null);
		Debug.Log("SendCharacterSelectedMsg called");
    }
}
