using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine;
using Spine.Unity;

public class Player : CharacterGeneral {



    
    // Use this for initialization
    void Start () {
        InitializeParam();

    }
	
	// Update is called once per frame
	void Update () {
        e_State = State.Idle;

        Vector3 v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(v_MousePos);
        CharacterMovement();
        Debug.Log(e_State);
	}

    public override void CharacterMovement()
    {
        float f_DeltaSpeed = f_Speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            e_State = State.Run;
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
}
