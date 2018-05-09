using UnityEngine;
using UnityEngine.UI;

public class Player : CharacterGeneral
{
    public GameObject myPlayer; //카메라 제어

    public static GameObject LocalPlayerInstance;
    
    public GameObject Main_Bullet; //주무장 총알 프리팹
    public GameObject Sub_Bullet; //부무장 총알 프리팹
    public GameObject Muzzle1, Muzzle2;

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
            /* Todo: 채팅칠때 캐릭터 움직이지 않도록 해야함
            if (GameObject.FindWithTag("GUI") && GameObject.FindWithTag("GUI").GetComponent<InputField>())
            {
                Debug.Log("GetComponent<InputField>().isActiveAndEnabled is" + GameObject.FindWithTag("GUI").GetComponent<InputField>().isActiveAndEnabled);
                return;
            }
            */
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

    protected void GetAimDegree(Vector3 v_TargetPos)
    {

        float x = g_Weapon.position.x - v_TargetPos.x;
        float y = g_Weapon.position.y - v_TargetPos.y;


        f_AimDegree = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
    protected void RotateGun(Vector3 v_TargetPos)
    {

        GetAimDegree(v_TargetPos);
        g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_AimDegree));

        if (f_AimDegree > -90 && f_AimDegree <= 90)
        {
            g_Sprite.localScale = new Vector3(f_SpritelocalScale, g_Sprite.localScale.y, g_Sprite.localScale.z);
            g_Weapon.localScale = new Vector3(g_Weapon.localScale.x, f_WeaponlocalScale, g_Weapon.localScale.z);
        }
        else
        {
            g_Sprite.localScale = new Vector3(-f_SpritelocalScale, g_Sprite.localScale.y, g_Sprite.localScale.z);
            g_Weapon.localScale = new Vector3(g_Weapon.localScale.x, -f_WeaponlocalScale, g_Weapon.localScale.z);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            Debug.Log("v_NetworkPosition is" + v_NetworkPosition);
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
        if (photonView.isMine == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Muzzle = Muzzle1;
                cur_Weapon = Weapon1;
                spine_GunAnim.Skeleton.SetSkin("Ar");
                spine_GunAnim.Skeleton.SetToSetupPose();
                spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Muzzle = Muzzle2;
                cur_Weapon = Weapon2;
                spine_GunAnim.Skeleton.SetSkin("Hg");
                spine_GunAnim.Skeleton.SetToSetupPose();
                spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);
            }
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        NetworkUtil.SetPlayer();
    }
    public void OnLeftLobby() {
        NetworkUtil.SetPlayer();
    }

    
    void OnTriggerEnter2D(Collider2D col)
    {

        if(col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            if (col.gameObject.GetComponent<BulletGeneral>().s_Victim == Util.S_PLAYER)
            {
                bool IsMine = gameObject.GetComponent<CharacterGeneral>().photonView.isMine;
                if (IsMine)
                {
                    PlayerSound.instance.Play_Sound_Gun_Hit();
                    gameObject.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                }
            }
        }

        if (col.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            bool IsMine = gameObject.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            {
                PlayerSound.instance.Play_Sound_Melee_Hit();
                gameObject.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, col.gameObject.GetComponent<EnemyGeneral>().f_Damage);
            }
        }
        
    }

}
