using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignUp : MonoBehaviour {

	public InputField ID_INPUT;
	public InputField PW_INPUT;


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SignUpPost() {
		Debug.Log("ID: " + ID_INPUT.text);
		Debug.Log("PW: " + PW_INPUT.text);
		WWW www;
		Hashtable postHeader = new Hashtable();
		postHeader.Add("Content-Type", "application/json");	

		// convert json string to byte
		string ourPostData = "{" 
		+ "\"email\":" + "\"" + ID_INPUT.text + "\", " 
		+ "\"password\":" + "\"" + PW_INPUT.text + "\", " 
		+ "\"confirmPassword\":" + "\"" + PW_INPUT.text + "\"" 
		+ "}";

		Debug.Log("ourPostData is"+ ourPostData);
 		byte[] pData = System.Text.Encoding.ASCII.GetBytes(ourPostData.ToCharArray());

		www = new WWW(Networking.ROOT_DOMAIN + Networking.SIGNUP_POST, pData, postHeader);
		StartCoroutine(WaitForRequest(www));
	}

	IEnumerator WaitForRequest(WWW data) {
		yield return data; // Wait until the download is done
		if (data.error != null)
		{
			Debug.Log("There was an error sending request: " + data.error);
		}
		else
		{
			Debug.Log("WWW Request: " + data.text);
			var res = JsonUtility.FromJson<WWWres> (data.text);
			if(res.success) { // Login 성공
				PlayerPrefs.SetString("email", ID_INPUT.text); // PlayerPref에 저장하고 씬 이동
			} else { // Login 실패
				PlayerPrefs.SetString("email", ""); // PlayerPref에 있는 값 비움
			} 
		}
	}

	[System.Serializable]
	public class WWWres
	{
		public bool success;
		public string msg;
	}
}
