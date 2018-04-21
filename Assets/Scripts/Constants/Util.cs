using UnityEngine;

public class Util : MonoBehaviour {
    
    //몬스터 정보
    public static float f_Tower_Damage = 10.0f;
    public static float f_Tower_Search = 5.0f;
    public static float f_Tower_Speed = 0.0f;
    public static float f_Tower_Hp = 100.0f;

    public static float f_Bruiser_Damage = 20.0f;
    public static float f_Bruiser_Search = 5.0f;
    public static float f_Bruiser_Speed = 3.0f;
    public static float f_Bruiser_Hp = 150.0f;

    public static float f_Bomber_Damage = 40.0f;
    public static float f_Bomber_Search = 5.0f;
    public static float f_Bomber_Speed = 2.0f;
    public static float f_Bomber_Hp = 50.0f;

    public static float f_Fallen_Damage = 20.0f;
    public static float f_Fallen_Search = 6.5f;
    public static float f_Fallen_Speed = 3.5f;
    public static float f_Fallen_Hp = 1000.0f;

    //태그
    public static string s_Player = "Player";
    public static string s_Enemy = "Enemy";

    //이름
    public static string s_Attacker = "Attacker";
    public static string s_Tanker = "Tanker";
    public static string s_Healer = "Healer";
    public static string s_Heavy = "Heavy";

    //기타
    public static Vector3 v_Accruate = new Vector3(0, 0.5f, 0);

    //함수

}
