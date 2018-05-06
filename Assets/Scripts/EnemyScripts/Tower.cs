using UnityEngine;

public class Tower : EnemyGeneral
{
    void Start()
    {
        n_hp = Util.F_TOWER_HP;
        f_Speed = Util.F_TOWER_SPEED;
        s_tag = Util.S_PLAYER;
        Target = GameObject.FindWithTag(s_tag);
        //임시 테스트용
        GeneralInitialize.GunParameter tempEnemyWeapon =
            new GeneralInitialize.GunParameter("Hg_Brownie", 10, 5, "Hg_Norm", 5);
        BulletImage = tempEnemyWeapon.BulletImage;
        cur_EnemyWeapon = tempEnemyWeapon;
        //여기까지
        InitializeParam();
    }

    void Update()
    {
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                if(Target)
                {
                    v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
                }

                UpdateAnimationControl(e_SpriteState, b_Fired, b_Reload);
                RotateGun(v_TargetPosition);
            }
        }
        else
        {
            UpdateNetworkAnimationControl();
        }

        Search(Util.F_TOWER_SEARCH);
    }
    
    protected override void FireBullet()
    {
        if (Muzzle) {
            Vector3 v_muzzle = Muzzle.transform.position;
            Vector3 v_bulletSpeed = (Muzzle.transform.position - (Target.transform.position + Util.V_ACCRUATE)).normalized * cur_EnemyWeapon.f_BulletSpeed;

            this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
            this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
        } else {
            Debug.Log("Muzzle Not Found");
        }
    }
    protected override void WeaponSpineControl(bool _b_EnemyFired, bool _b_EnemyReload)
    {
        if (b_IsSearch == true)
        {
            if (!_b_EnemyFired && Target.GetComponent<CharacterGeneral>().n_hp > 0)
            {
                FireBullet();
                spine_GunAnim.state.SetAnimation(0, "Shoot", false);
            }
        }
    }
    protected override void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_TargetPosition);
        }
        else
        {
            // Network player, receive data
            e_NetworkSpriteState = (SpriteState)stream.ReceiveNext();
            b_NetworkFired = (bool)stream.ReceiveNext();
            v_NetworkTargetPos = (Vector3)stream.ReceiveNext();

            f_LastNetworkDataReceivedTime = info.timestamp;
        }
    }
    protected override void OnCollisionEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            {
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, Util.F_TOWER_DAMAGE);
            }
        }
        if (col.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
    [PunRPC]
    protected override void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        GameObject bullet = Instantiate(this.g_Bullet, muzzlePos, Quaternion.identity);
        BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
        temp_bullet.bulletInfo = new GeneralInitialize.BulletParameter(gameObject.tag, cur_EnemyWeapon.f_Damage);
        temp_bullet.s_Victim = Util.S_PLAYER;
        bullet.GetComponent<Rigidbody2D>().velocity = (muzzlePos - this.g_Weapon.transform.position).normalized * this.cur_EnemyWeapon.f_BulletSpeed;
    }

}