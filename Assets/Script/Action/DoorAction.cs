using UnityEngine;
using System;

[Serializable]
public class DoorAction {
    public Bot Bot;

    private SmartDoor _door;
    public SmartDoor Door { set { _door = value; } }

    public bool Handle() {
        var _door = Bot.Sensor.Door;
        if (_door != null  // near door
                && !_door.Opened){
            if (Bot._patrolStatus != "smartdoor"){
                                // smartdoor action is not triggered, trigger
                Debug.Log("Trigger smartdoor action");
                Bot._patrolStatus = "smartdoor";
                Bot.status = "open_door";
            }
            Bot.Sensor.Handle(_door);
            return true;
        }
        return false;
    }
}

