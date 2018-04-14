using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public abstract class EnemyGeneral : Photon.MonoBehaviour
{
    public float f_Enemy_HP; //적 체력
    public float f_EnemyMovingSpeed; //적 무빙 스피드
    public float f_EnemySpriteLocalScale; //적 로컬 스케일
    public float f_EnemyWeaponLocalScale; //적 무기 로컬 스케일
    public float f_EnemyAimDegree; //Aim 각도

    public Transform EnemySprite; //적 스프라이트
    public Transform EnemyWeapon; //적 무기 스프라이트

    public GameObject EnemyBullet; //적 총알 Prefab

    public Rigidbody2D EnemyRigidBody; //적 강체

    public Animator EnemyAnimator; //적 애니메이터
    public SkeletonAnimation Spine_EnemyAnim;
    public SkeletonAnimation Spine_EnemyWeaponAnim;

    public enum EnemySpriteState { Idle, Run, Dead }; //적 상태
    public EnemySpriteState e_EnemySpriteState; //애니메이터에게 보내줄 적 상태

    public bool b_EnemyFired = false; //애니메이션 Shoot 컨트롤러(Event "Start" 와 "End" 컨트롤)
    public bool b_EnemyReload = false;

    protected virtual void InitializeParam()
    {
        EnemyBullet = Resources.Load("Bullet") as GameObject; //Resources 폴더 내의 Bullet을 GameObject로 가져옵니다.
        e_EnemySpriteState = EnemySpriteState.Idle; //초기 적 상태는 Idle
        EnemyRigidBody = transform.GetComponent<Rigidbody2D>(); //적에게 강체를 부여합니다.

        //규칙 : 적 스프라이트 or 스파인 정보는 EnemySprite에 저장합니다.
        EnemySprite = transform.Find("Sprite");
        if (transform.Find("Weapon") != null)
        {
            EnemyWeapon = transform.Find("Weapon");
        }
        f_EnemySpriteLocalScale = EnemySprite.localScale.x;
        f_EnemyWeaponLocalScale = EnemySprite.localScale.y;

        //적 애니메이터 결정
        if (EnemySprite.GetComponent<Animator>() != null)
        {
            EnemyAnimator = EnemySprite.GetComponent<Animator>();
        }
        else if (EnemySprite.GetComponent<SkeletonAnimation>() != null)
        {
            Spine_EnemyAnim = EnemySprite.GetComponent<SkeletonAnimation>();
        }

        if (EnemyWeapon != null && EnemyWeapon.GetComponent<SkeletonAnimation>() != null)
        {
            Spine_EnemyWeaponAnim = EnemyWeapon.GetComponent<SkeletonAnimation>();
            Spine_EnemyWeaponAnim.state.Event += SpineOnEvent;
        }
    }

    public virtual void SpineOnEvent(TrackEntry trackIndex, Spine.Event e)
    {

    }

    public void GetAimDegree(Vector3 v_TargetPos)
    {

        float x = EnemyWeapon.position.x - v_TargetPos.x;
        float y = EnemyWeapon.position.y - v_TargetPos.y;


        f_EnemyAimDegree = Mathf.Atan2(y, x) * Mathf.Rad2Deg;
    }

    protected virtual void UpdatePosition()
    {

    }

    protected virtual void RotateGun(Vector3 v_TargetPos)
    {

        GetAimDegree(v_TargetPos);
        EnemyWeapon.rotation = Quaternion.Euler(new Vector3(0, 0, f_EnemyAimDegree));

        if (f_EnemyAimDegree > -90 && f_EnemyAimDegree <= 90)
        {
            EnemySprite.localScale = new Vector3(f_EnemySpriteLocalScale, EnemySprite.localScale.y, EnemySprite.localScale.z);
            EnemyWeapon.localScale = new Vector3(EnemyWeapon.localScale.x, f_EnemyWeaponLocalScale, EnemyWeapon.localScale.z);
        }
        else
        {
            EnemySprite.localScale = new Vector3(-f_EnemySpriteLocalScale, EnemySprite.localScale.y, EnemySprite.localScale.z);
            EnemyWeapon.localScale = new Vector3(EnemyWeapon.localScale.x, -f_EnemyWeaponLocalScale, EnemyWeapon.localScale.z);
        }
    }

    protected virtual void EnemyAttack()
    {

    }

    protected virtual void EnemyDead()
    {

    }

    //Sprite 애니메이션 컨트롤
    protected virtual void UpdateAnimationControl(EnemySpriteState _e_EnemySpriteState, bool _b_EnemyFired)
    {

    }

    //Spine 애니메이션 없으면 Updata에 넣을 필요 없음
    protected virtual void WeaponSpineControl(bool _b_EnemyFired)
    {

    }

    protected virtual void FireBullet()
    {

    }

    //Player와의 직접적인 충돌 처리
    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("======Player가 Enemy와 충돌!!!======");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            { // 자기가 맞았을 경우에만 다른 클라이언트에게 "나 맞았다" RPC 호출
                hit.GetComponent<PhotonView>().RPC("TakeDamage", PhotonTargets.All, 5);
            }
        }
        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Player is dead.");
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
    //Player 탐색 
    protected virtual void Search()
    {

    }
}