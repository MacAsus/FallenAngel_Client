using UnityEngine;

public class Bruiser : EnemyGeneral
{

    // Use this for initialization
    void Start()
    {
        n_hp = Util.F_BRUISER_HP;
        f_Speed = Util.F_BRUISER_SPEED;
        Target = GameObject.FindWithTag(Util.S_PLAYER);
        s_tag = Util.S_PLAYER;
        InitializeParam();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                if (Target)
                {
                    v_TargetPosition = Target.transform.position + Util.V_ACCRUATE;
                }

                UpdateAnimationControl(e_SpriteState, b_Fired, b_Reload);
                //RotateGun(v_TargetPosition);
            }
        }
        else
        {
            UpdateNetworkAnimationControl();
        }

        Search(Util.F_BRUISER_SEARCH);
        Trace();
    }

    protected override void OnCollisionEnter2D(Collision2D col)
    {
        var hit = col.gameObject;

        if (col.collider.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            {
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, Util.F_BRUISER_DAMAGE);
            }
        }
        if (col.collider.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
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
