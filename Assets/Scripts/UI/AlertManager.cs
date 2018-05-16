using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlertManager : MonoBehaviour {
    public GameObject AlertObj;
    public Text nameText;
	public Text AlertText;
    private string sentence;

	// Use this for initialization
	void Start () {
		sentence = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void StartAlert(Alert alert) {
		Debug.Log("StartAlert Called");
		Debug.Log("Alert Text is " + alert.sentence + alert.name);
		nameText.text = alert.name;
		AlertText.text = "";
		AlertObj.SetActive(true);
        StartCoroutine(TypeSentence(alert.sentence));
	}
	IEnumerator TypeSentence(string sentence) {
		foreach (char letter in sentence.ToCharArray()) {
            AlertText.text += letter;
            yield return null;
        }
    }

	public void EndAlert() {
        AlertObj.SetActive(false);
        Debug.Log("End Alert");
	}


}
