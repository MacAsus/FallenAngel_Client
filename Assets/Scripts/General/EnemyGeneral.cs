using UnityEngine;

public class EnemyGeneral : CharacterGeneral
{
    public float f_Distance;

    public bool b_IsSearch = false;

    public GameObject Target;

    public GeneralInitialize.GunParameter cur_EnemyWeapon;

    protected Vector3 v_TargetPosition;

    //Photon Value
    protected Vector3 v_NetworkTargetPos;

    protected void Search(float dis)
    {
        Vector3 distance = new Vector3(9999, 9999);
        if (NetworkUtil.PlayerList.Count != 0)
        {
            // Debug.Log("NetworkUtil.PlayerList count " + NetworkUtil.PlayerList.Count);
            foreach (GameObject player in NetworkUtil.PlayerList)
            {
                Vector3 playerPos = player.transform.position;

                float playerToTowerDist = Vector3.Distance(playerPos, this.transform.position); // "플레이어 - 타워" 사이의 거리
                float minDistToTowerDist = Vector3.Distance(distance, this.transform.position); // "최소거리 - 타워" 사이의 거리

                // 현 플레이어 - 타워 거리보다 최소거리 - 타워거리가 더 가까우면
                if (playerToTowerDist < minDistToTowerDist)
                {
                    distance = playerPos;
                    f_Distance = playerToTowerDist;
                    Target = player;
                }
            }
            if (f_Distance <= dis)
            {
                b_IsSearch = true;
            }
            else
            {
                b_IsSearch = false;
            }
        }
    }
    protected virtual void Trace()
    {
        
    }
    protected virtual void OnCollisionEnter2D(Collider2D col)
    {

    }
}
