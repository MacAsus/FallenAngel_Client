using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Player : CharacterGeneral {
    public static GameObject LocalPlayerInstance;

    public int n_Magazine = 10;

    // Use this for initialization
    void Start () {
        InitializeParam();
    }
	
	// Update is called once per frame
	void Update () {
        // if this view is not mine, then do not update
        if(photonView.isMine == false && PhotonNetwork.connected == true){
            return;
        }


        if (e_SpriteState != SpriteState.Dead)
        {
            e_SpriteState = SpriteState.Idle;

            Vector3 v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RotateGun(v_MousePos);
            CharacterMovement();
            AnimationControl();
            FireBullet();
        }
        
	}

    public override void CharacterMovement()
    {
        float f_DeltaSpeed = f_Speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            e_SpriteState = SpriteState.Run;
            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(new Vector3(-f_DeltaSpeed, 0));
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(new Vector3(f_DeltaSpeed, 0));
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(new Vector3(0, -f_DeltaSpeed));
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(new Vector3(0, f_DeltaSpeed));
            }


        }
    }

    public override void SpineOnevent(TrackEntry trackIndex, Spine.Event e)
    {
        
        if (e.Data.name == "Shoot_Start")
        {

            
                b_Fired = true;

            
        }
        else if(e.data.name == "Shoot_End")
        {

                b_Fired = false;
        }
        
    }

    public override void AnimationControl()
    {
        if (e_SpriteState == SpriteState.Idle)
        {
            a_Animator.SetBool("Run", false);
        }
        if(e_SpriteState == SpriteState.Run)
        {
            a_Animator.SetBool("Run", true);
        }
    }
    public override void FireBullet()
    {

        if (Input.GetKey(KeyCode.Mouse0) && !b_Fired)
        {
            spine_GunAnim.state.SetAnimation(0, "Shoot", false);
        }

    }

}
