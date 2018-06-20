using UnityEngine;

public class Bruiser : EnemyGeneral
{

    // Use this for initialization
    void Start()
    {
        s_tag = Util.S_PLAYER;  
        n_hp = Util.F_BRUISER_HP;
        f_Speed = Util.F_BRUISER_SPEED;
        f_Damage = Util.F_BRUISER_DAMAGE;
        Target = GameObject.FindWithTag(Util.S_PLAYER);

        InitializeParam();
    }

    // Update is called once per frame
    void Update()
    {
        if (n_hp > 0)
        {
            if (Target != null)
            {
                v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
                WeaponSpineControl(b_Fired, b_Reload);
            }

            Search(Util.F_BRUISER_SEARCH);
            Trace();
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
            }
        }
    }

    protected override void Trace()
    {
        if (b_IsSearch == true && Target.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            rigid.velocity = (v_TargetPosition - transform.position).normalized * (f_Speed);
            a_Animator.SetBool("Run", true);
        }
        else
        {
            rigid.velocity = Vector3.zero;
            a_Animator.SetBool("Run", false);
        }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
}
