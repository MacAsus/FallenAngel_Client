using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSound : MonoBehaviour {

    private AudioSource myAudio;
    public static BulletSound instance = null;

    public AudioClip Sound_Explosion; // 폭발음
    public AudioClip Sound_Gun_Hit; // 원거리 무기 타격음

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start()
    {
        myAudio = GetComponent<AudioSource>();
    }

    public void Play_Sound_Explosion()
    {
        myAudio.PlayOneShot(Sound_Explosion);
    }
    public void Play_Sound_Gun_Hit()
    {
        myAudio.PlayOneShot(Sound_Gun_Hit);
    }

}
