using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour {

	public static void TriggerDialogue(Dialogue dialogue) {
		FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
	}
}
