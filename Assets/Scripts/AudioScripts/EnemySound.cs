using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySound : MonoBehaviour {

    public static EnemySound instance = null;

    public AudioClip Sound_Explosion; // 폭발음(봄버)

    public AudioClip Sound_Main_Shoot; // 주 무기 발사음

    public AudioClip Sound_Gun_Hit; // 원거리 무기 타격음
    public AudioClip Sound_Melee_Hit; // 근거리 무기 타격음(몸체 충돌)

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

    public void Play_Sound_Main_Shoot()
    {
        myAudio.PlayOneShot(Sound_Main_Shoot);
    }

    public void Play_Sound_Gun_Hit()
    {
        myAudio.PlayOneShot(Sound_Gun_Hit);
    }
    public void Play_Sound_Melee_Hit()
    {
        myAudio.PlayOneShot(Sound_Melee_Hit);
    }
}
