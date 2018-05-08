using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {
    public AudioSource BtnClickSound;

    public void PlayGame() {
        SceneManager.LoadScene("RoomList", LoadSceneMode.Single);
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void OnButtonClickSound() {
        BtnClickSound.Play();
    }
}
