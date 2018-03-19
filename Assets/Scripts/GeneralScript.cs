using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralScript : MonoBehaviour {

    public static GeneralScript Instance = null;

    [Range(0,3)]
    public int selectedObj = 0;
    private int prevNum;
    [Range(0,4)]
    public int objNum = 1;

    public GameObject tempObj;
    private GameObject[] tempArr = new GameObject[4];

    //포지션받기
    public Vector3 GetObjPos(int tempIndex)
    {
        return tempArr[tempIndex].transform.position;
    }
    //포지션set
    public void SetObjPos(int tempIndex, Vector3 tempVec)
    {
        tempArr[tempIndex].transform.position = tempVec;
    }
    //오브젝트 id 받기
    public int GetObjIndex(int tempIndex)
    {
        if (tempIndex < objNum)
        {
            return tempArr[tempIndex].GetComponent<objTempScripts>().objIndex;
        }
        else
        {
            return -1;
        }
    }



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        prevNum = objNum;
        for (int i = 0; i < 4; i++)
        {
            tempArr[i] = Instantiate(tempObj, Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 10)), Quaternion.identity);
            tempArr[i].GetComponent<objTempScripts>().objIndex = i;
        }
        for (int i = 0; i < objNum; i++)
        {
            tempArr[i].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.2f + 0.2f * i, 0.5f, 10));
            tempArr[i].GetComponent<objTempScripts>().moveEnalbe = true;
            
        }
    }

    void Update()
    {
        if (objNum != prevNum)
        {

            for (int i = 0; i < objNum; i++)
            {
                tempArr[i].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(0.2f + 0.2f * i, 0.5f, 10));
                tempArr[i].GetComponent<objTempScripts>().moveEnalbe = true;
            }
            for (int i = objNum; i < 4; i++)
            {
                tempArr[i].transform.position = Camera.main.ViewportToWorldPoint(new Vector3(-0.5f, 0, 10));
                tempArr[i].GetComponent<objTempScripts>().moveEnalbe = false;
            }
            prevNum = objNum;
        }

    }


}
