﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Events;
using Spine;
using Spine.Unity;



public class Boss1 : EnemyGeneral {


    public PlayableDirector FadeOut;
    //총구
    public GameObject muzzle1;
    public GameObject muzzle2;
    public GameObject muzzle3;

    public GameObject muzzle4;
    public GameObject muzzle5;
    public GameObject muzzle6;

    public GameObject muzzle7;
    public GameObject muzzle8;
    public GameObject muzzle9;

    public GameObject muzzle10;
    public GameObject muzzle11;
    public GameObject muzzle12;

    public Vector3[] v_muzzle = new Vector3[13];
    public GameObject SpawnMonster;

    public ParticleSystem[] deadPart;


    bool b_Dead = false;
    bool b_ParticleStart = false;
    bool b_ShootCool = false;
    bool b_RushCool = false;
    bool b_SpawnCool = false;

    string[] s_CurAnim = { " ", " ", " " };
    bool b_IsSpin = false;
    
    // Use this for initialization
    void Start () {


        s_tag = Util.S_PLAYER;
        n_hp = Util.F_ROBOT_HP;
        f_Speed = Util.F_ROBOT_SPEED;
        f_Damage = Util.F_ROBOT_DAMAGE;

        //shootDelayTime = 0.333f; //총알 발사 딜레이

        Target = GameObject.FindWithTag(s_tag);

        v_muzzle[0] = this.gameObject.transform.position;
        

        Bullet = Resources.Load("BulletPrefab/" + Util.S_SMG_BULLET_NAME) as GameObject;
        EnemyWeapon = new GeneralInitialize.GunParameter(Util.S_SMG_NAME, Util.S_SMG_BULLET_NAME, 6.0f, Util.F_SMG_BULLET_DAMAGE, Util.F_SMG_MAGAZINE);
        
        InitializeParam();

        if(spine_CharacterAnim == null)
        {
            return;
        }

        spine_CharacterAnim.state.Event += BossOnevent;
        spine_CharacterAnim.state.Start += delegate (Spine.TrackEntry entry)
        {
            if (spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Shot")
            {
                int temp = Random.Range(1, 3);
                string animName = "Robot_Attack" + temp;
                spine_CharacterAnim.state.SetAnimation(1, animName, true);
            }
            if(spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Dead")
            {
                b_IsSpin = false;
                spine_CharacterAnim.state.ClearTrack(2);
                spine_CharacterAnim.state.SetAnimation(1, "Robot_Dead", true);
            }
            if(spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Idle")
            {
                if (spine_CharacterAnim.state.GetCurrent(2) != null)
                {
                    b_IsSpin = false;
                    spine_CharacterAnim.state.GetCurrent(2).loop = false;
                }
                spine_CharacterAnim.state.SetAnimation(1, "Robot_Idle1", true);
            }
        };
        spine_CharacterAnim.state.Complete += delegate (Spine.TrackEntry entry)
        {
            if (spine_CharacterAnim.state.GetCurrent(entry.TrackIndex).Animation.name == "Spin")
            {
                if (b_IsSpin)
                {
                    return;
                }
                else
                {
                    spine_CharacterAnim.state.ClearTrack(entry.TrackIndex);
                }
            }
        };
        for(int i=0; i<deadPart.Length; i++)
        {
            deadPart[i].Stop();
        }
        //this.photonView.RPC("StartSpawn", PhotonTargets.All);
    }

    // Update is called once per frame
    void Update ()
    {

        if (!b_Dead)
        {
            v_muzzle[1] = muzzle1.transform.position;
            v_muzzle[2] = muzzle2.transform.position;
            v_muzzle[3] = muzzle3.transform.position;
            v_muzzle[4] = muzzle4.transform.position;
            v_muzzle[5] = muzzle5.transform.position;
            v_muzzle[6] = muzzle6.transform.position;
            v_muzzle[7] = muzzle7.transform.position;
            v_muzzle[8] = muzzle8.transform.position;
            v_muzzle[9] = muzzle9.transform.position;
            v_muzzle[10] = muzzle10.transform.position;
            v_muzzle[11] = muzzle11.transform.position;
            v_muzzle[12] = muzzle12.transform.position;

            //생존 시
            if (n_hp > 0)
            {
                Search(Util.F_ROBOT_SEARCH);

                if (Target != null)
                {
                    v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
                }

                //체력 100% ~ 50%
                if (n_hp > (Util.F_ROBOT_HP / 2) && n_hp <= Util.F_ROBOT_HP)
                {
                    if (b_IsSearch == true)
                    {
                        this.photonView.RPC("Rush", PhotonTargets.All, 2.0f);
                        this.photonView.RPC("SpinSpeed", PhotonTargets.All, 1.0f);
                    }
                    if (spine_CharacterAnim.state.GetCurrent(2) != null)
                    {
                        spine_CharacterAnim.state.GetCurrent(2).timeScale = 1.0f;
                    }
                    if (spine_CharacterAnim.state.GetCurrent(0) != null)
                    {
                        spine_CharacterAnim.state.GetCurrent(0).timeScale = 0.3f;
                    }
                    //this.photonView.RPC("SpinSpeed", PhotonTargets.All, 0.1f);
                    this.photonView.RPC("ShootSpeed", PhotonTargets.All, 0.3f);
                    
                }

                //체력 50% 미만
                if (n_hp <= (Util.F_ROBOT_HP / 2))
                {
                    if (b_IsSearch == true)
                    {
                        this.photonView.RPC("Rush", PhotonTargets.All, 2.5f);
                        //this.photonView.RPC("SpinSpeed", PhotonTargets.All, 1.5f);
                        //this.photonView.RPC("ShootSpeed", PhotonTargets.All, 1.0f);
                    }
                    if (spine_CharacterAnim.state.GetCurrent(2) != null)
                    {
                        spine_CharacterAnim.state.GetCurrent(2).timeScale = 1.5f;
                    }
                    if(spine_CharacterAnim.state.GetCurrent(0)!= null)
                    {
                        spine_CharacterAnim.state.GetCurrent(0).timeScale = 1.0f;
                    }
                        this.photonView.RPC("SpinSpeed", PhotonTargets.All, 0.15f);

                        this.photonView.RPC("ShootSpeed", PhotonTargets.All, 1.0f);
                        //this.photonView.RPC("Pattern2Anim", PhotonTargets.All);
                    
                }
            }

            //사망 시
            if (n_hp <= 0)
            {
                this.photonView.RPC("Dead", PhotonTargets.All);


            }
        }
    }

    protected void BossOnevent(TrackEntry trackIndex, Spine.Event e)
    {
        if (e.data.name == "Shoot")
        {
            this.photonView.RPC("FireAll", PhotonTargets.All);
        }
    }

    void setAnimation(int track, string animName, bool loop, float speed)
    {
        if(spine_CharacterAnim.state.GetCurrent(track) != null)
        {
            if (spine_CharacterAnim.state.GetCurrent(track).Animation.name != animName)
            {
                s_CurAnim[track] = " ";
            }
        }else
        {
            s_CurAnim[track] = " ";
        }
        if (s_CurAnim[track] == animName)
        {
            return;
        }
        else
        {
            s_CurAnim[track] = animName;
            spine_CharacterAnim.state.SetAnimation(track, animName, loop).timeScale = speed;
        }
    }

    //void setAnimation(int track, string animName, float speed)
    //{
    //    spine_CharacterAnim.state.SetAnimation(track, animName, true).TimeScale = speed;
    //}

    void handleEvent(Spine.TrackEntry entry, Spine.Event e) 
    {
        
    }

    [PunRPC]
    protected void Rush(float speed)
    {
        rigid.velocity = (v_TargetPosition - transform.position).normalized * f_Speed * speed;
    }
    [PunRPC]
    protected void SpinSpeed(float speed)
    {
        setAnimation(2, "Spin", true, speed);
    }
    [PunRPC]
    protected void ShootSpeed(float speed)
    {
        setAnimation(0, "Shot", true, speed);
    }
    [PunRPC]
    protected void Dead()
    {
        setAnimation(0, "Dead", true, 1);
        b_Dead = true;
        StartCoroutine(DeadPartcile());
        rigid.velocity = Vector3.zero;
        FadeOut.Play();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_TargetPosition);
        }
        else
        {
            // Network player, receive data
            e_NetworkSpriteState = (SpriteState)stream.ReceiveNext();
            b_NetworkFired = (bool)stream.ReceiveNext();
            v_NetworkTargetPos = (Vector3)stream.ReceiveNext();

            f_LastNetworkDataReceivedTime = info.timestamp;
        }
    }

    //패턴1 - 스핀X, 모든 총구에서 총알 뿌리기
    //protected void Pattern1()
    //{
    //    Vector3[] v_bulletDir = new Vector3[13];

    //    v_bulletDir[0] = v_muzzle[0];

    //    for (int i = 1; i <= 12; i++)
    //    {
    //        v_bulletDir[i] = (v_muzzle[i] - (v_muzzle[0] + Util.V_ACCRUATE)).normalized;
    //    }

    //    this.photonView.RPC("Pattern1", PhotonTargets.All, v_bulletDir);
    //    //this.photonView.RPC("PatternAnim", PhotonTargets.Others);
    //}
    [PunRPC]
    protected void FireAll()
    {
        Vector3[] bulletDir = new Vector3[13];

        bulletDir[0] = v_muzzle[0];

        for (int i = 1; i <= 12; i++)
        {
            bulletDir[i] = (v_muzzle[i] - transform.position).normalized;
        }

        //총알 생성
        GameObject bullet1 = Instantiate(Bullet, v_muzzle[1], muzzle1.transform.rotation);
        GameObject bullet2 = Instantiate(Bullet, v_muzzle[2], muzzle2.transform.rotation);
        GameObject bullet3 = Instantiate(Bullet, v_muzzle[3], muzzle3.transform.rotation);

        GameObject bullet4 = Instantiate(Bullet, v_muzzle[4], muzzle4.transform.rotation);
        GameObject bullet5 = Instantiate(Bullet, v_muzzle[5], muzzle5.transform.rotation);
        GameObject bullet6 = Instantiate(Bullet, v_muzzle[6], muzzle6.transform.rotation);

        GameObject bullet7 = Instantiate(Bullet, v_muzzle[7], muzzle7.transform.rotation);
        GameObject bullet8 = Instantiate(Bullet, v_muzzle[8], muzzle8.transform.rotation);
        GameObject bullet9 = Instantiate(Bullet, v_muzzle[9], muzzle9.transform.rotation);

        GameObject bullet10 = Instantiate(Bullet, v_muzzle[10], muzzle10.transform.rotation);
        GameObject bullet11 = Instantiate(Bullet, v_muzzle[11], muzzle11.transform.rotation);
        GameObject bullet12 = Instantiate(Bullet, v_muzzle[12], muzzle12.transform.rotation);
        //여기까지

        BulletGeneral temp_bullet1 = bullet1.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet2 = bullet2.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet3 = bullet3.GetComponent<BulletGeneral>();

        BulletGeneral temp_bullet4 = bullet4.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet5 = bullet5.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet6 = bullet6.GetComponent<BulletGeneral>();

        BulletGeneral temp_bullet7 = bullet7.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet8 = bullet8.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet9 = bullet9.GetComponent<BulletGeneral>();

        BulletGeneral temp_bullet10 = bullet10.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet11 = bullet11.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet12 = bullet12.GetComponent<BulletGeneral>();


        temp_bullet1.bulletInfo = EnemyWeapon;
        temp_bullet2.bulletInfo = EnemyWeapon;
        temp_bullet3.bulletInfo = EnemyWeapon;

        temp_bullet4.bulletInfo = EnemyWeapon;
        temp_bullet5.bulletInfo = EnemyWeapon;
        temp_bullet6.bulletInfo = EnemyWeapon;

        temp_bullet7.bulletInfo = EnemyWeapon;
        temp_bullet8.bulletInfo = EnemyWeapon;
        temp_bullet9.bulletInfo = EnemyWeapon;

        temp_bullet10.bulletInfo = EnemyWeapon;
        temp_bullet11.bulletInfo = EnemyWeapon;
        temp_bullet12.bulletInfo = EnemyWeapon;


        temp_bullet1.s_Victim = s_tag;
        temp_bullet2.s_Victim = s_tag;
        temp_bullet3.s_Victim = s_tag;

        temp_bullet4.s_Victim = s_tag;
        temp_bullet5.s_Victim = s_tag;
        temp_bullet6.s_Victim = s_tag;

        temp_bullet7.s_Victim = s_tag;
        temp_bullet8.s_Victim = s_tag;
        temp_bullet9.s_Victim = s_tag;

        temp_bullet10.s_Victim = s_tag;
        temp_bullet11.s_Victim = s_tag;
        temp_bullet12.s_Victim = s_tag;


        bullet1.GetComponent<Rigidbody2D>().velocity = bulletDir[1].normalized * EnemyWeapon.f_BulletSpeed;
        bullet2.GetComponent<Rigidbody2D>().velocity = bulletDir[2].normalized * EnemyWeapon.f_BulletSpeed;
        bullet3.GetComponent<Rigidbody2D>().velocity = bulletDir[3].normalized * EnemyWeapon.f_BulletSpeed;

        bullet4.GetComponent<Rigidbody2D>().velocity = bulletDir[4].normalized * EnemyWeapon.f_BulletSpeed;
        bullet5.GetComponent<Rigidbody2D>().velocity = bulletDir[5].normalized * EnemyWeapon.f_BulletSpeed;
        bullet6.GetComponent<Rigidbody2D>().velocity = bulletDir[6].normalized * EnemyWeapon.f_BulletSpeed;

        bullet7.GetComponent<Rigidbody2D>().velocity = bulletDir[7].normalized * EnemyWeapon.f_BulletSpeed;
        bullet8.GetComponent<Rigidbody2D>().velocity = bulletDir[8].normalized * EnemyWeapon.f_BulletSpeed;
        bullet9.GetComponent<Rigidbody2D>().velocity = bulletDir[9].normalized * EnemyWeapon.f_BulletSpeed;

        bullet10.GetComponent<Rigidbody2D>().velocity = bulletDir[10].normalized * EnemyWeapon.f_BulletSpeed;
        bullet11.GetComponent<Rigidbody2D>().velocity = bulletDir[11].normalized * EnemyWeapon.f_BulletSpeed;
        bullet12.GetComponent<Rigidbody2D>().velocity = bulletDir[12].normalized * EnemyWeapon.f_BulletSpeed;
    }

    protected override void Search(float dis)
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0)
        {
            foreach (GameObject player in NetworkUtil.PlayerList)
            {
                Vector3 playerPos;
                if (player != null)
                {
                    playerPos = player.transform.position;
                }
                else
                {
                    continue;
                }

                float playerToTowerDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToTowerDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if (playerToTowerDist < minDistToTowerDist)
                {
                    distance = playerPos;
                    f_Distance = playerToTowerDist;
                    Target = player;
                }
            }
            if (f_Distance <= dis)
            {
                b_IsSearch = true;
                //Target.GetComponentInChildren<SpriteRenderer>().color = new Color32(0, 0, 255, 128);
            }
            else
            {
                b_IsSearch = false;
                //Target.GetComponentInChildren<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    IEnumerator DeadPartcile()
    {
        for (int i = 0; i < deadPart.Length; i++)
        {
            deadPart[i].Play();
            yield return new WaitForSeconds(0.8f);
        }

    }

    IEnumerator ShootStart()
    {
        yield return new WaitForSeconds(5f);
    }
    IEnumerator ShootCool()
    {
        yield return new WaitForSeconds(3f);
    }
    IEnumerator RushCoolTime()
    {
        yield return new WaitForSeconds(6f);
    }

    [PunRPC]
    void StartSpawn()
    {
        StartCoroutine(MonsterSpawnCool());
    }
    IEnumerator MonsterSpawnCool()
    {
        while (true)
        {
            Debug.Log("Spawn");

            yield return new WaitForSeconds(10f);
        }
    }

}
