using UnityEngine;

public class InspectAction {
    public Bot Bot;
    public Transform transform { get { return Bot.transform; } }

    public PatrolTask patrolTask;

    public bool Handle() {
        if (Bot._patrolStatus != "inspecting")
            Bot._patrolStatus = "inspecting";

        if (Bot.status == "idle"){
            var _gotoTargetPosition = patrolTask.NextPatrolPoint;
            Bot.TakeGotoAction(
                _gotoTargetPosition, 1.0f,
                _gotoTargetPosition - transform.position, 1.0f, true);
        }
        return true;
    }
}
