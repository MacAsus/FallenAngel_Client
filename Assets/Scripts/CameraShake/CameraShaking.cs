using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShaking : MonoBehaviour {
    public static CameraShaking instance = null;
    public float shakes = 0f;
    public float shakeAmount = 0.1f;
    public float decreaseFactor = 0.5f;
    Vector3 originalPos;
    public bool b_isShaking;

	// Use this for initialization
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

	void OnEnable () {
        b_isShaking = false;
        originalPos = gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		if(b_isShaking)
        {
            if(shakes > 0)
            {
                gameObject.transform.position = Random.insideUnitSphere * shakeAmount;
                shakes -= Time.deltaTime * decreaseFactor;
            }
            else
            {
                shakes = 0f;
                gameObject.transform.position = originalPos;
                b_isShaking = false;
            }
        }
	}

    public void ShakeCamera(float shaking)
    {
        if (!b_isShaking)
        {
            originalPos = gameObject.transform.position;
        }
        shakes = shaking;
        b_isShaking = true;
    }
}
