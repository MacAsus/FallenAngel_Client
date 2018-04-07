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
            AnimationControl(e_SpriteState, b_Fired);
            RotateGun(v_MousePos);
        }

    }

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
    protected void FireBullet()
    {
        Debug.Log("FireBullet called");
        Vector3 v_muzzle = weaponScript.getMuzzlePos();
        Vector3 v_bulletSpeed = weaponScript.getBulletSpeed();

        this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
    }

    [PunRPC]
    void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed) {
        Debug.Log("FireBulletNetwork called");
        GameObject bullet = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos- this.g_Weapon.transform.position).normalized * this.cur_Weapon.f_BulletSpeed;
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

    void fireControl(bool _fired, Vector3 _v_MousePos)
    {
        bool isNowFiring = this.b_Fired;
        this.b_Fired = _fired;
        if (!isNowFiring && this.b_Fired)
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }
        this.v_MousePos = _v_MousePos;
        RotateGun(v_MousePos);
    }

    void moveControl(SpriteState _spriteState)
    {
        if (_spriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_spriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }

    void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            cur_Weapon = Weapon1;

        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            cur_Weapon = Weapon2;
        }
        spine_GunAnim.skeleton.SetSkin(cur_Weapon.s_GunName);
    }
    //임시 충돌 테스트
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Bullet")
        {
            this.n_hp -= Weapon1.f_Damage; //맞은 총알이 어떤 무기에서 날아오는지 모르겠음
        }
    }
}
