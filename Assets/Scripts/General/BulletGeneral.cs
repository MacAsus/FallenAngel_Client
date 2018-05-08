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

        if(col.gameObject.layer == LayerMask.NameToLayer("EnemyBody") && s_Victim == Util.S_PLAYER)
        {
        
        }
        else if(col.gameObject.layer == LayerMask.NameToLayer("PlayerBody") && s_Victim == Util.S_ENEMY)
        {

        }else
        {
            //Debug.Log("Col Tag : " + col.tag);
            BulletSound.instance.Play_Sound_Gun_Hit();
            Destroy(this.gameObject);
        }
    }
}