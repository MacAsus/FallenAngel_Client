using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundSound : MonoBehaviour {

    public static BackGroundSound instance = null;

    public AudioClip Sound_Lobby_BGM; // 로비 배경음
    public AudioClip Sound_Ingame_BGM; // 인게임 기본 배경음
    public AudioClip Sound_Boss_BGM; // 보스 스테이지 배경음

    private AudioSource myAudio;

    // Use this for initialization
    void Start () {
        myAudio = GetComponent<AudioSource>();
    }

    public void Play_Sound_Lobby_BGM()
    {
        myAudio.PlayOneShot(Sound_Lobby_BGM);
    }
    public void Play_Sound_Ingame_BGM()
    {
        myAudio.PlayOneShot(Sound_Ingame_BGM);
    }
    public void Play_Sound_Boss_BGM()
    {
        myAudio.PlayOneShot(Sound_Boss_BGM);
    }
}
