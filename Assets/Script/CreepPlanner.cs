using UnityEngine;

public class CreepPlanner : TaskPlanner {
    private CreepAction _creepAction;

    void Awake (){
        _creepAction = new CreepAction(){
            Bot = Bot,
        };
        _creepAction.Start();
    }

    protected override void TickPlanner(){
        _creepAction.Update();
    }
}
