using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponChanger : MonoBehaviour
{

    public Image WeaponImg;
    string gunName;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        gunName = InGame.Player.GetComponent<Player>().cur_Weapon.s_GunName;
		Debug.Log("gunName is " + gunName);
        switch (gunName)
        {
			// 어태커
            case Util.S_AR_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			case Util.S_LASER_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			// 헤비
			case Util.S_GATLING_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			case Util.S_GRENADE_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("Military_Icon_Kit/01_GUN/gun_aks74");
                break;
			// 탱커
			case Util.S_SMG_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			case Util.S_SHIELD_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("Military_Icon_Kit/01_GUN/gun_aks74");
                break;
			// 힐러
			case Util.S_HG_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			case Util.S_HEAL_NAME:
				WeaponImg.sprite = Resources.Load<Sprite>("gun_ai_ae");
                break;
			default:
				WeaponImg.sprite = new Sprite();
				break;
        }
    }
}
