using UnityEngine;

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

            StartCoroutine(Death_Wait_Sec(0.5f));
        }
    }
}
