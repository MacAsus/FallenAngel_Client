using UnityEngine;

public class Util : MonoBehaviour {

    //==========몬스터 정보==========//
    /* Tower */
    public static string S_TOWER_NAME = "Tower";
    public static float F_TOWER_DAMAGE = 10.0f;
    public static float F_TOWER_SEARCH = 5.0f;
    public static float F_TOWER_SPEED = 0.0f;
    public static float F_TOWER_HP = 100.0f;

    /* Bruiser */
    public static string S_BRUISER_NAME = "Bruiser";
    public static float F_BRUISER_DAMAGE = 20.0f;
    public static float F_BRUISER_SEARCH = 5.0f;
    public static float F_BRUISER_SPEED = 3.0f;
    public static float F_BRUISER_HP = 150.0f;

    /* Bomber */
    public static string S_BOBMER_NAME = "Bomber";
    public static float F_BOMBER_DAMAGE = 40.0f;
    public static float F_BOMBER_SEARCH = 5.0f;
    public static float F_BOMBER_SPEED = 2.0f;
    public static float F_BOMBER_HP = 50.0f;

    /* Fallen */
    public static string S_FALLEN_NAME = "Fallen";
    public static float F_FALLEN_DAMAGE = 20.0f;
    public static float F_FALLEN_SEARCH = 6.5f;
    public static float F_FALLEN_SPEED = 3.5f;
    public static float F_FALLEN_HP = 1000.0f;

    //==========무기 및 탄약 정보==========//
    /* Ar */
    public static string S_AR_NAME = "Ar";
    public static string S_AR_BULLET_NAME = "Ar_Norm";
    public static float F_AR_BULLET_SPEED = 20.0f;
    public static float F_AR_BULLET_DAMAGE = 3.0f;
    public static float F_AR_MAGAZINE = 30.0f;

    /* Hg */
    public static string S_HG_NAME = "Hg";
    public static string S_HG_BULLET_NAME = "Hg_Norm";
    public static float F_HG_BULLET_SPEED = 5.0f;
    public static float F_HG_BULLET_DAMAGE = 5.0f;
    public static float F_HG_MAGAZINE = 7.0f;

    //==========태그==========//
    public static string S_PLAYER = "Player";
    public static string S_ENEMY = "Enemy";
    public static string S_PLAYER_BULLET = "PlayerBullet";
    public static string S_ENEMY_BULLET = "EnemyBullet";

    //==========이름==========//
    public static string S_ATTACKER = "Attacker";
    public static string S_TANKER = "Tanker";
    public static string S_HEALER = "Healer";
    public static string S_HEAVY = "Heavy";

    //==========기타==========//
    public static Vector3 V_ACCRUATE = new Vector3(0, 0.5f, 0);

    //==========함수==========//

}
