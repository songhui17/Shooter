using UnityEngine;
using System;

public class BotRemote : Actor {
    public Animator Animator;

    [SerializeField]
    private Weapon _weapon;
    public Weapon Weapon { get { return _weapon; } }

    public void Explose() {
        Destroy(gameObject);
    }

    public void PlayAnimation(string clip_) {
        Animator.SetTrigger(clip_);
        // TODO:
        switch (clip_) {
            case "attack":
            case "attack_left":
            case "attack_right":
                {
                    _weapon.Attack();
                }
                break;
            default:
                break;
        }
    }

    void ApplyDamage(int damage_){
        Debug.Log("I got hit damage_: " + damage_);
        HP--;
        if (HP <= 0)
        {
            OnBotKilled();
            Destroy(gameObject);
            HP = 0;
        }
    }
}
