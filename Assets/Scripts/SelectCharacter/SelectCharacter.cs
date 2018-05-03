using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChoiceAttacker() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SendCharacterSelectedMsg(Util.s_Attacker);
		SceneManager.UnloadSceneAsync("SelectCharacter");
    }

	public void ChoiceTanker() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SendCharacterSelectedMsg(Util.s_Tanker);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

	public void ChoiceHealer() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SendCharacterSelectedMsg(Util.s_Healer);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

	public void ChoiceHeavy() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SendCharacterSelectedMsg(Util.s_Heavy);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

	private void SendCharacterSelectedMsg(string job)
    {
        byte evtCode = Events.SOMEONE_SELECTED_CHARACTER_EVT;    // Someone Selected Character & Weapon
		RaiseEventOptions options = new RaiseEventOptions();
		options.Receivers = ReceiverGroup.All;
        bool reliable = true;
		
        PhotonNetwork.RaiseEvent(evtCode, job, reliable, options);
		Debug.Log("SendCharacterSelectedMsg called");
    }

}
