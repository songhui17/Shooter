using UnityEngine;
using System;

public class BotRemote : Actor {
    public int ID;
    public Animator Animator;

    public void Explose() {
        Destroy(gameObject);
    }

    public void PlayAnimation(string clip_) {
        Animator.SetTrigger(clip_);
    }

    //TODO: copy from Bot.cs
    public event Action<Actor> BotKilled;
    void ApplyDamage(int damage_){
        Debug.Log("I got hit damage_: " + damage_);
        HP--;
        if (HP <= 0)
        {
            if (BotKilled != null) {
                BotKilled(this);
            }
            Destroy(gameObject);
            HP = 0;
        }
    }
}
