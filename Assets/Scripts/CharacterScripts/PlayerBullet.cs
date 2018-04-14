using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    public GeneralInitialize.BulletParameter bulletInfo;

    public string s_Victim = "";

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == s_Victim && hit.GetComponent<EnemyGeneral>().f_Enemy_HP > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<EnemyGeneral>().photonView.isMine;
            if (IsMine)
            { // 자기가 맞았을 경우에만 다른 클라이언트에게 "나 맞았다" RPC 호출
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, bulletInfo.f_Damage);
            }
            Destroy(this.gameObject);
        }
        if (col.tag == s_Victim && hit.GetComponent<EnemyGeneral>().f_Enemy_HP == 0)
        {
            //적 사망
            Debug.Log("Enemy is dead.");
            hit.GetComponent<EnemyGeneral>().e_EnemySpriteState = EnemyGeneral.EnemySpriteState.Dead;
        }
    }
    /*
    //화면 밖으로 나갈시 Bullet 자동삭제
    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
    */
}