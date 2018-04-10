using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletGeneral : MonoBehaviour {

    public GeneralInitialize.BulletParameter bulletInfo;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    void OnTriggerEnter2D(Collider2D col)
    {
        var hit = col.gameObject;
        if (col.tag == "Player" && hit.GetComponent<Player>().n_hp > 0)
        {
            hit.GetComponent<Player>().n_hp -= bulletInfo.f_Damage;
            Destroy(this.gameObject);
            Debug.Log("Bullet Collision!!!");
            Debug.Log("Collided Player HP : " + hit.GetComponent<Player>().n_hp);
        }
        if (col.tag == "Player" && hit.GetComponent<Player>().n_hp == 0)
        {
            // 캐릭터 사망
            Debug.Log("Collided Player is dead.");
        }
    }
    /*
    //화면 밖으로 나갈시 Bullet 자동삭제
    void OnBecameInvisible()
    {
        Destroy(this.gameObject);
    }
    */
}
