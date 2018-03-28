using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SplashGui : MonoBehaviour {
	public Button startBtn;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void onClickStartBtn() {
		SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
	}
}
