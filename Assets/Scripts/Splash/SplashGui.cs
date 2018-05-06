using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class SplashGui : MonoBehaviour
{

	private bool IsEnterAnotherScene = false;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
		// If Space || Enter || ESC Key Entered, then Load "Menu" Scene
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Escape) || (Input.GetKeyDown(KeyCode.Return)) && !IsEnterAnotherScene)
        {
            SceneManager.LoadScene("Menu", LoadSceneMode.Single);
			IsEnterAnotherScene = true;
        }
    }
}
