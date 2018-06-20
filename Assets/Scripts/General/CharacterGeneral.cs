using UnityEngine;
using Spine;
using Spine.Unity;
using System.Collections;

public abstract class CharacterGeneral : Photon.MonoBehaviour
{
    public Rigidbody2D rigid;

    public float n_hp;    //캐릭터의 체력(실제 체력)
    public float f_MaxHp; //캐릭터의 최대체력
    public float f_AimDegree; //Aim 각도
    public float f_Speed; //캐릭터 무빙 스피드
    public float f_Multiple = 1.0f;
    public float f_SpritelocalScale; //캐릭터 로컬 스케일(타일과 크기 맞춤을 위함)
    public float f_WeaponlocalScale; //무기 로컬 스케일(타일과 크기 맞춤을 위함)

    public string s_tag;
    public string s_jobname;

    public Transform g_Sprite; //캐릭터 스프라이트(or spine) Transform
    public Transform g_Weapon; //무기 스프라이트(or spine) Transform

    public SpriteRenderer mySprite;

    public GameObject UI; //UI 프리팹
    public GameObject Muzzle; //총구 위치

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

    private void Update()
    {
        g_Sprite.GetComponent<SpriteRenderer>().sortingOrder = (int)transform.position.y;
        g_Weapon.gameObject.GetComponent<MeshRenderer>().sortingOrder = (int)transform.position.y + 1;
    }

    protected virtual void InitializeParam()
    {
        e_SpriteState = SpriteState.Idle; //상태 초기화(Idle)

        rigid = transform.GetComponent<Rigidbody2D>(); //강체 적용

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

        if (g_Sprite.GetComponent<SpriteRenderer>() != null)
        {
            mySprite = g_Sprite.GetComponent<SpriteRenderer>();
        }

        f_MaxHp = n_hp; //최대체력 설정
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
    
    protected virtual void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired, bool _b_Reload)
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
    protected void TestParticle()
    {

    }
    protected virtual void WeaponSpineControl(bool _b_Fired, bool _b_Reload)
    {

    }
    protected virtual void FireBullet()
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
    protected void FireAnimationNetwork(string WeaponName)
    {
        if (b_NetworkFired)
        {
            if(WeaponName == Util.S_AR_NAME) {
                spine_GunAnim.state.SetAnimation(0, Util.S_AR_NAME+"_Shoot", true);
            }
            
        }
        else
        {
            if(WeaponName == Util.S_AR_NAME) {
                spine_GunAnim.state.SetAnimation(0, Util.S_AR_NAME+"_Shoot", false);
            }
            
        }
    }
    
    

    protected IEnumerator IsDamaged()
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

    protected IEnumerator IsDamagedEnemy()
    {
        if (mySprite != null)
        {
            mySprite.color = new Color32(255, 0, 0, 255);

            yield return new WaitForSeconds(0.1f);

            mySprite.color = new Color32(255, 255, 255, 255);
        }

        else
        {
            spine_CharacterAnim.skeleton.SetColor(Color.red);

            yield return new WaitForSeconds(0.1f);

            spine_CharacterAnim.skeleton.SetColor(Color.white);
        }
        yield return null;
    }

    protected IEnumerator Weaken()
    {
        if (mySprite != null)
        {
            mySprite.color = new Color32(255, 0, 0, 255);
            f_Multiple = Util.F_DOUBLE;

            yield return new WaitForSeconds(Util.F_LASER);

            mySprite.color = new Color32(255, 255, 255, 255);
            f_Multiple = 1.0f;
        }
        else
        {
            spine_CharacterAnim.skeleton.SetColor(Color.red);
            f_Multiple = Util.F_DOUBLE;

            yield return new WaitForSeconds(Util.F_LASER);

            spine_CharacterAnim.skeleton.SetColor(Color.white);
            f_Multiple = 1.0f;
        }

        yield return null;
    }

    protected IEnumerator IsHealing()
    {
        mySprite.color = new Color32(0, 255, 0, 255);

        yield return new WaitForSeconds(0.1f);

        mySprite.color = new Color32(255, 255, 255, 255);
        yield return null;
    }

    protected IEnumerator Death_Wait_Sec(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Destroy(this.gameObject);
    }
}
