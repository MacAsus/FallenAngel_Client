using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour
{
    public GeneralInitialize.GunParameter bulletInfo;

    public string s_Victim = "";

    void OnCollisionEnter2D(Collision2D col)
    {
        var hit = col.gameObject;

        if (col.collider.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            if (s_Victim == Util.S_ENEMY)
            {
                bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
                if (IsMine)
                {
                    hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, bulletInfo.f_BulletDamage);
                }

                BulletSound.instance.Play_Sound_Gun_Hit();
                Destroy(this.gameObject);
            }

            if (s_Victim == Util.S_PLAYER)
            {
                bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
                if (IsMine)
                {
                    hit.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, bulletInfo.f_BulletDamage);
                }

                BulletSound.instance.Play_Sound_Gun_Hit();
                Destroy(this.gameObject);
            }
        }
        if (col.collider.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
}