using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGame : MonoBehaviour {

	public GameObject playerPrefab;

	// Use this for initialization
	void Start () {
		PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
