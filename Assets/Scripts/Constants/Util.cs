using UnityEngine;

public class Util : MonoBehaviour {

    //==========몬스터 정보==========//
    /* Tower */
    public const string S_TOWER_NAME = "Tower";
    public const float F_TOWER_DAMAGE = 10.0f;
    public const float F_TOWER_SEARCH = 5.0f;
    public const float F_TOWER_SPEED = 0.0f;
    public const float F_TOWER_HP = 15.0f;

    /* Bruiser */
    public const string S_BRUISER_NAME = "Bruiser";
    public const float F_BRUISER_DAMAGE = 20.0f;
    public const float F_BRUISER_SEARCH = 5.0f;
    public const float F_BRUISER_SPEED = 5.0f;
    public const float F_BRUISER_HP = 30.0f;

    /* Bomber */
    public const string S_BOBMER_NAME = "Bomber";
    public const float F_BOMBER_DAMAGE = 40.0f;
    public const float F_BOMBER_SEARCH = 5.0f;
    public const float F_BOMBER_SPEED = 2.0f;
    public const float F_BOMBER_HP = 50.0f;

    /* Robot */
    public const string S_ROBOT_NAME = "Robot";
    public const float F_ROBOT_DAMAGE = 30.0f;
    public const float F_ROBOT_SEARCH = 6.5f;
    public const float F_ROBOT_SPEED = 3.5f;
    public const float F_ROBOT_HP = 500.0f;

    //==========무기 및 탄약 정보==========//

    //어태커
    /* Ar */
    public const string S_AR_NAME = "Ar";
    public const string S_AR_BULLET_NAME = "Ar_Norm";
    public const float F_AR_BULLET_SPEED = 25.0f;
    public const float F_AR_BULLET_DAMAGE = 6.0f;
    public const float F_AR_MAGAZINE = 30.0f;

    /* Laser */
    public const string S_LASER_NAME = "Laser";
    public const string S_LASER_BULLET_NAME = "Laser_Norm";
    //public const float F_LASER_BULLET_SPEED = ;
    public const float F_LASER_DAMAGE = 3.0f;
    public const float F_LASER_MAGAZINE = 1.0f;

    //헤비
    /* Gatling */
    public const string S_GATLING_NAME = "Gatling";
    public const string S_GATLING_BULLET_NAME = "Gatling_Norm";
    public const float F_GATLING_BULLET_SPEED = 30.0f;
    public const float F_GATLING_BULLET_DAMAGE = 2.0f;
    public const float F_GATLING_MAGAZINE = 200.0f;

    /* Grenade */
    public const string S_GRENADE_NAME = "Grenade";
    public const string S_GRENADE_BULLET_NAME = "Grenade_Norm";
    public const float F_GRENADE_BULLET_SPEED = 10.0f;
    public const float F_GRENADE_BULLET_DAMAGE = 5.0f;
    public const float F_GRENADE_MAGAZINE = 6.0f;

    //탱커
    /* SMG */
    public const string S_SMG_NAME = "Smg";
    public const string S_SMG_BULLET_NAME = "Smg_Norm";
    public const float F_SMG_BULLET_SPEED = 20.0f;
    public const float F_SMG_BULLET_DAMAGE = 3.0f;
    public const float F_SMG_MAGAZINE = 20.0f;

    /* Shield */
    public const string S_SHIELD_NAME = "Shield";
    public const float F_SHIELD_HP = 100.0f;

    //힐러
    /* Hg */
    public const string S_HG_NAME = "Hg";
    public const string S_HG_BULLET_NAME = "Hg_Norm";
    public const float F_HG_BULLET_SPEED = 18.0f;
    public const float F_HG_BULLET_DAMAGE = 5.0f;
    public const float F_HG_MAGAZINE = 7.0f;

    /* Heal */
    public const string S_HEAL_NAME = "Pistol";
    public const string S_HEAL_BULLET_NAME = "Heal_Norm";
    public const float F_HEAL_BULLET_SPEED = 8.0f;
    public const float F_HEAL_BULLET_DAMAGE = 5.0f;
    public const float F_HEAL_MAGAZINE = 7.0f;

    public const float F_HEAL_SELF = 30.0f; //자힐

    //==========태그==========//
    public const string S_PLAYER = "Player";
    public const string S_ENEMY = "Enemy";
    public const string S_PLAYER_BULLET = "PlayerBullet";
    public const string S_ENEMY_BULLET = "EnemyBullet";

    //==========이름==========//
    public const string S_ATTACKER = "Attacker";
    public const string S_TANKER = "Tanker";
    public const string S_HEALER = "Healer";
    public const string S_HEAVY = "Heavy";

    //==========쿨타임==========//
    public const float F_LASER = 5.0f;
    public const float F_LASER_HIT_COOLTIME = 1.0f;
    public const float F_LASER_SHOOT_TIME = 2.0f;
    public const float F_SHIELD = 5.0f;
    public const float F_HEAL = 10.0f;
    public const float F_GRENADE = 10.0f;

    //==========버프 및 디버프==========//
    public const float F_BURNING = 1.0f;
    public const float F_DOUBLE = 2.0f;

    //몬스터 탄약

    //==========기타==========//
    public static Vector3 V_ACCRUATE = new Vector3(0, 0.5f, 0);

    //==========함수==========//

}
