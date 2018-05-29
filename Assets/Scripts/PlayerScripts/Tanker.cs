using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tanker : Player
{

    private PhotonVoiceRecorder recorder;
    private PhotonVoiceSpeaker speaker;


    void Start()
    {
        s_tag = Util.S_ENEMY;

        Main_Bullet = Resources.Load("BulletPrefab/" + Util.S_SMG_BULLET_NAME) as GameObject;

        Weapon1 = new GeneralInitialize.GunParameter(Util.S_SMG_NAME, Util.S_SMG_BULLET_NAME, Util.F_SMG_BULLET_SPEED, Util.F_SMG_BULLET_DAMAGE, Util.F_SMG_MAGAZINE);
        Weapon2 = new GeneralInitialize.GunParameter(Util.S_SHIELD_NAME, " " , 0, 0, Util.F_SHIELD_HP);
        cur_Weapon = Weapon1;


        InitializeParam();

        cur_Weapon = Weapon1;
        Muzzle = Muzzle1;
        spine_GunAnim.Skeleton.SetSkin(Weapon1.s_GunName);

        transform.Find("Shield").GetComponent<BoxCollider2D>().enabled = false;

        if (UI != null)
        {
            GameObject _uiGo = Instantiate(UI) as GameObject;
            _uiGo.SendMessage("SetTarget", this, SendMessageOptions.RequireReceiver);
        }
        else
        {
            Debug.LogWarning("<Color=Red><a>Missing</a></Color> PlayerUiPrefab reference on player Prefab.", this);
        }

        PhotonNetwork.sendRate = 500 / Launcher.MaxPlayersPerRoom;
        PhotonNetwork.sendRateOnSerialize = 500 / Launcher.MaxPlayersPerRoom;

        // Find Photon Voice Recorder And Speaker
        recorder = this.GetComponent<PhotonVoiceRecorder>();
        speaker = this.GetComponent<PhotonVoiceSpeaker>();
    }

    // Update is called once per frame
    void Update()
    {
        // if this view is not mine, then do not update
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                // movement synced by Photon View
                UpdatePosition();

                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                UpdateAnimationControl(e_SpriteState, b_Fired, b_Reload);
                RotateGun(v_MousePos, b_NeedtoRotate);
                
                ChangeWeapon();
                UpdateRecorderSprite();
            }
        }
        else
        {
            UpdateNetworkedPosition();
            UpdateMousePosition(); // Need To Lerp
            UpdateNetworkAnimationControl();
            UpdateNetworkRecorderSprite();
        }
    }

    void LateUpdate()
    {
        if (photonView.isMine == true)
        {
            myPlayer = InGame.Player;
            Camera.main.transform.position = myPlayer.transform.position - Vector3.forward;
        }
    }

    protected override void WeaponSpineControl(bool _b_Fired, bool _b_Reload)
    {
        if (OptionModal.IsActive)
        { // 옵션창이 켜져있으면 무기 사용 X
            return;
        }

        if (!_b_Fired && !_b_Reload) // 기본 상태일 때
        {
            if (cur_Weapon == Weapon1)
            {
                b_SlowRun = false;
                b_NeedtoRotate = true;
                UpdateShieldHp();
                if (Input.GetKey(KeyCode.Mouse0) && cur_Weapon.f_Magazine > 0)
                {
                    FireBullet();
                    spine_GunAnim.state.SetAnimation(0, "Smg_Shoot", false);
                    PlayerSound.instance.Play_Sound_Main_Shoot();
                    --cur_Weapon.f_Magazine;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    spine_GunAnim.state.SetAnimation(0, "Smg_Reload", false);
                    PlayerSound.instance.Play_Sound_Main_Reload();
                    cur_Weapon.f_Magazine = Util.F_SMG_MAGAZINE;
                }
                if (cur_Weapon.f_Magazine == 0) // 장탄수가 0일 때
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        PlayerSound.instance.Play_Sound_Zero_Shoot();
                    }
                }
            }
            if (cur_Weapon == Weapon2)
            {
                b_NeedtoRotate = false;

                if (Input.GetKey(KeyCode.Mouse0) && Skill == true)
                {
                    spine_GunAnim.state.SetAnimation(0, "ShieldUp", true);
                    PlayerSound.instance.Play_Sound_Melee_Shoot();
                    b_SlowRun = true;
                    transform.Find("Shield").GetComponent<BoxCollider2D>().enabled = true;
                }
                else if (Input.GetKey(KeyCode.Mouse0) && Skill == false)
                {
                    UpdateShieldHp();
                    //금지 사운드
                }
                //방패가 깨지는 경우
                else if (Weapon2.f_Magazine <= 0.0f)
                {
                    Skill = false;
                    PlayerSound.instance.Play_Sound_Melee_Hit();

                    Timer += Time.deltaTime;
                    if (Timer > Util.F_SHIELD)
                    {
                        Timer = 0;
                        Skill = true;
                        cur_Weapon.f_Magazine = Util.F_SHIELD_HP;
                    }
                }
                else
                {
                    spine_GunAnim.state.SetAnimation(0, "Idle", true);
                    b_SlowRun = false;
                    UpdateShieldHp();
                }
            }
        }
    }
    protected override void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired, bool _b_Reload)
    {
        base.UpdateAnimationControl(_e_SpriteState, _b_Fired, _b_Reload);
        if (e_SpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run_Slow", b_SlowRun);
        }else
        {
            a_Animator.SetBool("Run_Slow", false);
        }
        if (b_SlowRun)
        {
            a_Animator.speed = 0.5f;
        }else
        {
            a_Animator.speed = 1;
        }
    }

    protected override void FireBullet()
    {
        Debug.Log("FireBullet called");
        Vector3 v_muzzle = Muzzle.transform.position;
        Vector3 v_bulletSpeed = (Muzzle.transform.position - g_Weapon.transform.position).normalized * cur_Weapon.f_BulletSpeed;

        this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
        this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others, cur_Weapon.s_GunName);
    }

    [PunRPC]
    protected override void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        if (cur_Weapon == Weapon1)
        {
            GameObject bullet = Instantiate(Main_Bullet, muzzlePos, Muzzle.transform.rotation);
            BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = Weapon1;
            temp_bullet.s_Victim = s_tag;
            bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - g_Weapon.transform.position).normalized * Weapon1.f_BulletSpeed;
        }
    }

    void UpdateShieldHp()
    {
        while (Weapon2.f_Magazine >= Util.F_SHIELD_HP)
        {
            Weapon2.f_Magazine += 10 * Time.deltaTime;

            if (Weapon2.f_Magazine >= Util.F_SHIELD_HP)
            {
                Weapon2.f_Magazine = Util.F_SHIELD_HP;
                break;
            }
        }
    }
}
