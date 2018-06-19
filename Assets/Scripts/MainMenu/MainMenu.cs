using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public AudioSource BtnClickSound;

    public void PlayGame() {
        string prefsEmail = PlayerPrefs.GetString("email"); // PlayerPref에 저장하고 씬 이동
        if (!string.IsNullOrEmpty(prefsEmail)) // already login
        {
            SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
        } else { // need to login
            SceneManager.LoadScene("Login", LoadSceneMode.Single);
        }
        
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void OnButtonClickSound() {
        BtnClickSound.Play();
    }
}
