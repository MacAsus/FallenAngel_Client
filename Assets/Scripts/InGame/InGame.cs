using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame : MonoBehaviour {
	public GameObject PlayerPref;

	// Use this for initialization
	void Start () {
		PhotonNetwork.Instantiate(this.PlayerPref.name, new Vector3(0f, 0.426f, 0f), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
