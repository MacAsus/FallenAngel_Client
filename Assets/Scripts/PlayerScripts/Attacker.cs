using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : Player {

    private PhotonVoiceRecorder recorder;
    private PhotonVoiceSpeaker speaker;

    void Start()
    {
        s_tag = Util.S_ENEMY;

        Main_Bullet = Resources.Load("BulletPrefab/" + Util.S_AR_BULLET_NAME) as GameObject;
        Sub_Bullet = Resources.Load("BulletPrefab/" + Util.S_LASER_NAME) as GameObject;

        Weapon1 = new GeneralInitialize.GunParameter(Util.S_AR_NAME, Util.S_AR_BULLET_NAME, Util.F_AR_BULLET_SPEED, Util.F_AR_BULLET_DAMAGE, Util.F_AR_MAGAZINE);
        Weapon2 = new GeneralInitialize.GunParameter(Util.S_LASER_NAME, Util.S_LASER_NAME, 0, Util.F_LASER_DAMAGE, Util.F_LASER_MAGAZINE);


        InitializeParam();


        cur_Weapon = Weapon1;
        Muzzle = Muzzle1;
        spine_GunAnim.Skeleton.SetSkin(Weapon1.s_GunName);
        b_NeedtoRotate = true;

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
	void Update ()
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
                RotateGun(v_MousePos, true);
                ChangeWeapon();
                UpdateRecorderSprite();
                Debug.Log("원하는것 call!!");
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
        if(OptionModal.IsActive) { // 옵션창이 켜져있으면 무기 사용 X
            return;
        }

        if (!_b_Fired && !_b_Reload) // 기본 상태일 때
        {
            if (Weapon2.f_Magazine == 0.0f)
            {
                Skill = false;
                Timer += Time.deltaTime;
                if (Timer >= Util.F_LASER)
                {
                    Timer = 0;
                    Skill = true;
                    Weapon2.f_Magazine = Util.F_LASER_MAGAZINE;
                }
            }
            if (cur_Weapon == Weapon1)
            {
                if (Input.GetKey(KeyCode.Mouse0) && cur_Weapon.f_Magazine > 0)
                {
                    FireBullet();
                    spine_GunAnim.state.SetAnimation(0, "Ar_Shoot", false);
                    PlayerSound.instance.Play_Sound_Main_Shoot();
                    --cur_Weapon.f_Magazine;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    spine_GunAnim.state.SetAnimation(0, "Ar_Reload", false);
                    PlayerSound.instance.Play_Sound_Main_Reload();
                    cur_Weapon.f_Magazine = Util.F_AR_MAGAZINE;
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
                if (Input.GetKey(KeyCode.Mouse0) && Skill == true)
                {
                    FireBullet();
                    spine_GunAnim.state.SetAnimation(0, "Laser_Shoot", false);
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
            GameObject bullet = Instantiate(Sub_Bullet, muzzlePos, Muzzle.transform.rotation);
            BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = Weapon2;
            temp_bullet.s_Victim = s_tag;
            //bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - g_Weapon.transform.position).normalized * 20.0f;
            //StartCoroutine(Death_Wait_Sec(1.0f));
            //Destroy(bullet);
        }
    }
}