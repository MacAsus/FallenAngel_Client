using System.Collections;
using UnityEngine;

public class Tower : EnemyGeneral
{

    void Start()
    {
        s_tag = Util.S_PLAYER;
        n_hp = Util.F_TOWER_HP;
        f_Speed = Util.F_TOWER_SPEED;
        f_Damage = Util.F_TOWER_DAMAGE;

        shootDelayTime = 0.5f;

        Target = GameObject.FindWithTag(s_tag);

        //Bullet = Resources.Load("BulletPrefab/" + Util.S__BULLET_NAME) as GameObject;
        //EnemyWeapon = new GeneralInitialize.GunParameter(Util.S__NAME, Util.S__BULLET_NAME, Util.F__BULLET_SPEED, Util.F__BULLET_DAMAGE, Util.F__MAGAZINE);

        //디버깅용
        Bullet = Resources.Load("BulletPrefab/" + Util.S_ENEMY_BULLET_NAME) as GameObject;
        EnemyWeapon = new GeneralInitialize.GunParameter(Util.S_ENEMY_NAME, Util.S_ENEMY_BULLET_NAME, Util.F_ENEMY_BULLET_SPEED, Util.F_ENEMY_BULLET_DAMAGE, Util.F_ENEMY_MAGAZINE);
        //여기까지

        InitializeParam();
    }

    void Update()
    {
        if (n_hp > 0) {
            delayTimer += Time.deltaTime;

            if (Target != null)
            {
                v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
                WeaponSpineControl(b_Fired, b_Reload);
            }

            Search(Util.F_TOWER_SEARCH);
        }
    }

    protected override void WeaponSpineControl(bool _b_EnemyFired, bool _b_EnemyReload)
    {
        if (b_IsSearch == true && delayTimer > shootDelayTime)
        {
            if (!_b_EnemyFired && Target.GetComponent<CharacterGeneral>().n_hp > 0)
            {
                FireBullet();
                EnemySound.instance.Play_Sound_Main_Shoot();
                a_Animator.SetBool("Aim", true);
                delayTimer = 0f;
            }
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        Debug.Log("SerializeState Called");
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
    
    protected override void Search(float dis)
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0)
        {
            foreach (GameObject player in NetworkUtil.PlayerList)
            {
                Vector3 playerPos;
                if (player != null)
                {
                    playerPos = player.transform.position;
                }
                else
                {
                    continue;
                }
                float playerToTowerDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToTowerDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if (playerToTowerDist < minDistToTowerDist)
                {
                    distance = playerPos;
                    f_Distance = playerToTowerDist;
                    Target = player;
                }
            }
            if (f_Distance <= dis)
            {
                b_IsSearch = true;
            }
            else
            {
                b_IsSearch = false;
                a_Animator.SetBool("Aim", false);
            }
        }
    }

    protected override void FireBullet()
    {
        if (Muzzle)
        {
            Vector3 v_muzzle = Muzzle.transform.position;
            Vector3 v_bulletSpeed = (Muzzle.transform.position - (Target.transform.position + Util.V_ACCRUATE)).normalized * Util.F_HG_BULLET_SPEED;

            this.photonView.RPC("FireBulletNetwork", PhotonTargets.All, v_muzzle, v_bulletSpeed);
            this.photonView.RPC("FireAnimationNetwork", PhotonTargets.Others);
        }
    }

    [PunRPC]
    protected override void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        GameObject bullet = Instantiate(Bullet, muzzlePos, Quaternion.identity);
        BulletGeneral temp_bullet = bullet.GetComponent<BulletGeneral>();
        temp_bullet.bulletInfo = EnemyWeapon;
        temp_bullet.s_Victim = s_tag;
        bullet.GetComponent<Rigidbody2D>().velocity = (Target.transform.position - muzzlePos).normalized * Util.F_SMG_BULLET_SPEED;
    }
    
}