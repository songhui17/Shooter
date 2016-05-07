using UnityEngine;

public class IdleAction {
    public Bot Bot;
    public bool Handle(){
        Debug.Assert(Bot != null, "Bot is null");

        if (Bot._patrolStatus != "idle"){
            Bot._patrolStatus = "idle";
        }

        if (Bot.status != "idle"){
            Bot.status = "idle";
        }
        return true;
    }
}
