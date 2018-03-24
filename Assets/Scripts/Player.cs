using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CharacterGeneral {


    bool b_LeftorRight = false;
    bool b_UporDown = false;

    // Use this for initialization
    void Start () {
        InitializeParam();
    }
	
	// Update is called once per frame
	void Update () {

        Vector3 v_MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RotateGun(v_MousePos);
        CharacterMovement();
	}

    

    public override void CharacterMovement()
    {
        float f_DeltaSpeed = f_Speed * Time.deltaTime;
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
