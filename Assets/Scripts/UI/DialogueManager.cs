using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueManager : MonoBehaviour {

	private Queue<string> sentences;

	// Use this for initialization
	void Start () {
		sentences = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartDialogue(Dialogue dialogue) {
		Debug.Log("Start Conversation with " + dialogue.name);

		sentences.Clear();

		foreach(string sentence in dialogue.sentences) {
			
		}
	}
}
