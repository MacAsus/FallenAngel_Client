using UnityEngine;

public class EnemyGeneral : CharacterGeneral
{
    public float f_Distance;

    public bool b_IsSearch = false;

    public GameObject Target;
    public GameObject Bullet;

    public GeneralInitialize.GunParameter EnemyWeapon;

    protected Vector3 v_TargetPosition;

    //Photon Value
    protected Vector3 v_NetworkTargetPos;

    protected virtual void Search(float dis)
    {

    }
    protected virtual void Trace()
    {
        
    }
    protected virtual void OnCollisionEnter2D(Collider2D col)
    {

    }
}
