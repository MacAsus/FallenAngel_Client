using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Bomber : CharacterGeneral
{

    public float f_Distance; //Player와 Bomber와의 거리

    public bool b_IsSearch = false; //Player 탐색 여부

    public GameObject BomberPrefab; //Bomber 프리팹
    public GameObject Target; //공격할 타겟
    public GameObject Bomber_Muzzle; //Bomber 공격 시작점
    public GameObject EnemyUiPrefab; //적 UI 프리팹

    public Sprite BomberBulletImage; //Bomber 총알 이미지

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
        n_hp = 50f;
        f_Speed = 2f;
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        Target = GameObject.FindWithTag("Player");
        Bomber_Muzzle = this.gameObject;
        //임시 테스트용
        GeneralInitialize.GunParameter tempEnemyWeapon =
            new GeneralInitialize.GunParameter("Hg_Brownie", 10, 5, "Hg_Norm", 5);
        BomberBulletImage = tempEnemyWeapon.BulletImage;
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

                if (Target)
                {
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
        Trace();
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
        Debug.Log("[Tower] FireBullet called");
        if (Bomber_Muzzle)
        {
            Vector3 v_muzzle = Bomber_Muzzle.transform.position + v_Accurate; //Bomber 자신의 중앙값 위치
            Vector3 v_bulletSpeed = (Bomber_Muzzle.transform.position - (Target.transform.position + v_Accurate)).normalized * cur_EnemyWeapon.f_BulletSpeed;

            this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
            this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
        }
        else
        {
            Debug.Log("Muzzle Not Found");
        }
    }

    [PunRPC]
    void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        Vector3 front = new Vector3(1, 0, 0);
        Debug.Log("[Tower] FireBulletNetwork called");
        GameObject bullet1 = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        GameObject bullet2 = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        GameObject bullet3 = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        GameObject bullet4 = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        BulletGeneral temp_bullet1 = bullet1.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet2 = bullet2.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet3 = bullet3.GetComponent<BulletGeneral>();
        BulletGeneral temp_bullet4 = bullet4.GetComponent<BulletGeneral>();
        temp_bullet1.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet2.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet3.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet4.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet1.s_Victim = "Player";
        temp_bullet2.s_Victim = "Player";
        temp_bullet3.s_Victim = "Player";
        temp_bullet4.s_Victim = "Player";
        bullet1.GetComponent<Rigidbody2D>().velocity = (muzzlePos - (muzzlePos + front)).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
        bullet2.GetComponent<Rigidbody2D>().velocity = ((muzzlePos + front) - muzzlePos).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
        bullet3.GetComponent<Rigidbody2D>().velocity = (muzzlePos - Bomber_Muzzle.transform.position).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
        bullet4.GetComponent<Rigidbody2D>().velocity = (Bomber_Muzzle.transform.position - muzzlePos).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
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
        if (NetworkUtil.PlayerList.Count != 0)
        {
            Debug.Log("NetworkUtil.PlayerList count " + NetworkUtil.PlayerList.Count);
            foreach (GameObject player in NetworkUtil.PlayerList)
            {
                Vector3 playerPos = player.transform.position;

                float playerToBomberDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToBomberDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if (playerToBomberDist < minDistToBomberDist)
                {
                    distance = playerPos;
                    f_Distance = playerToBomberDist;
                    Target = player;
                }
            }

            if (Target != null)
            {
                Debug.Log("Player pos: " + Target.transform.position.x + " : " + Target.transform.position.y);
            }

            Debug.Log("f_Distance is: " + f_Distance);

            if (f_Distance <= 5)
            { // 거리가 5보다 가까운 플레이어가 있으면
                    b_IsSearch = true;
            }
            else
            { // 거리가 5보다 가까운 플레이어가 없으면
                b_IsSearch = false;
            }
        }
    }

    void Trace()
    {
        //살아있는 Player 탐색 성공 시 Run 애니메이션 실행
        if (b_IsSearch == true && Target.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            rigid.velocity = ((Target.transform.position + v_Accurate) - this.transform.position).normalized * (f_Speed);
            a_Animator.SetBool("Run", true);
        }
        //탐색이 되지 않았을 시 Run 애니메이션을 끔
        else
        {
            rigid.velocity = Vector3.zero;
            a_Animator.SetBool("Run", false);
        }
    }

    [PunRPC]
    void TakeDamage(float _f_Damage)
    {
        Target.GetComponent<CharacterGeneral>().n_hp -= _f_Damage;
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
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, 40.0f); //Bomber에 부딪힐 경우 40의 Damage를 입습니다.
            }
            Destroy(this.gameObject); //Bomber는 소멸합니다.
        }
        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Player is dead.");
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
}