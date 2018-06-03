using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heavy : Player {

    private PhotonVoiceRecorder recorder;
    private PhotonVoiceSpeaker speaker;

    public float f_FullSpinGauge = 3;
    private float f_SpinGauge = 0;
    private float f_Recoil = 0;
    private bool b_SpinBool = true;
    private bool b_CoolBool = true;
    private string s_CurAnimation = " ";

    private Vector3 v_TerminalBulletPos;

    void Start()
    {
        s_tag = Util.S_ENEMY;

        Main_Bullet = Resources.Load("BulletPrefab/" + Util.S_GATLING_BULLET_NAME) as GameObject;
        Sub_Bullet = Resources.Load("BulletPrefab/" + Util.S_GRENADE_BULLET_NAME) as GameObject;

        Weapon1 = new GeneralInitialize.GunParameter(Util.S_GATLING_NAME, Util.S_GATLING_BULLET_NAME, Util.F_GATLING_BULLET_SPEED, Util.F_GATLING_BULLET_DAMAGE, Util.F_GATLING_MAGAZINE);
        Weapon2 = new GeneralInitialize.GunParameter(Util.S_GRENADE_NAME, Util.S_GRENADE_BULLET_NAME, Util.F_GRENADE_BULLET_SPEED, Util.F_GRENADE_BULLET_DAMAGE, Util.F_GRENADE_MAGAZINE);

        InitializeParam();

        cur_Weapon = Weapon1;
        Muzzle = Muzzle1;
        spine_GunAnim.Skeleton.SetSkin(Weapon1.s_GunName);

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
                
                if (Input.GetMouseButtonDown(0))
                {
                    v_TerminalBulletPos = v_MousePos;
                }
                
                // movement synced by Photon View
                UpdatePosition();

                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                
                RotateGun(v_MousePos, b_NeedtoRotate);
                UpdateAnimationControl(e_SpriteState, b_Fired, b_Reload);
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

            //if (b_UnHit == true)
            //{
            //    CameraShaking.instance.ShakeCamera(0.5f);
            //}
        }
    }

    protected override void WeaponSpineControl(bool _b_Fired, bool _b_Reload)
    {
        if (OptionModal.IsActive)
        { // 옵션창이 켜져있으면 무기 사용 X
            return;
        }

        if (Weapon2.f_Magazine == 0.0f)
        {
            Skill = false;
            Timer += Time.deltaTime;
            if (Timer >= Util.F_GRENADE)
            {
                Timer = 0;
                Skill = true;
                Weapon2.f_Magazine = Util.F_GRENADE_MAGAZINE;
            }
        }
        if (!_b_Fired && !_b_Reload) // 기본 상태일 때
        {
            if (cur_Weapon == Weapon1)
            {
                if (Input.GetKey(KeyCode.Mouse0) && cur_Weapon.f_Magazine > 0)
                {
                    b_SlowRun = true;

                    if (s_CurAnimation != "Gatling_Spin")
                    {
                        s_CurAnimation = "Gatling_Spin";
                        spine_GunAnim.state.SetAnimation(0, "Gatling_Spin", true);
                    }
                    if ((f_SpinGauge < f_FullSpinGauge) && b_SpinBool)
                    {
                        StartCoroutine(GatlingSpin());
                    }
                    else if (f_SpinGauge >= f_FullSpinGauge)
                    {
                        StartCoroutine("FireRate");
                        f_SpinGauge = f_FullSpinGauge;
                        f_Recoil = Random.Range(-10.0f, 10.0f);
                        FireBullet();

                        PlayerSound.instance.Play_Sound_Main_Shoot();
                        --cur_Weapon.f_Magazine;

                    }

                }
                else if (!Input.GetKey(KeyCode.Mouse0) || cur_Weapon.f_Magazine == 0)
                {
                    b_SlowRun = false;
                    f_Recoil = 0;
                    if ((f_SpinGauge > 0) && b_CoolBool)
                    {
                        StartCoroutine(GatlingCool());
                    }else if (f_SpinGauge < 0)
                    {
                        f_SpinGauge = 0;
                    }

                    if (f_SpinGauge == 0)
                    {
                        s_CurAnimation = "Idle";
                        spine_GunAnim.state.ClearTrack(0);
                        spine_GunAnim.Skeleton.SetToSetupPose();
                    }
                }
                if (Input.GetKey(KeyCode.R))
                {
                    f_SpinGauge = 0;
                    spine_GunAnim.state.SetAnimation(0, "Gatling_Reload", false);
                    PlayerSound.instance.Play_Sound_Main_Reload();
                    cur_Weapon.f_Magazine = Util.F_GATLING_MAGAZINE;
                }
                if (cur_Weapon.f_Magazine == 0)
                {
                    if (Input.GetKey(KeyCode.Mouse0))
                    {
                        PlayerSound.instance.Play_Sound_Zero_Shoot();
                    }
                }
            }
            if (cur_Weapon == Weapon2)
            {
                b_SlowRun = false;
                //b_SpinBool = true;
                //b_CoolBool = true;
                f_SpinGauge = 0;
                f_Recoil = 0;

                if (Input.GetKey(KeyCode.Mouse0) && Skill == true)
                {
                    FireBullet();
                    spine_GunAnim.state.SetAnimation(0, "Grenade_Spin", false);
                    PlayerSound.instance.Play_Sound_Sub_Shoot();
                    --cur_Weapon.f_Magazine;
                }
                if (Input.GetKey(KeyCode.Mouse0) && Skill == false)
                {
                    PlayerSound.instance.Play_Sound_Zero_Shoot();
                }
            }
        }
    }
    protected override void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired, bool _b_Reload)
    {
        base.UpdateAnimationControl(_e_SpriteState, _b_Fired, _b_Reload);
        //if (e_SpriteState == SpriteState.Run)
        //{
        //    a_Animator.SetBool("Run_Slow", b_SlowRun);
        //}
        //else
        //{
        //    a_Animator.SetBool("Run_Slow", false);
        //}
        if (b_SlowRun)
        {
            a_Animator.speed = 0.5f;
        }
        else
        {
            a_Animator.speed = 1;
        }
    }

    protected override void RotateGun(Vector3 v_TargetPos, bool b_NeedtoRotate)
    {
        
        GetAimDegree(v_TargetPos);
        f_AimDegree += f_Recoil;
        if (b_NeedtoRotate)
        {
            if (f_SpinGauge > 0)
            {
                if(Mathf.Round(f_AimDegree) != Mathf.Round(g_Weapon.rotation.z))
                {
                    g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_AimDegree));
                }
            }
            else
            {
                g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_AimDegree));
            }
            if (f_AimDegree > -90 && f_AimDegree <= 90)
            {
                g_Sprite.localScale = new Vector3(f_SpritelocalScale, f_SpritelocalScale);
                g_Weapon.localScale = new Vector3(f_WeaponlocalScale, f_WeaponlocalScale);
            }
            else
            {
                g_Sprite.localScale = new Vector3(-f_SpritelocalScale, f_SpritelocalScale);
                g_Weapon.localScale = new Vector3(f_WeaponlocalScale, -f_WeaponlocalScale);
            }
        }
        else
        {
            g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            if (f_AimDegree > -90 && f_AimDegree <= 90)
            {
                g_Sprite.localScale = new Vector3(f_SpritelocalScale, f_SpritelocalScale);
                g_Weapon.localScale = new Vector3(f_WeaponlocalScale, f_WeaponlocalScale);
            }
            else
            {
                g_Sprite.localScale = new Vector3(-f_SpritelocalScale, f_SpritelocalScale);
                g_Weapon.localScale = new Vector3(-f_WeaponlocalScale, f_WeaponlocalScale);
            }
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

        if (cur_Weapon == Weapon2)
        {
            GameObject Grenade = Instantiate(Sub_Bullet, muzzlePos, Muzzle.transform.rotation);
            BulletGeneral temp_bullet = Grenade.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = Weapon2;
            temp_bullet.s_Victim = s_tag;
            Grenade.GetComponent<Rigidbody2D>().velocity = (muzzlePos - g_Weapon.transform.position).normalized * Weapon2.f_BulletSpeed;
        }
    }

    protected override void ChangeWeapon()
    {
        if (photonView.isMine == true)
        {
            //개틀링건이 가열중일 때 무기교체 시
            if (cur_Weapon == Weapon1 && f_SpinGauge > 0)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    f_SpinGauge = 0;
                    spine_GunAnim.state.SetAnimation(0, "Gatling_Spin", false);
                    Muzzle = Muzzle2;
                    cur_Weapon = Weapon2;
                    spine_GunAnim.Skeleton.SetSkin(Weapon2.s_GunName);
                    spine_GunAnim.Skeleton.SetToSetupPose();
                    spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);
                }
            }
            //개틀링건이 가열중이지 않을 때 무기교체 시
            else if (cur_Weapon == Weapon1 && f_SpinGauge == 0)
            {
                if (Input.GetKeyDown(KeyCode.Alpha2))
                {
                    Muzzle = Muzzle2;
                    cur_Weapon = Weapon2;
                    spine_GunAnim.Skeleton.SetSkin(Weapon2.s_GunName);
                    spine_GunAnim.Skeleton.SetToSetupPose();
                    spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);
                }
            }
            //일반상태
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Muzzle = Muzzle1;
                cur_Weapon = Weapon1;
                spine_GunAnim.Skeleton.SetSkin(Weapon1.s_GunName);
                spine_GunAnim.Skeleton.SetToSetupPose();
                spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);

            }
        }
    }

    IEnumerator GatlingSpin()
    {
        b_SpinBool = false;
        f_SpinGauge += 1.0f;
        yield return new WaitForSeconds(0.5f);
        b_SpinBool = true;
    }
    IEnumerator FireRate()
    {
        b_Fired = true;
        yield return new WaitForSeconds(0.05f);
        b_Fired = false;
    }
    IEnumerator GatlingCool()
    {
        b_CoolBool = false;
        f_SpinGauge -= 1.0f;
        yield return new WaitForSeconds(0.5f);
        b_CoolBool = true;
    }
}
