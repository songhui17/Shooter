using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sensor : MonoBehaviour {

    private Bot _bot;
    public Bot Bot {
        get { return _bot ?? (_bot = GetComponent<Bot>()); }
    }

    private List<GameObject> _attackTargetList;
    public List<GameObject> AttackTargetList {
        get { return _attackTargetList ?? (_attackTargetList = new List<GameObject>()); }
    }

    // SmartDoor _door;
    protected List<SmartDoor> _pendingSmartDoor = new List<SmartDoor>();
    private List<SmartDoor> _handledSmartDoor = new List<SmartDoor>();
    public SmartDoor Door {
        get {
            if (_pendingSmartDoor.Count > 0){
                return _pendingSmartDoor[0];
            }else{
                return null;
            }
        }
    }

    // SmartJumpObstacle _obstacle;
    protected List<SmartJumpObstacle> _pendingJumpObstacle = new List<SmartJumpObstacle>();
    public SmartJumpObstacle Obstacle {
        get {
            if (_pendingJumpObstacle.Count > 0){
                return _pendingJumpObstacle[0];
            }else{
                return null;
            }
        }
    }

    public void Handle(SmartJumpObstacle obstacle_){
        if (obstacle_ == null) return;
        _pendingJumpObstacle.Remove(obstacle_);
    }

    public void Handle(SmartDoor door_){
        if (door_ == null) return;
        _pendingSmartDoor.Remove(door_);
    }

}
