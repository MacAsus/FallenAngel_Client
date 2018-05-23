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

    /* Fallen */
    public const string S_FALLEN_NAME = "Fallen";
    public const float F_FALLEN_DAMAGE = 20.0f;
    public const float F_FALLEN_SEARCH = 6.5f;
    public const float F_FALLEN_SPEED = 3.5f;
    public const float F_FALLEN_HP = 1000.0f;

    //==========무기 및 탄약 정보==========//
    /* Ar */
    public const string S_AR_NAME = "Ar";
    public const string S_AR_BULLET_NAME = "Ar_Norm";
    public const float F_AR_BULLET_SPEED = 20.0f;
    public const float F_AR_BULLET_DAMAGE = 5.0f;
    public const float F_AR_MAGAZINE = 30.0f;

    /* Hg */
    public const string S_HG_NAME = "Hg";
    public const string S_HG_BULLET_NAME = "Hg_Norm";
    public const float F_HG_BULLET_SPEED = 10.0f;
    public const float F_HG_BULLET_DAMAGE = 5.0f;
    public const float F_HG_MAGAZINE = 10.0f;

    /* Gatling */
    public const string S_GATLING_NAME = "Gatling";
    public const string S_GATLING_BULLET_NAME = "Gatling_Norm";
    public const float F_GATLING_BULLET_SPEED = 30.0f;
    public const float F_GATLING_BULLET_DAMAGE = 1.0f;
    public const float F_GATLING_MAGAZINE = 200.0f;

    /* Grenade */
    public const string S_GRENADE_NAME = "Grenade";
    public const string S_GRENADE_BULLET_NAME = "Grenade_Norm";
    public const float F_GRENADE_BULLET_SPEED = 20.0f;
    public const float F_GRENADE_BULLET_DAMAGE = 3.0f;
    public const float F_GREANDE_MAGAZINE = 6.0f;

    /* SMG */
    public const string S_SMG_NAME = "Smg";
    public const string S_SMG_BULLET_NAME = "Smg_Norm";
    public const float F_SMG_BULLET_SPEED = 20.0f;
    public const float F_SMG_BULLET_DAMAGE = 3.0f;
    public const float F_SMG_MAGAZINE = 15.0f;

    /* Shield */
    public const string S_SHIELD_NAME = "Shield";
    public const float F_SHIELD_HP = 100.0f;

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

    //==========기타==========//
    public static Vector3 V_ACCRUATE = new Vector3(0, 0.5f, 0);

    //==========함수==========//

}
