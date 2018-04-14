using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Tower : EnemyGeneral
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
    EnemySpriteState e_NetworkSpriteState;
    bool b_NetworkFired;
    double f_LastNetworkDataReceivedTime;

    // Use this for initialization
    void Start()
    {
        base.f_Enemy_HP = 100f;
        base.f_EnemyMovingSpeed = 0f;
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
            if (e_EnemySpriteState != EnemySpriteState.Dead)
            {
                e_EnemySpriteState = EnemySpriteState.Idle;

                v_TargetPosition = Target.transform.position;

                // movement synced by Photon View
                UpdatePosition();

                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                UpdateAnimationControl(e_EnemySpriteState, b_EnemyFired);
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
        if (e_NetworkSpriteState == EnemySpriteState.Idle)
        {
            EnemyAnimator.SetBool("Run", false);
        }
        if (e_NetworkSpriteState == EnemySpriteState.Run)
        {
            EnemyAnimator.SetBool("Run", true);
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
    public override void SpineOnEvent(TrackEntry trackIndex, Spine.Event e)
    {
        if (e.Data.name == "Shoot_Start")
        {
            b_EnemyFired = true;
        }
        else if (e.data.name == "Shoot_End")
        {
            b_EnemyFired = false;
        }
    }

    //애니메이션 컨트롤(weaponSpineControl, fireBullet 모두 처리하는 중)
    protected override void UpdateAnimationControl(EnemySpriteState _e_EnemySpriteState, bool _b_EnemyFired)
    {
        WeaponSpineControl(_b_EnemyFired);
        if (_e_EnemySpriteState == EnemySpriteState.Idle)
        {
            EnemyAnimator.SetBool("Run", false);
        }
        if (_e_EnemySpriteState == EnemySpriteState.Run)
        {
            EnemyAnimator.SetBool("Run", true);
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
        GameObject bullet = Instantiate(this.EnemyBullet, muzzlePos, Quaternion.identity);
        EnemyBullet temp_bullet = bullet.GetComponent<EnemyBullet>();
        temp_bullet.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet.s_Victim = "Player";
        bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - this.EnemyWeapon.transform.position).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
    }

    [PunRPC]
    void FireAnimationNetwork()
    {
        if (b_NetworkFired)
        {
            Spine_EnemyWeaponAnim.state.SetAnimation(0, "Shoot", true);
        }
        else
        {
            Spine_EnemyWeaponAnim.state.SetAnimation(0, "Shoot", false);
        }
    }

    protected override void WeaponSpineControl(bool _b_EnemyFired)
    {
        if (b_IsSearch == true)
        {
            if (!_b_EnemyFired && Target.GetComponent<CharacterGeneral>().n_hp != 0)
            {
                FireBullet();
                Spine_EnemyWeaponAnim.state.SetAnimation(0, "Shoot", false);
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            stream.SendNext(v_NetworkPosition); // 현재 위치가 아니라 움직일 위치를 보내주는게 좋음
            stream.SendNext(e_EnemySpriteState);
            stream.SendNext(b_EnemyFired);
            stream.SendNext(v_TargetPosition);
        }
        else
        {
            // Network player, receive data
            v_NetworkPosition = (Vector3)stream.ReceiveNext();
            e_NetworkSpriteState = (EnemySpriteState)stream.ReceiveNext();
            b_NetworkFired = (bool)stream.ReceiveNext();
            v_NetworkTargetPos = (Vector3)stream.ReceiveNext();

            // RotateGun(_v_MousePos);
            f_LastNetworkDataReceivedTime = info.timestamp;
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

    [PunRPC]
    void TakeDamage(float _f_Damage)
    {
        this.f_Enemy_HP -= _f_Damage;
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        base.OnTriggerEnter2D(col);
    }

    protected override void Search()
    {
        Target = GameObject.FindWithTag("Player");
        f_Distance = Vector3.Distance(Target.transform.position, this.transform.position);
        if (f_Distance <= 5)
        {
            b_IsSearch = true;
        }
        else
        {
            b_IsSearch = false;
        }
    }
}