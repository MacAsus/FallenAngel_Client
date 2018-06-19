using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.UI;

public class CutSceneControl : MonoBehaviour {

    public PlayableDirector PlayableDir;

    UnityEvent MyEvent;

    public bool b_Stop = false;
    bool b_EffectEnd = false;
    public Text t_Dialogue;



    public string[] s_Dialogue;
    public int nextDialog;
    int DialogueNum;

    // Use this for initialization
    void Start () {
        DialogueNum = 0;
        nextDialog = -1;
	}
	
	// Update is called once per frame
	void Update () {


        if (DialogueNum >= s_Dialogue.Length)
        {
            return;
        }

        if (nextDialog > DialogueNum)
        {
            PlayableDir.Pause();
            Coroutine tempCor = null;

            if (!b_EffectEnd)
            {
                tempCor = StartCoroutine(TextBoxEffect());
                b_EffectEnd = true;
            }
            if (Input.anyKeyDown)
            {
                b_EffectEnd = false;
                if (tempCor != null)
                {
                    StopCoroutine(tempCor);
                }
                DialogueNum += 1;
                t_Dialogue.text = "";
                PlayableDir.Resume();
            }


        }


        //if (b_Stop)
        //{
        //    if (b_EffectEnd)
        //    {
        //        if (Input.anyKeyDown)
        //        {
        //            b_Stop = false;
        //            b_EffectEnd = false;
        //            DialogueNum += 1;
        //            t_Dialogue.text = "";
        //            PlayableDir.Resume();
        //        }
        //    }
        //    else
        //    {
        //        b_EffectEnd = true;
        //        PlayableDir.Pause();
        //        StartCoroutine(TextBoxEffect());

        //    }
        //}
    }

    void PauseDirector()
    {
        PlayableDir.Pause();
        b_Stop = true;
    }

    void ResumeDirector()
    {
        PlayableDir.Resume();
        b_Stop = false;
    }

    void PlayDirector()
    {
        PlayableDir.Play();
        b_Stop = false;
    }
    void Ping()
    {
        Debug.Log("Ping");
    }

    IEnumerator TextBoxEffect()
    {
        for(int i=0; i<s_Dialogue[DialogueNum].Length; i++)
        {
            t_Dialogue.text += s_Dialogue[DialogueNum][i];
            yield return null;
        }

    }
    IEnumerator whileLoop()
    {
        while (true)
        {

            if (DialogueNum >= s_Dialogue.Length)
            {
                break;
            }

            if (nextDialog > DialogueNum)
            {
                Coroutine tempCor = null;
                PlayableDir.Pause();
                if (!b_EffectEnd)
                {
                    tempCor = StartCoroutine(TextBoxEffect());
                    b_EffectEnd = true;
                }
                if (Input.anyKeyDown)
                {
                    b_EffectEnd = false;
                    if (tempCor != null)
                    {
                        StopCoroutine(tempCor);
                    }
                    DialogueNum += 1;
                    t_Dialogue.text = "";
                    PlayableDir.Resume();
                }


            }
            yield return null;
        }
    }

}
