using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserScript : MonoBehaviour {
    public Transform LaserBody;
    public float LaserLength = 30;
    int n_Layer;
    //public Transform DirTransform;
    //Vector3 dir;
    
	// Use this for initialization
	void Start () {
        n_Layer = LayerMask.GetMask("Wall", "EnemyBody");
	}
	
	// Update is called once per frame
	void Update () {

        RaycastHit2D hit = Physics2D.Raycast(transform.position, transform.right,LaserLength,n_Layer);
        LaserBody.localScale = new Vector3(LaserLength, LaserBody.localScale.y, LaserBody.localScale.z);
        LaserBody.localPosition = new Vector3(LaserLength/2, LaserBody.localPosition.y, LaserBody.localPosition.z);

        if (hit.collider != null)
        {

            if(hit.collider.gameObject.layer == LayerMask.NameToLayer("Wall"))
            {
                LaserBody.localScale = new Vector3(hit.distance, LaserBody.localScale.y, LaserBody.localScale.z);
                LaserBody.localPosition = new Vector3(hit.distance / 2, LaserBody.localPosition.y, LaserBody.localPosition.z);
                

            }
        }
	}
}
