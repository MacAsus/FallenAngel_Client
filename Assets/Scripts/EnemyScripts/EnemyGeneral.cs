using UnityEngine;
using System.Collections;

public class EnemyGeneral : CharacterGeneral
{
    public float f_Distance;
    public float f_Damage;
    public float delayTimer = 0f;

    public float shootDelayTime; //Enemy 총알 생성 속도

    public bool b_IsSearch = false;


    public ParticleSystem ps;
    public GameObject Target;
    public GameObject Bullet;

    public GeneralInitialize.GunParameter EnemyWeapon;

    protected Vector3 v_TargetPosition;

    //Photon Value
    protected Vector3 v_NetworkTargetPos;

    protected virtual void Search(float dis)
    {

    }
    protected virtual void Trace()
    {
        
    }

    void Start()
    {
        ps = GetComponentInChildren<ParticleSystem>();
        ps.Stop();
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        //총알과 충돌
        if (hit.layer == LayerMask.NameToLayer("Bullet"))
        {
            //데미지
            if (hit.GetComponent<BulletGeneral>().s_Victim == Util.S_ENEMY)
            {
                //헤비의 유탄에 맞을 경우
                if (hit.tag == Util.S_GRENADE_NAME)
                {
                    bool IsMine = gameObject.GetComponentInParent<CharacterGeneral>().photonView.isMine;
                    if (IsMine)
                    {
                        EnemySound.instance.Play_Sound_Gun_Hit();
                        gameObject.GetComponent<PhotonView>().RPC("TakeDamageGrenade", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                    }
                }

                //어태커의 레이저에 맞을 경우
                else if (hit.tag == Util.S_LASER_NAME)
                {
                    bool IsMine = gameObject.GetComponentInParent<CharacterGeneral>().photonView.isMine;
                    if (IsMine)
                    {
                        EnemySound.instance.Play_Sound_Gun_Hit();
                        gameObject.GetComponent<PhotonView>().RPC("TakeDamageLaser", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                    }
                }

                //그외 나머지(일반)
                else
                {
                    bool IsMine = gameObject.GetComponentInParent<CharacterGeneral>().photonView.isMine;
                    if (IsMine)
                    {
                        EnemySound.instance.Play_Sound_Gun_Hit();
                        gameObject.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, col.gameObject.GetComponent<BulletGeneral>().bulletInfo.f_BulletDamage);
                    }
                }
            }
        }
    }

    [PunRPC]
    protected void TakeDamage(float _f_Damage)
    {
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            this.n_hp -= _f_Damage;
            StartCoroutine("IsDamagedEnemy");
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            this.transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            this.transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
            ps.Play();
            EnemySound.instance.Play_Sound_Explosion();

            StartCoroutine(Death_Wait_Sec(0.5f));
        }
    }

    [PunRPC]
    protected void TakeDamageLaser(float _f_Damage)
    {
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            this.n_hp -= _f_Damage;
            StartCoroutine("IsDamagedEnemy");
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            this.transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            this.transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
            ps.Play();
            EnemySound.instance.Play_Sound_Explosion();

            StartCoroutine(Death_Wait_Sec(0.5f));
        }
    }

    [PunRPC]
    protected void TakeDamageGrenade(float _f_Damage)
    {
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            this.n_hp -= _f_Damage;
            StartCoroutine("IsDamagedEnemy");
            StartCoroutine("Burning");
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            this.transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            this.transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
            ps.Play();
            EnemySound.instance.Play_Sound_Explosion();

            StartCoroutine(Death_Wait_Sec(0.5f));
        }
    }

    protected IEnumerator Burning(float _f_Damage)
    {
        float Timer = 0;
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            while (Timer >= 5.0f)
            {
                Timer += Time.deltaTime;
                this.n_hp -= _f_Damage;
                StartCoroutine("IsDamagedEnemy");
                yield return new WaitForSeconds(Time.deltaTime);
            }
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            this.transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            this.transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = false;
            ps.Play();
            EnemySound.instance.Play_Sound_Explosion();

            StartCoroutine(Death_Wait_Sec(0.5f));
        }
        yield return null;
    }
}
