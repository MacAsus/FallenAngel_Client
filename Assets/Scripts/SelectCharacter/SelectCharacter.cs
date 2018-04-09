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
		SceneManager.UnloadSceneAsync("SelectCharacter");
    }

	public void ChoiceTanker() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

	public void ChoiceHealer() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

	public void ChoiceHeavy() {
		SceneManager.LoadScene("SelectWeapon", LoadSceneMode.Additive);
		SceneManager.UnloadSceneAsync("SelectCharacter");
	}

}
