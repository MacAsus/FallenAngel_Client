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

    //Photon Value
    Vector3 v_NetworkPosition;
    Vector3 v_NetworkTargetPos;
    SpriteState e_NetworkSpriteState;
    bool b_NetworkFired;
    double f_LastNetworkDataReceivedTime;

    // Use this for initialization
    void Start()
    {
        base.n_hp = 100f;
        base.f_Speed = 0f;
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
                    v_TargetPosition = Target.transform.position;
                }

                // movement synced by Photon View
                UpdatePosition();

                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                UpdateAnimationControl(e_SpriteState, b_Fired);
                RotateGun(v_TargetPosition);
            }
        }
        else
        {
            UpdateNetworkedPosition();
            UpdateTargetPosition(); // Need To Lerp
            UpdateNetworkAnimationControl();
        }
    }

    void UpdateNetworkedPosition()
    {
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - f_LastNetworkDataReceivedTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;
        int lerpValue = 20; // lerpValue가 높아질 수록 빠르게 따라잡음

        Vector3 newPosition = Vector3.Lerp(transform.position, v_NetworkPosition, Time.smoothDeltaTime * lerpValue); // 

        if (Vector3.Distance(transform.position, v_NetworkPosition) > 3f)
        {
            newPosition = v_NetworkPosition;
            Debug.Log("Teleport");
        }

        // Debug.Log("newPosition is" + newPosition.x + " : " + newPosition.y);

        transform.position = newPosition;
    }

    void UpdateNetworkAnimationControl()
    {
        // UpdateAnimationControl(e_NetworkSpriteState, b_NetworkFired);
        if (e_NetworkSpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (e_NetworkSpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }

    void UpdateTargetPosition()
    {
        RotateGun(v_NetworkTargetPos);
    }

    //적 이동 
    protected override void UpdatePosition()
    {
        //Tower는 이동하지 않고 Search 기능만 수행합니다.
        Search();
        /*
        int tempx = 0;
        int tempy = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            e_EnemySpriteState = EnemySpriteState.Run;
            if (Input.GetKey(KeyCode.A))
            {
                tempx -= 1;
            }
            if (Input.GetKey(KeyCode.D))
            {
                tempx += 1;
            }
            if (Input.GetKey(KeyCode.W))
            {
                tempy += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                tempy -= 1;
            }
        }

        v_NetworkPosition = new Vector3(EnemyRigidBody.position.x + (f_EnemyMovingSpeed * tempx * Time.deltaTime), EnemyRigidBody.position.y + (f_EnemyMovingSpeed * tempy * Time.deltaTime));
        EnemyRigidBody.velocity = new Vector2(f_EnemyMovingSpeed * tempx, f_EnemyMovingSpeed * tempy);
        */
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
        Vector3 v_muzzle = Tower_Muzzle.transform.position;
        Vector3 v_bulletSpeed = (Tower_Muzzle.transform.position - Target.transform.position).normalized * cur_EnemyWeapon.f_BulletSpeed;

        this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
        this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        Debug.Log("[Tower] FireBulletNetwork called");
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
            if (!_b_EnemyFired && Target.GetComponent<CharacterGeneral>().n_hp != 0)
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
            Tower_Muzzle = GameObject.Find("Hg_Muzzle");
        }
        else if (cur_EnemyWeapon.s_GunName.Contains("Ar"))
        {
            Tower_Muzzle = GameObject.Find("Ar_Muzzle");
        }
    }

    protected override void Search()
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0) {
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
                Debug.Log("Player pos: " + Target.transform.position.x + " : " + Target.transform.position.y);
            }

            Debug.Log("f_Distance is: " + f_Distance);

            if(f_Distance <= 5) { // 거리가 5보다 가까운 플레이어가 있으면
                b_IsSearch = true;
            } else { // 거리가 5보다 가까운 플레이어가 없으면
                b_IsSearch = false;
            }
        }
    }
}