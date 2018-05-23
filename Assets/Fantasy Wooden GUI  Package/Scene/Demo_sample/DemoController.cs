using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class DemoController : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Screen.SetResolution(1280, 800, true);
        
        CamTrans = Camera.main.transform;

    }

    private Transform CamTrans;
    public int MoveSpeed = 16;
    private int CurrPage = 0;
    private int FullPage = 4;
    public Text PageText;
	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Debug.Log("Right Move");

            CurrPage++;
            if (CurrPage > FullPage)
            {
                CurrPage = FullPage;
                return;

            }

            PageText.text = "Page:" + (CurrPage + 1);
        Vector3 Tmppos = new Vector3( CamTrans.transform.position.x+ MoveSpeed, CamTrans.transform.position.y,CamTrans.transform.position.z);
            CamTrans.transform.position = Tmppos;
        
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Debug.Log("Left Move");

            CurrPage--;
            if (CurrPage < 0)
            {
                CurrPage = 0;
                return;

            }
            PageText.text = "Page:" + (CurrPage+1);

            Vector3 Tmppos = new Vector3(CamTrans.transform.position.x - MoveSpeed, CamTrans.transform.position.y, CamTrans.transform.position.z);
            CamTrans.transform.position = Tmppos;

        }



    }
}
