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
        Sub_Bullet = Resources.Load("BulletPrefab/" + Util.S_HG_BULLET_NAME) as GameObject;

        Weapon1 = new GeneralInitialize.GunParameter(Util.S_AR_NAME, Util.S_AR_BULLET_NAME, Util.F_AR_BULLET_SPEED, Util.F_AR_BULLET_DAMAGE, Util.F_AR_MAGAZINE);
        Weapon2 = new GeneralInitialize.GunParameter(Util.S_HG_NAME, Util.S_HG_BULLET_NAME, Util.F_HG_BULLET_SPEED, Util.F_HG_BULLET_DAMAGE, Util.F_HG_MAGAZINE);

        cur_Weapon = Weapon1;

        InitializeParam();

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
                RotateGun(v_MousePos);
                ChangeWeapon();
            }
        }
        else
        {
            UpdateNetworkedPosition();
            UpdateMousePosition(); // Need To Lerp
            UpdateNetworkAnimationControl();
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

    protected override void FireBullet()
    {
        Debug.Log("FireBullet called");
        Vector3 v_muzzle = Muzzle.transform.position;
        Vector3 v_bulletSpeed = (Muzzle.transform.position - g_Weapon.transform.position).normalized * cur_Weapon.f_BulletSpeed;

        this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
        this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
    }

    [PunRPC]
    protected override void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        if (cur_Weapon == Weapon1)
        {
            Debug.Log("FireBulletNetwork called");
            GameObject bullet = Instantiate(Main_Bullet, muzzlePos, Quaternion.identity);
            BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = Weapon1;
            temp_bullet.s_Victim = s_tag;
            bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - g_Weapon.transform.position).normalized * Weapon1.f_BulletSpeed;
        }

        if(cur_Weapon == Weapon2)
        {
            Debug.Log("FireBulletNetwork called");
            GameObject bullet = Instantiate(Sub_Bullet, muzzlePos, Quaternion.identity);
            BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
            temp_bullet.bulletInfo = Weapon2;
            temp_bullet.s_Victim = s_tag;
            bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - g_Weapon.transform.position).normalized * Weapon2.f_BulletSpeed;
        }
    }
}
