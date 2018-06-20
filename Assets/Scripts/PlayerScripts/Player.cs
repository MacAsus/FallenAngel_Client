using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using System.Collections;

public class Player : CharacterGeneral
{
    public bool Skill = true;
    public float Timer = 0.0f;

    public GameObject myPlayer; //카메라 제어

    public static GameObject LocalPlayerInstance;
    
    public GameObject Main_Bullet; //주무장 총알 프리팹
    public GameObject Sub_Bullet; //부무장 총알 프리팹
    public GameObject Muzzle1, Muzzle2;

    public ParticleSystem DeadPs;

    protected bool b_NeedtoRotate = true;
    protected bool b_SlowRun = false;
    protected bool b_Knock = false;

    //무기 및 탄약 정보를 받아옴
    public GeneralInitialize.GunParameter cur_Weapon, Weapon1, Weapon2;

    protected Vector3 v_MousePos;

    //Photon Value
    protected Vector3 v_NetworkMousePos;
    protected bool b_NetworkIsTransmitting = false;

    void Start()
    {
        DeadPs = GetComponentInChildren<ParticleSystem>();
        DeadPs.Stop();
    }

    protected void UpdateMousePosition()
    {
        RotateGun(v_NetworkMousePos, b_NeedtoRotate);
    }

    protected virtual void UpdatePosition()
    {
        int tempx = 0;
        int tempy = 0;
        float temp_speed = f_Speed;
        if (!b_Knock)
        {
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // If chat module is enabled, then block user moving
                if (InGame.keyboardInputDisabled)
                {
                    return;
                }

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
            if (b_SlowRun)
            {
                temp_speed = f_Speed / 2;
            }
            v_NetworkPosition = new Vector3(rigid.position.x + (temp_speed * tempx * Time.deltaTime), rigid.position.y + (temp_speed * tempy * Time.deltaTime));

            rigid.velocity = new Vector2(temp_speed * tempx, temp_speed * tempy);
        }
    }

    protected virtual void UpdateRecorderSprite() {
        PhotonVoiceRecorder recorder = this.GetComponent<HighlightIcon>().recorder;
        this.GetComponent<HighlightIcon>().recorderSprite.enabled = (recorder != null && recorder.IsTransmitting &&
                PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined);
    }

    protected virtual void UpdateNetworkRecorderSprite()
    {
        this.GetComponent<HighlightIcon>().recorderSprite.enabled = b_NetworkIsTransmitting;
    }

    protected void GetAimDegree(Vector3 v_TargetPos)
    {

        float x = g_Weapon.position.x - v_TargetPos.x;
        float y = g_Weapon.position.y - v_TargetPos.y;


        f_AimDegree = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
    protected virtual void RotateGun(Vector3 v_TargetPos, bool b_NeedtoRotate)
    {

        GetAimDegree(v_TargetPos);
        if (b_NeedtoRotate)
        {
            g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_AimDegree));

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
        }else
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
            stream.SendNext(this.GetComponent<HighlightIcon>().recorder != null && this.GetComponent<HighlightIcon>().recorder.IsTransmitting &&
                PhotonVoiceNetwork.ClientState == ExitGames.Client.Photon.LoadBalancing.ClientState.Joined); // 보이스 챗 말풍선 아이콘
        }
        else
        {
            // Network player, receive data
            v_NetworkPosition = (Vector3)stream.ReceiveNext();
            e_NetworkSpriteState = (SpriteState)stream.ReceiveNext();
            b_NetworkFired = (bool)stream.ReceiveNext();
            v_NetworkMousePos = (Vector3)stream.ReceiveNext();
            b_NetworkIsTransmitting = (bool)stream.ReceiveNext();

            // RotateGun(_v_MousePos);
            f_LastNetworkDataReceivedTime = info.timestamp;
        }
    }

    protected virtual void ChangeWeapon()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Muzzle = Muzzle1;
            cur_Weapon = Weapon1;
            spine_GunAnim.Skeleton.SetSkin(Weapon1.s_GunName);
            spine_GunAnim.Skeleton.SetToSetupPose();
            spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);
            b_Fired = false;
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (Muzzle2 != null)
            {
                Muzzle = Muzzle2;
            }
            else
            {
                Muzzle = null;
            }
            cur_Weapon = Weapon2;
            spine_GunAnim.Skeleton.SetSkin(Weapon2.s_GunName);
            spine_GunAnim.Skeleton.SetToSetupPose();
            spine_GunAnim.AnimationState.Apply(spine_GunAnim.Skeleton);
            b_Fired = false;
        }
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info) {
        NetworkUtil.SetPlayer();
    }
    public void OnLeftLobby() {
        NetworkUtil.SetPlayer();
    }

    
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        //총알과 충돌
        if (col.gameObject.layer == LayerMask.NameToLayer("Bullet"))
        {
            //데미지
            if (col.gameObject.GetComponent<BulletGeneral>().s_Victim == Util.S_PLAYER)
            {
                bool IsMine = gameObject.GetComponent<CharacterGeneral>().photonView.isMine;
                if (IsMine)
                {
                    gameObject.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                }
            }
            
            //체력회복
            if (col.gameObject.GetComponent<BulletGeneral>().s_Help == Util.S_PLAYER && col.gameObject.transform.Find("Trigger").tag != Util.S_HEALER)
            {
                bool IsMine = gameObject.GetComponent<CharacterGeneral>().photonView.isMine;
                if (IsMine)
                {
                    //총알의 데미지만큼 체력 회복
                    gameObject.GetComponent<PhotonView>().RPC("PlayerHealing", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                }
            }
        }

        //Player가 적과 닿았을 경우
        if (col.gameObject.layer == LayerMask.NameToLayer("EnemyBody"))
        {
            bool IsMine = gameObject.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            {
                Vector3 temp_Dir = Vector3.Normalize(transform.position - col.transform.position);
                //PlayerHitMove(temp_Dir);
                gameObject.GetComponent<PhotonView>().RPC("PlayerHitMove", PhotonTargets.All, temp_Dir);
                gameObject.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, col.gameObject.GetComponentInParent<EnemyGeneral>().f_Damage);
            }
        }
    }

    [PunRPC]
    protected virtual void PlayerTakeDamage(float _f_Damage)
    {
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            this.n_hp -= _f_Damage;
            this.b_UnHit = true;
            transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine("IsDamaged");
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            DeadPs.Play();
            e_SpriteState = SpriteState.Dead;
            StartCoroutine(Death_Wait_Sec(0.1f));
        }
    }

    [PunRPC]
    protected void PlayerHealing(float _f_Heal)
    {
        Debug.Log("PlayerHealing 호출!!!");
        if (this.n_hp > 0)
        {
            if (this.n_hp + _f_Heal >= f_MaxHp)
            {
                n_hp = f_MaxHp;
            }
            else
            {
                this.n_hp += _f_Heal;
            }
        }
        StartCoroutine("IsHealing");
    }

    [PunRPC]
    protected void PlayerHitMove(Vector3 dir)
    {
        StartCoroutine(KnockBackTimer(0.25f));
        this.gameObject.GetComponentInParent<Rigidbody2D>().velocity = dir.normalized * 15.0f;

    }

    IEnumerator KnockBackTimer(float time)
    {
        b_Knock = true;
        yield return new WaitForSeconds(time);
        b_Knock = false;
    }

}
