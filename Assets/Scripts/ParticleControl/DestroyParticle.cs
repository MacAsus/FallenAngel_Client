using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour
{

    public ParticleSystem ps;

    void Start()
    {

    }

    void Update()
    {
        /*
        if (ps.IsAlive() == false)
        {
            Destroy(gameObject);
        }
        */
    }

    public void StartParticle()
    {
        ps.Play();
        CameraShaking r = new CameraShaking();
        r.ShakeCamera(1.0f);
    }
}
