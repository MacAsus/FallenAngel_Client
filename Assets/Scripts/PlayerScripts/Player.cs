using UnityEngine;

public class Player : CharacterGeneral
{
    public GameObject myPlayer; //카메라 제어

    public static GameObject LocalPlayerInstance;
    
    public GameObject Main_Bullet; //주무장 총알 프리팹
    public GameObject Sub_Bullet; //부무장 총알 프리팹

    //무기 및 탄약 정보를 받아옴
    public GeneralInitialize.GunParameter cur_Weapon, Weapon1, Weapon2;

    protected Vector3 v_MousePos;

    //Photon Value
    protected Vector3 v_NetworkMousePos;

    protected void UpdateMousePosition()
    {
        RotateGun(v_NetworkMousePos);
    }
    protected void UpdatePosition()
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

        v_NetworkPosition = new Vector3(rigid.position.x + (f_Speed * tempx * Time.deltaTime), rigid.position.y + (f_Speed * tempy * Time.deltaTime));
        rigid.velocity = new Vector2(f_Speed * tempx, f_Speed * tempy);
    }
    
    protected override void WeaponSpineControl(bool _b_Fired, bool _b_Reload)
    {
        if(!_b_Fired && !_b_Reload) // 기본 상태일 때
        {
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
            }
            if (cur_Weapon == Weapon2)
            {
                if (Input.GetKey(KeyCode.Mouse0) && cur_Weapon.f_Magazine > 0)
                {
                    FireBullet();
                    spine_GunAnim.state.SetAnimation(0, "Hg_Shoot", false);
                    PlayerSound.instance.Play_Sound_Sub_Shoot();
                    --cur_Weapon.f_Magazine;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    spine_GunAnim.state.SetAnimation(0, "Hg_Reload", false);
                    PlayerSound.instance.Play_Sound_Sub_Reload();
                    cur_Weapon.f_Magazine = Util.F_HG_MAGAZINE;
                }
            }
            if (cur_Weapon.f_Magazine == 0) // 장탄수가 0일 때
            {
                if (Input.GetKey(KeyCode.Mouse0))
                {
                    PlayerSound.instance.Play_Sound_Zero_Shoot();
                }
            }
        }
    }

    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            stream.SendNext(v_NetworkPosition); // 현재 위치가 아니라 움직일 위치를 보내주는게 좋음
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_MousePos);
        }
        else
        {
            // Network player, receive data
            v_NetworkPosition = (Vector3)stream.ReceiveNext();
            e_NetworkSpriteState = (SpriteState)stream.ReceiveNext();
            b_NetworkFired = (bool)stream.ReceiveNext();
            v_NetworkMousePos = (Vector3)stream.ReceiveNext();

            // RotateGun(_v_MousePos);
            f_LastNetworkDataReceivedTime = info.timestamp;
        }
    }
    protected void ChangeWeapon()
    {
        if(photonView.isMine == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Muzzle = GameObject.Find("Ar_muzzle");
                cur_Weapon = Weapon1;
                spine_GunAnim.skeleton.SetSkin(cur_Weapon.s_GunName);
            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Muzzle = GameObject.Find("Hg_Muzzle");
                cur_Weapon = Weapon2;
                spine_GunAnim.skeleton.SetSkin(cur_Weapon.s_GunName);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        NetworkUtil.SetPlayer();
    }
    public void OnLeftLobby() {
        NetworkUtil.SetPlayer();
    }
}
