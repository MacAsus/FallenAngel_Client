using UnityEngine;

public class Bruiser : EnemyGeneral {

	// Use this for initialization
	void Start () {
        n_hp = Util.f_Bruiser_Hp;
        f_Speed = Util.f_Bruiser_Speed;
        Target = GameObject.FindWithTag(Util.s_Player);
        s_tag = Util.s_Player;
        InitializeParam();
	}
	
	// Update is called once per frame
	void Update () {
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                if (Target)
                {
                    v_TargetPosition = Target.transform.position + Util.v_Accruate;
                }

                UpdateAnimationControl(e_SpriteState, b_Fired);
                RotateGun(v_TargetPosition);
            }
        }
        else
        {
            UpdateNetworkAnimationControl();
        }

        Search(Util.f_Bruiser_Search);
        Trace();
    }

    protected override void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            {
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, Util.f_Bruiser_Damage);
            }
        }
        if (col.tag == s_tag && hit.GetComponent<CharacterGeneral>().n_hp == 0)
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
}
