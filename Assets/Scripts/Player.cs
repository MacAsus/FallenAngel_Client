using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Player : CharacterGeneral
{
    public static GameObject LocalPlayerInstance;

    public GeneralInitialize.GunParameter cur_Weapon;

    public GeneralInitialize.GunParameter Weapon1, Weapon2;

    public Sprite bulletImage;

    public WeaponGeneral weaponScript;

    public GameObject g_Muzzle;


    public string s_jobname;
    public int n_Magazine = 10;

    Vector3 v_MousePos = new Vector3();

    // Use this for initialization
    void Start()
    {
        weaponScript = GetComponent<WeaponGeneral>();
        //디버그용
        //로비에서 총까지 다 구현하면 지울것
        GeneralInitialize.GunParameter tempParam = new GeneralInitialize.GunParameter("Hg_Brownie", 10, 5, "Hg_Norm", 5);
        bulletImage = tempParam.BulletImage;
        //여기까지
        PhotonNetwork.sendRate = 500 / Launcher.MaxPlayersPerRoom;
        PhotonNetwork.sendRateOnSerialize = 500 / Launcher.MaxPlayersPerRoom;
        InitializeParam();

        //디버그용
        //위에 코드 지우면 주석 풀것
        //if(Weapon1 != null)
        //{
        //    cur_Weapon = Weapon1;
        //}else if(Weapon2 != null)
        //{
        //    cur_Weapon = Weapon2;
        //}

        Weapon1 = tempParam;
        Weapon2 = tempParam;
        cur_Weapon = tempParam;

        FindMuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        // if this view is not mine, then do not update
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }


        if (e_SpriteState != SpriteState.Dead)
        {
            e_SpriteState = SpriteState.Idle;

            v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // movement synced by Photon View
            CharacterMovement();

            // below value should be setted by manually

            //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
            AnimationControl(e_SpriteState, b_Fired);
            RotateGun(v_MousePos);
            ChangeWeapon();
        }

    }

    //캐릭터 이동 
    protected override void CharacterMovement()
    {
        int tempx = 0;
        int tempy = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            e_SpriteState = SpriteState.Run;
            if (Input.GetKey(KeyCode.A))
            {
                tempx -= 1;
            }
            if(Input.GetKey(KeyCode.D))
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
        rigid.velocity = new Vector2(f_Speed * tempx, f_Speed * tempy);

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
    protected override void AnimationControl(SpriteState _e_SpriteState, bool _b_Fired)
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
        Debug.Log("FireBullet called");
        Vector3 v_muzzle = g_Muzzle.transform.position;
        Vector3 v_bulletSpeed = (g_Muzzle.transform.position - g_Weapon.transform.position).normalized * cur_Weapon.f_BulletSpeed;

        this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
    }

    [PunRPC]
    void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed) {
        Debug.Log("FireBulletNetwork called");
        if(this.photonView.isMine)
        {
            GameObject bullet = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
            BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_Weapon.f_Damage);
            bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - this.g_Weapon.transform.position).normalized * this.cur_Weapon.f_BulletSpeed;
        }
        
    }

    protected override void WeaponSpineControl(bool _b_Fired)
    {
        
        if (Input.GetKey(KeyCode.Mouse0) && !_b_Fired) // triggerd by local
        {
            // GameObject bullet = weaponScript.FireBullet();
            FireBullet();
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_MousePos);
        }
        else
        {
            // Network player, receive data
            SpriteState _e_SpriteState = (SpriteState)stream.ReceiveNext();
            bool _b_Fired = (bool)stream.ReceiveNext();
            Vector3 _v_MousePos = (Vector3)stream.ReceiveNext();
            
            // below value should be setted by manually
            AnimationControl(_e_SpriteState, _b_Fired);
            RotateGun(_v_MousePos);

            // timestamp: info.timestamp
        }
    }

    //총 변경
    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cur_Weapon = Weapon1;
            FindMuzzle();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cur_Weapon = Weapon2;
            FindMuzzle();
        }
        spine_GunAnim.skeleton.SetSkin(cur_Weapon.s_GunName);
    }

    //총구 위치 찾기
    void FindMuzzle()
    {
        if (cur_Weapon.s_GunName.Contains("Hg"))
        {
            g_Muzzle = GameObject.Find("Hg_Muzzle");
        }
        else if (cur_Weapon.s_GunName.Contains("Ar"))
        {
            g_Muzzle = GameObject.Find("Ar_Muzzle");
        }
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        
    }
}
