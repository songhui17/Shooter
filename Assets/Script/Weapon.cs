using UnityEngine;

public class Weapon : MonoBehaviour {
    public float AttackRange = 2;

    public virtual void Attack(){
    }

    public virtual bool CanHit(GameObject target_){
        return false;
    }
}
