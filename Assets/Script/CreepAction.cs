using UnityEngine;
using System;

[Serializable]
public class CreepAction {
    public Bot Bot;
    private Sensor Sensor {
        get { return Bot.Sensor; }
    }

    private AttackAction _attackAction;
    private IdleAction _idleAction;

    public bool Start(){
        _attackAction = new AttackAction(){
            Bot = Bot,
        };
        _idleAction = new IdleAction() {
            Bot = Bot,
        };
        Bot._patrolStatus = "idle";
        return true;
    }

    public bool Update(){
        if (Bot.status == "knife") return true;

        if (_attackAction.Handle()) return true;

        if (_idleAction.Handle()) return true;

        Debug.Assert(false, "No action is taken");
        return false;
    }
}
