using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Spine;
using Spine.Unity;


public class CutScenePlayer : MonoBehaviour {

    [Serializable]
    public class CutSceneObj
    {
        public GameObject cutObj;
        public SpineAnimation spine_anim;
        public Animator spr_anim;

        public CharacterGeneral ObjCharGeneral;
    }



    public CutSceneObj[] ObjArr;

    public GameObject[] SpineAnimationObj;
    public GameObject[] SprAnimationObj;



    public void getManualInteractiveObj()
    {
        GameObject temp;
        Transform spriteTrans;
        for(int i = 0; i<ObjArr.Length; i++)
        {
            if(ObjArr[i].cutObj == null)
            {
                continue;
            }else
            {
                temp = ObjArr[i].cutObj;
                if(temp.GetComponent<CharacterGeneral>() != null)
                {
                    if(temp.transform.Find("Sprite") == null)
                    {
                        continue;
                    }else
                    {
                        spriteTrans = temp.transform.Find("Sprite");
                        if (spriteTrans.GetComponent<Animator>() != null)
                        {

                        } else if(spriteTrans.GetComponent<SpineAnimation>() != null)
                        {

                        }
                    }
                    
                }else
                {
                    continue;
                }
            }
        }
    }
    public void getAutoInteractiveObj()
    {

    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
