using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class objTempScripts : MonoBehaviour {

    public bool moveEnalbe = false;
    public int objIndex = -1;
    public Vector3 changePos;
    public float speed = 3;

    public Text posUI;
    private Text textObj;
    // Use this for initialization
    void Start () {
        textObj = Instantiate(posUI);
        textObj.transform.SetParent(FindObjectOfType<Canvas>().transform);
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 camPos = Camera.main.WorldToScreenPoint(transform.position);

        textObj.rectTransform.position = new Vector3(camPos.x, camPos.y + transform.GetComponent<BoxCollider>().bounds.size.y*100, 0);
        textObj.text = transform.position.x + " : " + transform.position.y;
        if (moveEnalbe && GeneralScript.Instance.selectedObj == objIndex)
        {

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Translate(new Vector3(-speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.UpArrow))
            {
                transform.Translate(new Vector3(0, speed * Time.deltaTime, 0));
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Translate(new Vector3(speed * Time.deltaTime, 0, 0));
            }
            if (Input.GetKey(KeyCode.DownArrow))
            {
                transform.Translate(new Vector3(0, -speed * Time.deltaTime, 0));
            }
        }
    }
    public void ChangePostion()
    {
        if (moveEnalbe)
        {
            transform.position = changePos;
        }
    }

}
