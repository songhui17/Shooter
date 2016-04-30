public class DoorAction {
    public Bot Bot;

    private SmartDoor _door;
    public SmartDoor Door { set { _door = value; } }

    public bool Handle() {
        if (_door != null && !_door.Opened){
            if (Bot._patrolStatus != "smartdoor"){
                // if (_verbose)
                //     Debug.Log("open_door");
                Bot._patrolStatus = "smartdoor";
                Bot.status = "open_door";
            }  
            return true;
        }
        return false;
    }
}

