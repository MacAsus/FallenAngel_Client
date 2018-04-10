using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public abstract class CharacterGeneral : Photon.MonoBehaviour
{

    public float n_hp;    //캐릭터의 체력

    public Rigidbody2D rigid;
    public float f_AimDegree; //Aim 각도
    public float f_Speed; //캐릭터 무빙 스피드

    public string s_Weapon; //가지고 있는 무기의 이름
    public Transform g_Sprite; //캐릭터 스프라이트(or spine) Transform
    public Transform g_Weapon; //무기 스프라이트(or spine) Transform
    public GameObject g_Bullet; //총알 prefab (나중에 xml 파싱을 이용하도록 한다)

    public float f_SpritelocalScale; //캐릭터 로컬 스케일(타일과 크기 맞춤을 위함)
    public float f_WeaponlocalScale; //무기 로컬 스케일(타일과 크기 맞춤을 위함)

    public Animator a_Animator; //애니메이터
    public SkeletonAnimation spine_CharacterAnim; //(캐릭터 애니메이션이 spine일 때) 애니메이터
    public SkeletonAnimation spine_GunAnim; //(총의 애니메이션이 spine일 때) 애니메이터

    public enum SpriteState { Idle, Run, Dead }; //캐릭터의 상태 enum

    public bool b_Fired = false; //애니메이션 Shoot 컨트롤러(Event "Start" 와 "End" 컨트롤)
    public bool b_Reload = false;
    public SpriteState e_SpriteState; //애니메이터에게 보내줄 상태(캐릭터의 상태)


    protected virtual void InitializeParam()
    {
        g_Bullet = Resources.Load("Bullet") as GameObject;
        e_SpriteState = SpriteState.Idle;
        rigid = transform.GetComponent<Rigidbody2D>();
        //규칙 : 캐릭터의 스프라이트 or 스파인 정보는 g_Sprite에 저장
        g_Sprite = transform.Find("Sprite");
        if (transform.Find("Weapon") != null)
        {
            g_Weapon = transform.Find("Weapon");
        }
        f_SpritelocalScale = g_Sprite.localScale.x;
        f_WeaponlocalScale = g_Sprite.localScale.y;

        //애니메이터 결정
        if (g_Sprite.GetComponent<Animator>() != null)
        {
            a_Animator = g_Sprite.GetComponent<Animator>();
        }
        else if (g_Sprite.GetComponent<SkeletonAnimation>() != null)
        {
            spine_CharacterAnim = g_Sprite.GetComponent<SkeletonAnimation>();
        }

        if (g_Weapon != null && g_Weapon.GetComponent<SkeletonAnimation>() != null)
        {
            spine_GunAnim = g_Weapon.GetComponent<SkeletonAnimation>();
            spine_GunAnim.state.Event += SpineOnevent;
        }

    }

    public virtual void SpineOnevent(TrackEntry trackIndex, Spine.Event e)
    {

    }


    public void GetAimDegree(Vector3 v_TargetPos)
    {

        float x = g_Weapon.position.x - v_TargetPos.x;
        float y = g_Weapon.position.y - v_TargetPos.y;


        f_AimDegree = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }


    protected virtual void UpdatePosition()
    {

    }


    protected virtual void RotateGun(Vector3 v_TargetPos)
    {

        GetAimDegree(v_TargetPos);
        g_Weapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_AimDegree));

        if (f_AimDegree > -90 && f_AimDegree <= 90)
        {
            g_Sprite.localScale = new Vector3(f_SpritelocalScale, g_Sprite.localScale.y, g_Sprite.localScale.z);
            g_Weapon.localScale = new Vector3(g_Weapon.localScale.x, f_WeaponlocalScale, g_Weapon.localScale.z);
        }
        else
        {
            g_Sprite.localScale = new Vector3(-f_SpritelocalScale, g_Sprite.localScale.y, g_Sprite.localScale.z);
            g_Weapon.localScale = new Vector3(g_Weapon.localScale.x, -f_WeaponlocalScale, g_Weapon.localScale.z);
        }
    }

    protected virtual void CharacterAttack()
    {

    }

    protected virtual void CharacterDead()
    {

    }


    //Sprite 애니메이션 컨트롤
    protected virtual void UpdateAnimationControl(SpriteState _e_SpriteState, bool _b_Fired)
    {

    }

    //Spine 애니메이션 없으면 Updata에 넣을 필요 없음
    protected virtual void WeaponSpineControl(bool _b_Fired)
    {

    }

    protected virtual void FireBullet()
    {

    }

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {

    }
}
