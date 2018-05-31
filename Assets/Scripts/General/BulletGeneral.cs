using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour
{
    public GeneralInitialize.GunParameter bulletInfo;

    public string s_Victim = ""; //데미지를 입힐 상대방 태그
    public string s_Help = ""; //힐링을 입힐 상대방 태그

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if(col.gameObject.layer == LayerMask.NameToLayer("EnemyBody") && s_Victim == Util.S_PLAYER)
        {
            
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("PlayerBody") && s_Victim == Util.S_ENEMY)
        {

        }
        else
        {
            Destroy(this.gameObject);
        }
    }
}