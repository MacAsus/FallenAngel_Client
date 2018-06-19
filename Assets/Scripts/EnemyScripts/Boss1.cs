using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Spine;
using Spine.Unity;

public class AnimEvent : UnityEvent<int, string, float>
{

}

public class Boss1 : EnemyGeneral {

    public AnimEvent animEvent;

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

    string[] s_CurAnim = { " ", " ", " " };
    bool b_IsSpin = false;
    
    // Use this for initialization
    void Start () {
        s_tag = Util.S_PLAYER;
        n_hp = Util.F_ROBOT_HP;
        f_Speed = Util.F_ROBOT_SPEED;
        f_Damage = Util.F_ROBOT_DAMAGE;

        shootDelayTime = 0.333f; //총알 발사 딜레이

        Target = GameObject.FindWithTag(s_tag);

        v_muzzle[0] = this.gameObject.transform.position;
        

        Bullet = Resources.Load("BulletPrefab/" + Util.S_SMG_BULLET_NAME) as GameObject;
        EnemyWeapon = new GeneralInitialize.GunParameter(Util.S_SMG_NAME, Util.S_SMG_BULLET_NAME, 3.0f, Util.F_SMG_BULLET_DAMAGE, Util.F_SMG_MAGAZINE);

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

        if(animEvent == null)
        {
            animEvent = new AnimEvent();
        }
        //animEvent.AddListener(setAnimation);
	}

    // Update is called once per frame
    void Update ()
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

        if (n_hp > 0)
        {
            if (Target != null)
            {
                v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
            }

            this.photonView.RPC("PatternAnim", PhotonTargets.All);
            //PatternAnim();
            Search(Util.F_ROBOT_SEARCH);
        }

        if (n_hp <= 0)
        {
            this.photonView.RPC("Dead", PhotonTargets.All);
        }
        ////애니메이션 디버깅용
        //if (Input.GetKey(KeyCode.Q))
        //{

        //    setAnimation(0, "Shot", true, 1);

        //}
        //if (Input.GetKeyDown(KeyCode.W))
        //{
        //    if (b_IsSpin)
        //    {
        //        b_IsSpin = false;
        //    }
        //    else
        //    {
        //        b_IsSpin = true;
        //        setAnimation(2, "Spin", true, 1.25f);
        //    }
        //}
        //if (Input.GetKey(KeyCode.E))
        //{
        //    setAnimation(0, "Idle", true, 1);
        //}
        //if (Input.GetKey(KeyCode.R))
        //{
        //    setAnimation(0, "Dead", true, 1);
        //}
        ////여기까지
    }

    protected void BossOnevent(TrackEntry trackIndex, Spine.Event e)
    {
        if (e.data.name == "Shoot")
        {
            Pattern1();
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
    protected void SpinSpeed(float Time)
    {
        setAnimation(2, "Spin", true, Time);
    }
    protected void ShootSpeed(float Time)
    {
        setAnimation(0, "Shot", true, Time);
    }

    [PunRPC]
    protected void Dead()
    {
        setAnimation(0, "Dead", true, 1);

    }
    [PunRPC]
    protected void PatternAnim()
    {
        if (n_hp > 0)
        {
            //1단계 패턴
            if (n_hp <= Util.F_ROBOT_HP && n_hp >= Util.F_ROBOT_HP / 2)
            {
                ShootSpeed(1f);
                SpinSpeed(0.1f);
            }
            //2단계 패턴
            else
            {
                ShootSpeed(1f);
                SpinSpeed(0.1f);
            }
        }
    } 

    protected override void WeaponSpineControl(bool _b_EnemyFired, bool _b_EnemyReload)
    {
        
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
    protected void Pattern1()
    {
        Vector3[] v_bulletDir = new Vector3[13];

        v_bulletDir[0] = v_muzzle[0];

        for (int i = 1; i <= 12; i++)
        {
            v_bulletDir[i] = (v_muzzle[i] - (v_muzzle[0] + Util.V_ACCRUATE)).normalized;
        }

        this.photonView.RPC("Pattern1Network", PhotonTargets.All, v_bulletDir);
        //this.photonView.RPC("PatternAnim", PhotonTargets.Others);
    }
    [PunRPC]
    protected void Pattern1Network(Vector3[] bulletDir)
    {
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

    protected void Pattern2()
    {

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
            }
            else
            {
                b_IsSearch = false;
            }
        }
    }
}
