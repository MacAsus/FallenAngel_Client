using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections;

public abstract class CharacterGeneral : Photon.MonoBehaviour
{
    public Rigidbody2D rigid;

    public float n_hp;    //캐릭터의 체력
    public float f_AimDegree; //Aim 각도
    public float f_Speed; //캐릭터 무빙 스피드

    public float f_SpritelocalScale; //캐릭터 로컬 스케일(타일과 크기 맞춤을 위함)
    public float f_WeaponlocalScale; //무기 로컬 스케일(타일과 크기 맞춤을 위함)

    public string s_tag;
    public string s_jobname;

    public Transform g_Sprite; //캐릭터 스프라이트(or spine) Transform
    public Transform g_Weapon; //무기 스프라이트(or spine) Transform

    public SpriteRenderer mySprite;

    public GameObject UI; //UI 프리팹
    public GameObject Muzzle; //총구 위치

    public ParticleSystem Death_Particle; //사망 파티클 프리팹

    public Animator a_Animator; //애니메이터

    public SkeletonAnimation spine_CharacterAnim; //(캐릭터 애니메이션이 spine일 때) 애니메이터
    public SkeletonAnimation spine_GunAnim; //(총의 애니메이션이 spine일 때) 애니메이터

    public enum SpriteState { Idle, Run, Dead }; //캐릭터의 상태 enum
    public SpriteState e_SpriteState; //애니메이터에게 보내줄 상태(캐릭터의 상태)

    public bool b_Fired = false; //애니메이션 Shoot 컨트롤러(Event "Start" 와 "End" 컨트롤)
    public bool b_Reload = false;
    public bool b_UnHit = false;

    //Photon Value
    protected Vector3 v_NetworkPosition;
    protected SpriteState e_NetworkSpriteState;
    protected bool b_NetworkFired;
    protected double f_LastNetworkDataReceivedTime;

    protected virtual void InitializeParam()
    {
        e_SpriteState = SpriteState.Idle; //상태 초기화(Idle)

        rigid = transform.GetComponent<Rigidbody2D>(); //강체 적용

        Death_Particle = GetComponent<ParticleSystem>();

        g_Sprite = transform.Find("Sprite");
        if (transform.Find("Weapon") != null)
        {
            g_Weapon = transform.Find("Weapon");
        }

        f_SpritelocalScale = g_Sprite.localScale.x;
        f_WeaponlocalScale = g_Sprite.localScale.y;

        //캐릭터 애니메이션 적용
        if (g_Sprite.GetComponent<Animator>() != null)
        {
            a_Animator = g_Sprite.GetComponent<Animator>();
        }
        if (g_Sprite.GetComponent<SkeletonAnimation>() != null)
        {
            spine_CharacterAnim = g_Sprite.GetComponent<SkeletonAnimation>();
        }

        //무기 애니메이션 적용
        if (g_Weapon != null && g_Weapon.GetComponent<SkeletonAnimation>() != null)
        {
            spine_GunAnim = g_Weapon.GetComponent<SkeletonAnimation>();
            spine_GunAnim.state.Event += SpineOnevent;
        }

        mySprite = g_Sprite.GetComponent<SpriteRenderer>();
    }

    protected void SpineOnevent(TrackEntry trackIndex, Spine.Event e)
    {
        if (e.Data.name == "Shoot_Start")
        {
            b_Fired = true;
        }
        if (e.data.name == "Shoot_End")
        {
            b_Fired = false;
        }

        if (e.Data.name == "Reload_Start")
        {
            b_Reload = true;
        }
        if (e.data.name == "Reload_End")
        {
            b_Reload = false;
        }
    }
    
    protected void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired, bool _b_Reload)
    {
        WeaponSpineControl(_b_Fired, _b_Reload);
        if (_e_SpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_e_SpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }
    protected void UpdateNetworkAnimationControl()
    {
        if (e_NetworkSpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (e_NetworkSpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }
    protected void UpdateNetworkedPosition()
    {
        float pingInSeconds = (float)PhotonNetwork.GetPing() * 0.001f;
        float timeSinceLastUpdate = (float)(PhotonNetwork.time - f_LastNetworkDataReceivedTime);
        float totalTimePassed = pingInSeconds + timeSinceLastUpdate;
        int lerpValue = 20; // lerpValue가 높아질 수록 빠르게 따라잡음

        Vector3 newPosition = Vector3.Lerp(transform.position, v_NetworkPosition, Time.smoothDeltaTime * lerpValue); // 

        if (Vector3.Distance(transform.position, v_NetworkPosition) > 3f)
        {
            newPosition = v_NetworkPosition;
            Debug.Log("Teleport");
        }

        transform.position = newPosition;
    }

    protected virtual void WeaponSpineControl(bool _b_Fired, bool _b_Reload)
    {

    }
    protected virtual void FireBullet()
    {

    }
    protected virtual void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {

    }
    protected virtual void OnCollisionEnter2D(Collision2D col)
    {

    }
    
    [PunRPC]
    protected virtual void FireBulletNetwork(Vector3 muzzlePos, Vector3 bulletSpeed)
    {
        
    }
    [PunRPC]
    protected void FireAnimationNetwork()
    {
        if (b_NetworkFired)
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", true);
        }
        else
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
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
            StartCoroutine(Death_Wait_Sec(0.5f));
        }
    }
    [PunRPC]
    protected void PlayerTakeDamage(float _f_Damage)
    {
        if (this.n_hp > 0 && this.n_hp > _f_Damage)
        {
            this.n_hp -= _f_Damage;
            this.b_UnHit = true;
            transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = false;
            StartCoroutine("IsDamaged");
        }
        else
        {
            this.n_hp = 0;
            this.a_Animator.SetBool("Death", true);
            StartCoroutine(Death_Wait_Sec(1.0f));
        }
    }

    IEnumerator IsDamaged()
    {
        int count = 0;

        while (count < 10)
        {
            if (count % 2 == 0)
            {
                mySprite.color = new Color32(255, 255, 255, 90);
            }
            else
            {
                mySprite.color = new Color32(255, 255, 255, 180);
            }

            yield return new WaitForSeconds(0.1f);
            count++;
        }

        mySprite.color = new Color32(255, 255, 255, 255);
        b_UnHit = false;
        transform.Find("Trigger").GetComponent<BoxCollider2D>().enabled = true;
        yield return null;
    }
    IEnumerator IsDamagedEnemy()
    {
        mySprite.color = new Color32(255, 0, 0, 255);

        yield return new WaitForSeconds(0.1f);

        mySprite.color = new Color32(255, 255, 255, 255);
        yield return null;
    }
    IEnumerator Death_Wait_Sec(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (!Death_Particle.isPlaying)
        {
            Death_Particle.Play();
        }
        SoundGeneral.instance.Play_Sound_Explosion();
        //Instantiate(Death_Particle, transform.position, transform.rotation);
        Destroy(this.gameObject);
    }
}
