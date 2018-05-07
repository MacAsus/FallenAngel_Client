using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour
{
    public GeneralInitialize.GunParameter bulletInfo;

    public string s_Victim = "";

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            { // 자기가 맞았을 경우에만 다른 클라이언트에게 "나 맞았다" RPC 호출
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, bulletInfo.f_BulletDamage);
            }
            BulletSound.instance.Play_Sound_Gun_Hit();
            Destroy(this.gameObject);
        }
        if (col.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
}