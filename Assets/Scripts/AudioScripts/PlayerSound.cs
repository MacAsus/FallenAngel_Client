using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour {

    private AudioSource myAudio;
    public static PlayerSound instance = null;

    public AudioClip Sound_Zero_Shoot; // 장탄수 0일시 발사음

    public AudioClip Sound_Main_Reload; // 주 무기 장전음
    public AudioClip Sound_Sub_Reload; // 부 무기 장전음

    public AudioClip Sound_Main_Shoot; // 주 무기 발사음
    public AudioClip Sound_Sub_Shoot; // 부 무기 발사음
    public AudioClip Sound_Melee_Shoot; //근거리 무기 발사음

    public AudioClip Sound_Melee_Hit; // 근거리 무기 타격음
    public AudioClip Sound_Gun_Hit;

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

    // Update is called once per frame
    void Update () {
		
	}

    public void Play_Sound_Main_Reload()
    {
        myAudio.PlayOneShot(Sound_Main_Reload);
    }
    public void Play_Sound_Sub_Reload()
    {
        myAudio.PlayOneShot(Sound_Sub_Reload);
    }

    public void Play_Sound_Main_Shoot()
    {
        myAudio.PlayOneShot(Sound_Main_Shoot);
    }
    public void Play_Sound_Sub_Shoot()
    {
        myAudio.PlayOneShot(Sound_Sub_Shoot);
    }
    public void Play_Sound_Melee_Shoot()
    {
        myAudio.PlayOneShot(Sound_Melee_Shoot);
    }
    public void Play_Sound_Zero_Shoot()
    {
        myAudio.PlayOneShot(Sound_Zero_Shoot);
    }
    public void Play_Sound_Melee_Hit()
    {
        myAudio.PlayOneShot(Sound_Melee_Hit);
    }
    public void Play_Sound_Gun_Hit()
    {
        myAudio.PlayOneShot(Sound_Gun_Hit);
    }
}
