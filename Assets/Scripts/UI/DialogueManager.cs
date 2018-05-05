using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour {

	public Text nameText;
	public Text DialogueText;
	private Queue<string> sentences;
    public Animator animator;

    // Use this for initialization
    void Start () {
		sentences = new Queue<string>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartDialogue(Dialogue dialogue) {
        animator.SetBool("IsOpen", true);
        nameText.text = dialogue.name; 

		sentences.Clear();

		foreach(string sentence in dialogue.sentences) {
			sentences.Enqueue(sentence);
		}

		DisplayNextSentence();
	}

	IEnumerator TypeSentence(string sentence) {
        DialogueText.text = "";
		foreach (char letter in sentence.ToCharArray()) {
            DialogueText.text += letter;
            yield return null;
        }
    }

	public void DisplayNextSentence() {
		if (sentences.Count == 0) {
			EndDialogue();
			return;
		}

		string sentence = sentences.Dequeue();
        StartCoroutine(TypeSentence(sentence));
	}

	void EndDialogue() {
		animator.SetBool("IsOpen", false);
		Debug.Log("End Dialogue");
	}
}
