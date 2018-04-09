using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;


[RequireComponent(typeof(CharacterGeneral))]
public class WeaponGeneral : MonoBehaviour {

    private GeneralInitialize.GunParameter cur_weapon;

    public Player ParentPlayer;

    public GameObject g_Muzzle;
	// Use this for initialization
	void Start () {
        ParentPlayer = GetComponent<Player>();

	}
	
	// Update is called once per frame
	void Update () {
        if (cur_weapon != ParentPlayer.cur_Weapon)
        {
            cur_weapon = ParentPlayer.cur_Weapon;

            if (cur_weapon.s_GunName.Contains("Hg"))
            {
                g_Muzzle = GameObject.Find(gameObject.name + "/Hg_Muzzle");
            }else if(cur_weapon.s_GunName.Contains("Ar"))
            {
                g_Muzzle = GameObject.Find(gameObject.name + "Ar_Muzzle");
            }
        }
    }
    public Vector3 getMuzzlePos() {
        return g_Muzzle.transform.position;
    }

    public Vector3 getBulletSpeed() {
        return (g_Muzzle.transform.position - ParentPlayer.g_Weapon.transform.position).normalized * ParentPlayer.cur_Weapon.f_BulletSpeed;
    }

    public GameObject FireBullet()
    {
        //총구(날아가는 방향, 총알이 나오는 포지션) 총구 - 팔의 중심 = 방향 * 속도
        GameObject bullet = Instantiate(ParentPlayer.g_Bullet,g_Muzzle.transform.position, Quaternion.identity);
        bullet.GetComponent<Rigidbody2D>().velocity = (g_Muzzle.transform.position - ParentPlayer.g_Weapon.transform.position).normalized * ParentPlayer.cur_Weapon.f_BulletSpeed;
        return bullet;
        //bullet.transform.Translate((g_Muzzle.transform.position - ParentPlayer.g_Weapon.transform.position).normalized * f_BulletSpeed);
    }
    
}
