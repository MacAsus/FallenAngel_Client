using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour
{
    public GeneralInitialize.GunParameter bulletInfo;

    public ParticleSystem CollisionParticle;

    public string s_Victim = ""; //데미지를 입힐 상대방 태그
    public string s_Help = ""; //힐링을 입힐 상대방 태그

    void Start()
    {
        if (CollisionParticle != null)
        {
            CollisionParticle = GetComponentInChildren<ParticleSystem>();
            CollisionParticle.Stop();
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        //플레이어가 쏜 총알이 적과 충돌할 경우
        if (hit.layer == LayerMask.NameToLayer("EnemyBody") && s_Victim == Util.S_ENEMY)
        {
            if (this.gameObject.tag == "Grenade")
            {
                BulletSound.instance.Play_Sound_Explosion();
                CollisionParticle.Play();
            }
            //else
            //{
            //    BulletSound.instance.Play_Sound_Gun_Hit();
            //}
            Destroy(this.gameObject);
        }

        //적이 쏜 총알이 플레이어와 충돌할 경우
        else if (hit.layer == LayerMask.NameToLayer("PlayerBody") && s_Victim == Util.S_PLAYER)
        {
            BulletSound.instance.Play_Sound_Gun_Hit();
            Destroy(this.gameObject);
        }

        //적이 쏜 총알이 쉴드와 충돌할 경우
        else if (hit.layer == LayerMask.NameToLayer("Shield") && s_Victim == Util.S_PLAYER)
        {
            Destroy(this.gameObject);
        }

        //총알이 벽과 충돌할 경우
        else if (hit.layer == LayerMask.NameToLayer("Wall") || hit.tag == "Wall")
        {
            if (this.gameObject.tag == "Grenade")
            {
                BulletSound.instance.Play_Sound_Explosion();
                CollisionParticle.Play();
            }
            //else
            //{
            //    BulletSound.instance.Play_Sound_Gun_Hit();
            //}
            Destroy(this.gameObject);
        }
    }
}