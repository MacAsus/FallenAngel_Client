using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Bruiser : CharacterGeneral
{

    public float f_Distance; //Player와 Bruiser와의 거리

    public bool b_IsSearch = false; //Player 탐색 여부

    public GameObject BruiserPrefab; //브루저 프리팹
    public GameObject Target; //공격할 타겟
    public GameObject EnemyUiPrefab; //적 UI 프리팹

    Vector3 v_TargetPosition = new Vector3(); //타겟 위치 Vector3 값
    Vector3 v_Accurate = new Vector3(0, 0.5f, 0);

    //Photon Value
    Vector3 v_NetworkTargetPos;
    SpriteState e_NetworkSpriteState;
    double f_LastNetworkDataReceivedTime;

    // Use this for initialization
    void Start()
    {
        n_hp = 150f;
        f_Speed = 3f;
        rigid = this.gameObject.GetComponent<Rigidbody2D>();
        Target = GameObject.FindWithTag("Player");
        InitializeParam();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.isMine == true)
        {
            if (e_SpriteState != SpriteState.Dead)
            {
                e_SpriteState = SpriteState.Idle;

                if (Target)
                {
                    v_TargetPosition = Target.transform.position + v_Accurate;
                }
                // below value should be setted by manually

                //스파인 애니메이션, 총알의 발사 모두 처리하는 함수
                UpdateAnimationControl(e_SpriteState);
            }
        }
        else
        {
            UpdateNetworkAnimationControl();
        }

        Search(); // Find Nearest Other Players
        RotateGun(Target.transform.position + v_Accurate);
        Trace();
    }

    void UpdateNetworkAnimationControl()
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

    //애니메이션 컨트롤
    void UpdateAnimationControl(SpriteState _e_SpriteState)
    {
        if (_e_SpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_e_SpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Debug.Log("SerializeState Called");
        if (stream.isWriting)
        {
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_TargetPosition);
        }
        else
        {
            // Network player, receive data
            e_NetworkSpriteState = (SpriteState)stream.ReceiveNext();
            v_NetworkTargetPos = (Vector3)stream.ReceiveNext();

            f_LastNetworkDataReceivedTime = info.timestamp;
        }
    }

    protected override void Search()
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0)
        {
            Debug.Log("NetworkUtil.PlayerList count " + NetworkUtil.PlayerList.Count);
            foreach (GameObject player in NetworkUtil.PlayerList)
            {
                Vector3 playerPos = player.transform.position;

                float playerToTowerDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToTowerDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if (playerToTowerDist < minDistToTowerDist)
                {
                    distance = playerPos;
                    f_Distance = playerToTowerDist;
                    Target = player;
                }
            }

            if (Target != null)
            {
                Debug.Log("Player pos: " + Target.transform.position.x + " : " + Target.transform.position.y);
            }

            Debug.Log("f_Distance is: " + f_Distance);

            if (f_Distance <= 5)
            { // 거리가 5보다 가까운 플레이어가 있으면
                b_IsSearch = true;
            }
            else
            { // 거리가 5보다 가까운 플레이어가 없으면
                b_IsSearch = false;
            }
        }
    }
    
    void Trace()
    {
        //살아있는 Player 탐색 성공 시 Run 애니메이션 실행
        if (b_IsSearch == true && Target.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            rigid.velocity = ((Target.transform.position + v_Accurate) - this.transform.position).normalized * (base.f_Speed);
            a_Animator.SetBool("Run", true);
        }
        //탐색이 되지 않았을 시 Run 애니메이션을 끔
        else
        {
            rigid.velocity = Vector3.zero;
            a_Animator.SetBool("Run", false);
        }
    }
    
    protected override void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;

        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp > 0)
        {
            Debug.Log("===============충돌!!!=========");
            bool IsMine = hit.GetComponent<CharacterGeneral>().photonView.isMine;
            if (IsMine)
            { // 자기가 맞았을 경우에만 다른 클라이언트에게 "나 맞았다" RPC 호출
                hit.GetComponent<PhotonView>().RPC("PlayerTakeDamage", PhotonTargets.All, 20.0f); //Bruiser에 부딪힐 경우 20의 Damage를 입습니다.
            }
        }
        if (col.tag == "Player" && hit.GetComponent<CharacterGeneral>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Player is dead.");
            hit.GetComponent<CharacterGeneral>().e_SpriteState = CharacterGeneral.SpriteState.Dead;
        }
    }
}