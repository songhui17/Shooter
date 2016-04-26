using UnityEngine;
using System;
using System.Collections.Generic;

public class Bot : Actor {

    #region Fields

    private const string idle = "idle";

    private Task _activeTask;

    private string _status = idle;
    private string status {
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
            }
        }
    }

    #endregion

    #region Action Params 
    // TODO: abtract action

    private Vector3 _gotoTargetPosition = Vector3.zero;

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
                    // Debug.Log("shoot action is done.");
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

    private string _patrolStatus = "inspecting";
    private GameObject _attackTarget = null;
    private bool StartPatrolTask(Task task_){
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            // not important
            _patrolStatus = "inspecting";
            return true;
        }else{
            return false;
        }
    }

    private bool TickPatrolTask(Task task_){
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            if (_attackTarget == null){
                var detectRadius = 4.0f;
                var colliders = Physics.OverlapSphere(
                        transform.position, detectRadius);
                var targetColliders = new List<Collider>();
                _attackTarget = null;
                for (int i = 0; i < colliders.Length; i++){
                    var collider = colliders[i];
                    if (collider.gameObject != gameObject &&
                            collider.tag == "Player"){
                        targetColliders.Add(collider);
                        _attackTarget = collider.gameObject;
                        break;
                    }
                }
            }

            var detectEnemy = UnityEngine.Random.value > 0.5f;
            detectEnemy = _attackTarget != null; 
            
            if (detectEnemy){
                if (_patrolStatus == "inspecting"){
                    patrolTask.Interrupt();
                }
                _patrolStatus = "attacking";
                Debug.Log("_patrolStatus: " + _patrolStatus);
            }

            if (detectEnemy){
                var pos2Target = _attackTarget.transform.position - 
                    transform.position;
                var sqrAttackRange = 4;
                if (sqrAttackRange < pos2Target.sqrMagnitude){
                    // interrupt the current action
                    // and trigger goto action
                    if (status != idle){
                        _gotoTargetPosition = _attackTarget.transform.position;
                        status = "goto";
                    }
                }else{
                    if (status != "shoot"){
                        status = "shoot";
                    }
                }
            }else{
                // do nothing but set status
                _patrolStatus = "inspecting";
                if (status == idle){
                    _gotoTargetPosition = patrolTask.NextPatrolPoint;
                    status = "goto";
                }
            }

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
            return idle; // use Tick...Task  // return TakeNextAction();
        }

        if (StartFarmTask(task_)){
            return idle; // use Tick...Task  // return TakeNextAction();
        }

        if (StartPatrolTask(task_)){
            return idle; // use Tick...Task  // return TakeNextAction();
        }

        OnTaskDone();
        return idle;
    }
    
    private string TakeNextAction(){
        var gotoTask = _activeTask as GotoTask;
        if (gotoTask != null){
            _gotoTargetPosition = gotoTask.TargetPosition;
            return "goto";
        }

        var patrolTask = _activeTask as PatrolTask;
        if (patrolTask != null){
            switch (_patrolStatus){
                case "inspecting":
                    _gotoTargetPosition = patrolTask.NextPatrolPoint; 
                    return "goto";
                case "attacking":
                    // TODO: more logic
                    return "shoot";
                default:
                    throw new NotImplementedException();
            }
        }

        throw new NotImplementedException();
    }

    private bool HasNextAction(){
        var gotoTask = _activeTask as GotoTask;
        if (gotoTask != null){
            return !gotoTask.IsSatisfied();
        }

        var patrolTask = _activeTask as PatrolTask;
        if (patrolTask != null){
            return !patrolTask.IsSatisfied();
        }

        throw new NotImplementedException();
    }

    // private string OnActionDone(){
        // if (HasNextAction()){
            // return TakeNextAction();
        // }else{
            // OnTaskDone();
            // return idle;
        // }
    // }

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
        _gotoDone = false;
        Motor.MovementDone += OnMovementDone;
        Motor.SetDestination(_gotoTargetPosition);
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
    private float _shootDuration = 2;
    [SerializeField]
    private Weapon _weapon;

    private bool EnterShoot(){
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

    #endregion
}
