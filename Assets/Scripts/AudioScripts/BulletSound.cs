using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSound : MonoBehaviour {

    public static BulletSound instance = null;

    public AudioClip Sound_Explosion; // 폭탄 타격음

    public AudioClip Sound_Gun_Hit; // 총알 타격음

    private AudioSource myAudio;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Use this for initialization
    void Start () {
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
