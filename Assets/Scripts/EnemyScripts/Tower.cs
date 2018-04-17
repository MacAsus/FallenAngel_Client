using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Tower : CharacterGeneral
{

    public float f_Distance; //Player와 Tower와의 거리

    public bool b_IsSearch = false; //Player 탐색 여부

    public GameObject TowerPrefab; //타워 프리팹
    public GameObject Target; //공격할 타겟
    public GameObject Tower_Muzzle; //타워 공격 시작점
    public GameObject EnemyUiPrefab; //적 UI 프리팹

    public Sprite TowerBulletImage; //타워 총알 이미지

    public GeneralInitialize.GunParameter cur_EnemyWeapon; //적 무기 정보

    Vector3 v_TargetPosition = new Vector3(); //타겟 위치 Vector3 값
    Vector3 v_Accurate = new Vector3(0, 0.5f, 0);

    //Photon Value
    Vector3 v_NetworkTargetPos;
    SpriteState e_NetworkSpriteState;
    bool b_NetworkFired;
    double f_LastNetworkDataReceivedTime;

    // Use this for initialization
    void Start()
    {
        n_hp = 100f;
        f_Speed = 0f;
        Target = GameObject.FindWithTag("Player");
        //임시 테스트용
        GeneralInitialize.GunParameter tempEnemyWeapon =
            new GeneralInitialize.GunParameter("Hg_Brownie", 10, 5, "Hg_Norm", 5);
        TowerBulletImage = tempEnemyWeapon.BulletImage;
        cur_EnemyWeapon = tempEnemyWeapon;
        //여기까지
        InitializeParam();
        FindMuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                if(Target) {
                    v_TargetPosition = Target.transform.position + v_Accurate;
                }
                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                UpdateAnimationControl(e_SpriteState, b_Fired);
            }
        }
        else
        {
            UpdateNetworkAnimationControl();
        }

        Search(); // Find Nearest Other Players
        RotateGun(Target.transform.position + v_Accurate);
    }

    void UpdateNetworkAnimationControl()
    {
        if (e_NetworkSpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (e_NetworkSpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }

    //스파인 이벤트 처리
    public override void SpineOnevent(TrackEntry trackIndex, Spine.Event e)
    {
        if (e.Data.name == "Shoot_Start")
        {

            b_Fired = true;


        }
        else if (e.data.name == "Shoot_End")
        {
            b_Fired = false;
        }
    }

    //애니메이션 컨트롤(weaponSpineControl, fireBullet 모두 처리하는 중)
    protected override void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired)
    {
        WeaponSpineControl(_b_Fired);
        if (_e_SpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_e_SpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }

    protected override void FireBullet()
    {
        // Debug.Log("[Tower] FireBullet called");
        if (Tower_Muzzle) {
            Vector3 v_muzzle = Tower_Muzzle.transform.position;
            Vector3 v_bulletSpeed = (Tower_Muzzle.transform.position - (Target.transform.position + v_Accurate)).normalized * cur_EnemyWeapon.f_BulletSpeed;

            this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
            this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
        } else {
            Debug.Log("Muzzle Not Found");
        }
    }

    [PunRPC]
    void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        // Debug.Log("[Tower] FireBulletNetwork called");
        GameObject bullet = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
        temp_bullet.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet.s_Victim = "Player";
        bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - this.g_Weapon.transform.position).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
    }

    [PunRPC]
    void FireAnimationNetwork()
    {
        if (b_NetworkFired)
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", true);
        }
        else
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }
    }

    protected override void WeaponSpineControl(bool _b_EnemyFired)
    {
        if (b_IsSearch == true)
        {
            if (!_b_EnemyFired && Target.GetComponent<CharacterGeneral>().n_hp > 0)
            {
                FireBullet();
                spine_GunAnim.state.SetAnimation(0, "Shoot", false);
            }
        }
    }

    //총구 위치 찾기(추후 수정)
    void FindMuzzle()
    {
        if (cur_EnemyWeapon.s_GunName.Contains("Hg"))
        {
            // 인스펙터에서 적용
        }
        else if (cur_EnemyWeapon.s_GunName.Contains("Ar"))
        {
            // 인스펙터에서 적용
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Debug.Log("SerializeState Called");
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

    protected override void Search()
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0) {
            // Debug.Log("NetworkUtil.PlayerList count " + NetworkUtil.PlayerList.Count);
            foreach(GameObject player in NetworkUtil.PlayerList) {
                Vector3 playerPos = player.transform.position;
                
                float playerToTowerDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToTowerDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if(playerToTowerDist < minDistToTowerDist) {
                    distance = playerPos;
                    f_Distance = playerToTowerDist;
                    Target = player;
                }
            }

            if(Target != null) {
                // Debug.Log("Player pos: " + Target.transform.position.x + " : " + Target.transform.position.y);
            }

            // Debug.Log("f_Distance is: " + f_Distance);

            if(f_Distance <= 5) { // 거리가 5보다 가까운 플레이어가 있으면
                b_IsSearch = true;
            } else { // 거리가 5보다 가까운 플레이어가 없으면
                b_IsSearch = false;
            }
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            { // 자기가 맞았을 경우에만 다른 클라이언트에게 "나 맞았다" RPC 호출
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, 10.0f); //Tower에 부딪힐 경우 10의 Damage를 입습니다.
            }
        }
        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Player is dead.");
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
}