using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Player : CharacterGeneral
{
    public static GameObject LocalPlayerInstance;

    public int n_Magazine = 10;
    Vector3 v_MousePos = new Vector3();

    // Use this for initialization
    void Start()
    {
        PhotonNetwork.sendRate = 500 / Launcher.MaxPlayersPerRoom;
        PhotonNetwork.sendRateOnSerialize = 500 / Launcher.MaxPlayersPerRoom;
        InitializeParam();
    }

    // Update is called once per frame
    void Update()
    {
        // if this view is not mine, then do not update
        if (photonView.isMine == false && PhotonNetwork.connected == true)
        {
            return;
        }


        if (e_SpriteState != SpriteState.Dead)
        {
            e_SpriteState = SpriteState.Idle;

            v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // movement synced by Photon View
            CharacterMovement();

            // below value should be setted by manually
            AnimationControl(e_SpriteState, b_Fired, false);
            RotateGun(v_MousePos);
            FireBullet();
        }

    }

    protected override void CharacterMovement()
    {
        int tempx = 0;
        int tempy = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {

            if (Input.GetKey(KeyCode.A))
            {
                tempx -= 1;
            }
            if(Input.GetKey(KeyCode.D))
            {
                tempx += 1;
            }
            if (Input.GetKey(KeyCode.W))
            {
                tempy += 1;
            }
            if (Input.GetKey(KeyCode.S))
            {
                tempy -= 1;
            }


        }
        rigid.velocity = new Vector2(f_Speed * tempx, f_Speed * tempy);

    }

    public override void SpineOnevent(TrackEntry trackIndex, Spine.Event e)
    {

        if (e.Data.name == "Shoot_Start")
        {


            b_Fired = true;


        }
        else if (e.data.name == "Shoot_End")
        {

            b_Fired = false;
        }

    }

    protected override void AnimationControl(SpriteState _e_SpriteState, bool _b_Fired, bool networking)
    {
        WeaponSpineControl(_b_Fired, networking);
        if (_e_SpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_e_SpriteState == SpriteState.Run)
        {
            Debug.Log("==== Run ===");
            a_Animator.SetBool("Run", true);
        }

    }
    protected override void FireBullet()
    {


    }

    protected override void WeaponSpineControl(bool _b_Fired, bool networking)
    {
        if (!networking && Input.GetKey(KeyCode.Mouse0) && !_b_Fired) // triggerd by local
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }
        else // triggerd by network
        { 
            bool isNowFiring = this.b_Fired;
            this.b_Fired = _b_Fired;
            if (!isNowFiring && _b_Fired)
            {
                spine_GunAnim.state.SetAnimation(0, "Shoot", false);
            }
        }
    }

    void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            stream.SendNext(e_SpriteState);
            stream.SendNext(b_Fired);
            stream.SendNext(v_MousePos);
        }
        else
        {
            // Network player, receive data
            SpriteState _e_SpriteState = (SpriteState)stream.ReceiveNext();
            bool _b_Fired = (bool)stream.ReceiveNext();
            Vector3 _v_MousePos = (Vector3)stream.ReceiveNext();
            
            // below value should be setted by manually
            AnimationControl(_e_SpriteState, _b_Fired, false);
            RotateGun(_v_MousePos);
            FireBullet();
        }
    }

    void fireControl(bool _fired, Vector3 _v_MousePos)
    {
        bool isNowFiring = this.b_Fired;
        this.b_Fired = _fired;
        if (!isNowFiring && this.b_Fired)
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }
        this.v_MousePos = _v_MousePos;
        RotateGun(v_MousePos);
    }

    void moveControl(SpriteState _spriteState)
    {
        if (_spriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if (_spriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }
}
