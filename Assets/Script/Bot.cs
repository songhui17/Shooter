using UnityEngine;
using System;
using System.Collections.Generic;

public class Bot : Actor {

    #region Fields

    private bool _verbose = true;

    private const string idle = "idle";

    private Task _activeTask;

    public string _patrolStatus;
    private PatrolAction _patrolAction;

    #endregion

    #region Properties

    [SerializeField]
    private string _status = idle;
    public string status {
        get { return _status ?? (_status = idle); }
        set {
            switch (_status){
                case "goto":
                    ExitGoto();
                    break;
            }
            _status = value;
            switch (_status){
                case "goto":
                    EnterGoto();
                    break;
                case "shoot":
                    if (!EnterShoot()){
                        status = idle;
                    }
                    break;
                case "open_door":
                    EnterOpenDoor();
                    break;
            }
        }
    }

    private Sensor _sensor;
    private Sensor Sensor {
        get { return _sensor ?? (_sensor = GetComponent<Sensor>()); }
    }

    #endregion

    #region Action Params 
    
    // TODO: abtract action
    private Vector3 _gotoTargetPosition = Vector3.zero;
    private Vector3 _gotoTargetDirection = Vector3.forward;
    private float _gotoStoppingDistance = 0.1f;
    private float _gotoStoppingAngle = 5.0f;
    private bool _gotoForceDirection = false;

    #endregion 

    void GetTask(){
        if (_activeTask == null){
            _activeTask = TaskManager.Instance.GetTask(this);
            if(_activeTask != null)
                Debug.Log(string.Format(
                    "Get _activeTask:{0}", _activeTask));
            status = StartTask(_activeTask);
        }
    }

    void Update(){
        Profiler.BeginSample("UpdateAction");
        switch (status){
            case idle:
                UpdateIdle();
                break;
            case "goto":
                if (UpdateGoto()){
                    // TODO: handle next status other than idle
                    Debug.Log("goto action is done.");
                    status = idle;
                    // status = OnActionDone();
                }
                break;
            case "shoot":
                if (UpdateShoot()){
                    // TODO: handle next status other than idle
                    if (_verbose)
                        Debug.Log("shoot action is done.");

                    status = idle;
                    // status = OnActionDone();
                }
                break;
            case "open_door":
                if (UpdateOpenDoor()){
                    // TODO: handle next status other than idle
                    if (_verbose)
                        Debug.Log("open_door action is done.");

                    status = idle;
                    // status = OnActionDone();
                }
                break;
            default:
                status = idle;
                break;
        }
        Profiler.EndSample();

        Profiler.BeginSample("TickTaskPlanner");
        // Plan to take next action or finish
        if (_activeTask != null){
            if (TickGotoTask(_activeTask)) return;
            if (TickFarmTask(_activeTask)) return;
            if (TickPatrolTask(_activeTask)) return;
        }
        Profiler.EndSample();

        // Plan to get some new task to do or continue
        Profiler.BeginSample("GetTask");
        GetTask();
        Profiler.EndSample();
    }

    #region Task Planner Methods

    private bool StartGotoTask(Task task_){
        var gotoTask = task_ as GotoTask;
        if (gotoTask != null){
            gotoTask.Actor = this;
            return true;
        }else{
            return false;
        }
    }

    private bool TickGotoTask(Task task_){
        var gotoTask = task_ as GotoTask;
        if (gotoTask != null){
            if (status != "goto"){
                _gotoTargetPosition = gotoTask.TargetPosition; 
                status = "goto";
            }

            if (gotoTask.IsSatisfied()){
                OnTaskDone();
            }
            // TODO: handle goto failure
            return true;
        }else{
            return false;
        }
    }

    // TODO: use action to handle SmartDoor
    // SmartDoor _door;
    // void ReceiveSmartObject(GameObject object_){
    //     if (_verbose)
    //         Debug.Log(string.Format("ReceiveSmartObject object_: {0}", object_));
    //     _door = object_.GetComponent<SmartDoor>();
    //     if (_door != null){
    //         // TODO: push door
    //         // Q: why take action on receive
    //     }
    // }

    private bool StartPatrolTask(Task task_){
        if (_patrolAction == null){
            _patrolAction = new PatrolAction(){
                Bot = this,
                Sensor = Sensor,
            };
        }
        if (_patrolAction == null){
            _patrolAction = new PatrolAction(){
                Bot = this,
                Sensor = Sensor,
            };
        }


        return _patrolAction.Start(task_);
    }

    private bool TickPatrolTask(Task task_){
        if (_patrolAction.Update(task_)){
            var patrolTask = task_ as PatrolTask;
            if (patrolTask.IsSatisfied()){
                OnTaskDone();
                // TODO: handle patrol failure: bot die
            }
            return true;
        }else{
            return false;
        }
    }

    private bool StartFarmTask(Task task_){
        var farmTask = task_ as FarmTask;
        if (farmTask != null){
            if (farmTask.IsSatisfied()){
                OnTaskDone();
            }
            // TODO: handle farmTask failure
            return true;
        }else{
            return false;
        }
    }

    private bool TickFarmTask(Task task_){
        return false;
    }

    private string StartTask(Task task_){
        _activeTask = task_;
        if (_activeTask == null) return idle;

        if (StartGotoTask(task_)){
            return idle; // use Tick...Task
        }

        if (StartFarmTask(task_)){
            return idle; // use Tick...Task
        }

        if (StartPatrolTask(task_)){
            return idle; // use Tick...Task
        }

        OnTaskDone();
        return idle;
    }

    private void OnTaskDone(){
        Debug.Log("There is no more action to take.");

        if (_activeTask.IsSatisfied()){
            Debug.Log(string.Format(
                    "_activeTask:{0} is satisfied.",
                    _activeTask));
            _activeTask = null;
        }else{
            Debug.Log(string.Format(
                    "_activeTask:{0} is failed" + 
                    " after actions are token.",
                    _activeTask));
        }
        status = idle;
    }

    #endregion
    
    #region Action Methods

    private void EnterIdle(){
    }

    private bool UpdateIdle(){
        return true;
    }

    private bool _gotoDone = false;
    private string _gotoDoneReason = "DestinationReached";

    private AutoMotor _motor;
    private AutoMotor Motor{
        get { return _motor ?? (_motor = GetComponent<AutoMotor>()); }
    }

    private bool EnterGoto(){
        if (Motor == null) return false;
        if (_verbose){
            var info = "EnterGoto:\n";
            info += string.Format("_gotoTargetPosition: {0}, {1}, {2}\n",
                    _gotoTargetPosition.x, _gotoTargetPosition.y, _gotoTargetPosition.z); 
            info += string.Format("_gotoTargetDirection: {0}, {1}, {2}\n",
                    _gotoTargetDirection.x, _gotoTargetDirection.y, _gotoTargetDirection.z); 
            info += "_gotoForceDirection: " + _gotoForceDirection + "\n";
            info += "_gotoStoppingDistance:" + _gotoStoppingDistance + "\n";
            info += "_gotoStoppingAngle:" + _gotoStoppingAngle + "\n";
            Debug.Log(info);
        }

        _gotoDone = false;
        Motor.MovementDone += OnMovementDone;
        if (!_gotoForceDirection)
            Motor.SetDestination(_gotoTargetPosition, _gotoStoppingDistance);
        else
            Motor.SetDestination(
                _gotoTargetPosition, _gotoTargetDirection,
                _gotoStoppingDistance, _gotoStoppingAngle);
        return true;
    }

    private bool UpdateGoto(){
        return _gotoDone;
    }

    private void ExitGoto(){
        Debug.Log("ExitGoto");
        Motor.MovementDone -= OnMovementDone;
        Motor.ClearDestination();

        if (!_gotoDone){
            _gotoDoneReason = "MotorInterrupted";
            _gotoDone = true;
        }
        // TODO: handle longlong exit
    }

    private void OnMovementDone(object reason_){
        _gotoDoneReason = reason_ as string;
        _gotoDone = true;
    }

    private float _lastShootTime = 0;
    [SerializeField]
    private float _shootDuration = 2;
    [SerializeField]
    private Weapon _weapon;

    private bool EnterShoot(){
        if (_verbose)
            Debug.Log("EnterShoot");

        if (_weapon == null) return false;
        _lastShootTime = Time.realtimeSinceStartup;
        _weapon.Attack();
        return true;
    }

    private bool UpdateShoot(){
        return Time.realtimeSinceStartup - _lastShootTime > _shootDuration;
    }

    private void ExitShoot(){
        // Do nothing
    }

    private float _beginOpenDoorTime;
    private float _openDoorDuration = 2.0f;
    private void EnterOpenDoor(){
        if (_verbose)
            Debug.Log(string.Format(
                        "EnterOpenDoor Sensor.Door:{0}", Sensor.Door));
        // TODO: handle EnterOpenDoor failed?
        // if (Sensor.Door == null) return false;
        _beginOpenDoorTime = Time.realtimeSinceStartup;
        if (Sensor.Door != null) Sensor.Door.SendMessage("Trigger");
    }

    private bool UpdateOpenDoor(){
        return Time.realtimeSinceStartup - _beginOpenDoorTime >
            _openDoorDuration;  // animation time
    }

    public bool TakeGotoAction(
            Vector3 targetPosition_, float stoppingDistance_,
            Vector3 targetDirection_, float stoppingAngle_,
            bool forceDirection_){
        _gotoTargetPosition = targetPosition_; 
        _gotoStoppingDistance = stoppingDistance_; 
        _gotoTargetDirection = targetDirection_;
        _gotoStoppingAngle = stoppingAngle_;
        _gotoForceDirection = forceDirection_;
        status = "goto";
        return true;
    }
    #endregion
}
