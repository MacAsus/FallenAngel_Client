using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;


[RequireComponent(typeof(CharacterGeneral))]
public class WeaponGeneral : MonoBehaviour {

    private GeneralInitialize.GunParameter cur_weapon;

    public Player ParentPlayer;

    public string s_GunName;
    public float f_Damage;
    public float f_Magazine;
    public float f_BulletSpeed;

    public GameObject g_Muzzle;
	// Use this for initialization
	void Start () {
        ParentPlayer = GetComponent<Player>();
        cur_weapon = ParentPlayer.cur_Weapon;
        g_Muzzle = GameObject.Find("Hg_Muzzle");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    

    public void FireBullet()
    {

        //총구(날아가는 방향, 총알이 나오는 포지션) 총구 - 팔의 중심 = 방향 * 속도
        GameObject bullet = Instantiate(ParentPlayer.g_Bullet,g_Muzzle.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = (g_Muzzle.transform.position - ParentPlayer.g_Weapon.transform.position).normalized * ParentPlayer.cur_Weapon.f_BulletSpeed;
        //bullet.transform.Translate((g_Muzzle.transform.position - ParentPlayer.g_Weapon.transform.position).normalized * f_BulletSpeed);
    }
    
}
