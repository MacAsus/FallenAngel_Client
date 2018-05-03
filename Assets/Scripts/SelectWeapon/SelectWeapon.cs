using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectWeapon : MonoBehaviour {

	// Use this for initialization
	private string player_id;
	void Start () {
		// 네트워크에서 선택한 캐릭터 받아옴
		Debug.Log("PhotonNetwork.player.ID" + PhotonNetwork.player.ID);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	/*****************
	 * GUI Trigger
	 *****************/
	public void ChoiceFirstWeapon() {
		SendWeaponSelectedMsg(1);
		SceneManager.UnloadSceneAsync("SelectWeapon");
	}

	public void ChoiceSecondWeapon() {
		SendWeaponSelectedMsg(2);
		SceneManager.UnloadSceneAsync("SelectWeapon");
	}

	/*****************
	 * Custom Method
	******************/
    private void SendWeaponSelectedMsg(int weaponNum)
    {

        byte evtCode = Events.SOMEONE_SELECTED_WEAPON_EVT;    // Someone Selected Weapon
		RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
        bool reliable = true;
        PhotonNetwork.RaiseEvent(evtCode, weaponNum, reliable, options);
		Debug.Log("SendCharacterSelectedMsg called");
    }
}
