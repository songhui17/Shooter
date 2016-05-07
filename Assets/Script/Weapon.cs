using UnityEngine;

public class Weapon : MonoBehaviour {
    public float AttackRange = 2;

    public virtual bool Attack(){
        return true;
    }

    public virtual bool Reload(){
        return true;
    }

    public virtual bool CanHit(GameObject target_){
        return false;
    }
}
