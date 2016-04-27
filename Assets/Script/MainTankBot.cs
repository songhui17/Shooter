using UnityEngine;

public class MainTankBot : Actor {

    void ApplyDamage(int damage_){
        Debug.Log("I got hit damage_: " + damage_);
        HP--;
        if (HP <= 0)
        {
            Destroy(gameObject);
            HP = 0;
        }
    }
}
