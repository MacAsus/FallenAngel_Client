using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public abstract class CharacterGeneral : MonoBehaviour {

    public int n_hp;

    public float f_AimDegree;
    public float f_Speed;
    public bool b_Ranged;
    public string s_Weapon;
    public Transform g_Sprite;
    public Transform g_Weapon;
    public float f_SpritelocalScale;
    public float f_WeaponlocalScale;

    public Animator a_Animator;
    public SkeletonAnimation spine_CharacterAnim;
    public SkeletonAnimation spine_GunAnim;

    public enum SpriteState{ Idle, Run, Attack, Dead };
    public enum SpineState { Idle, Attack };
    public SpriteState e_SpriteState;

    public virtual void InitializeParam()
    {
        e_SpriteState = SpriteState.Idle;
        g_Sprite = transform.Find("Sprite");
        if (b_Ranged)
        {
            g_Weapon = transform.Find("Weapon");
        }
        f_SpritelocalScale = g_Sprite.localScale.x;
        f_WeaponlocalScale = g_Sprite.localScale.y;
        if (g_Sprite.GetComponent<Animator>() != null)
        {
            a_Animator = g_Sprite.GetComponent<Animator>();
        }
        else if (g_Sprite.GetComponent<SkeletonAnimation>() != null)
        {
            spine_CharacterAnim = g_Sprite.GetComponent<SkeletonAnimation>();
        }

        if (g_Weapon.GetComponent<SkeletonAnimation>() != null)
        {
            spine_GunAnim = g_Weapon.GetComponent<SkeletonAnimation>();
        }

    }

    public void GetAimDegree(Vector3 v_TargetPos)
    {
        
        float x = g_Weapon.position.x - v_TargetPos.x;
        float y = g_Weapon.position.y - v_TargetPos.y;


        f_AimDegree = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }
    


    public virtual void CharacterMovement()
    {

    }


    public virtual void RotateGun(Vector3 v_TargetPos)
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

    public virtual void CharacterAttack()
    {

    }

    public virtual void CharacterDead()
    {

    }


    //Sprite 애니메이션 컨트롤
    public virtual void AnimationControl()
    {

    }

    //Spine 애니메이션 없으면 Updata에 넣을 필요 없음
    public virtual void WeaponSpineControl()
    {

    }
}
