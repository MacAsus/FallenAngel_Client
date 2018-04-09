using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SelectWeapon : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ChoiceFirstWeapon() {
		SceneManager.UnloadSceneAsync("SelectWeapon");
	}

	public void ChoiceSecondWeapon() {
		SceneManager.UnloadSceneAsync("SelectWeapon");
	}
}
