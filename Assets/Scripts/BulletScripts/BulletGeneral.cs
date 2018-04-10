
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour
{

    public GeneralInitialize.BulletParameter bulletInfo;

    public string s_Victim = "";

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;
        if (col.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            hit.GetComponent<CharacterGeneral>().n_hp -= bulletInfo.f_Damage;
            Destroy(this.gameObject);
            Debug.Log("Bullet Collision!!!");
            Debug.Log("Collided Player HP : " + hit.GetComponent<CharacterGeneral>().n_hp);
        }
        if (col.tag == s_Victim && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Collided Player is dead.");
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
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
