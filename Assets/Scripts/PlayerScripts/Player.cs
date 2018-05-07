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
        if (photonView.isMine == true)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                Muzzle = GameObject.Find("Ar_muzzle");
                cur_Weapon = Weapon1;
                spine_GunAnim.Skeleton.SetSkin("Ar");
                spine_GunAnim.Skeleton.SetToSetupPose();
                spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);

            }
            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                Muzzle = GameObject.Find("Hg_Muzzle");
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
}
