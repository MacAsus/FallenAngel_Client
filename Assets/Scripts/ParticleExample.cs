using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleExample : MonoBehaviour {

    public ParticleSystem particle;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        
	}

    void StartParticle()
    {
        particle.Play();
    }
}
