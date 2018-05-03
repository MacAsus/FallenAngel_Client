using UnityEngine;

public class Util : MonoBehaviour {
    
    //몬스터 정보
    public static float F_TOWER_DAMAGE = 10.0f;
    public static float F_TOWER_SEARCH = 5.0f;
    public static float F_TOWER_SPEED = 0.0f;
    public static float F_TOWER_HP = 100.0f;

    public static float F_BRUISER_DAMAGE = 20.0f;
    public static float F_BRUISER_SEARCH = 5.0f;
    public static float F_BRUISER_SPEED = 3.0f;
    public static float F_BRUISER_HP = 150.0f;

    public static float F_BOMBER_DAMAGE = 40.0f;
    public static float F_BOMBER_SEARCH = 5.0f;
    public static float F_BOMBER_SPEED = 2.0f;
    public static float F_BOMBER_HP = 50.0f;

    public static float F_FALLEN_DAMAGE = 20.0f;
    public static float F_FALLEN_SEARCH = 6.5f;
    public static float F_FALLEN_SPEED = 3.5f;
    public static float F_FALLEN_HP = 1000.0f;

    //태그
    public static string S_PLAYER = "Player";
    public static string S_ENEMY = "Enemy";

    //이름
    public static string S_ATTACKER = "Attacker";
    public static string S_TANKER = "Tanker";
    public static string S_HEALER = "Healer";
    public static string S_HEAVY = "Heavy";

    //기타
    public static Vector3 V_ACCRUATE = new Vector3(0, 0.5f, 0);

    //함수

}
