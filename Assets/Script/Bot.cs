using UnityEngine;
using System;
using System.Collections.Generic;

public class Sensor {
    private LayerMask _targetLayer;
    public LayerMask TargetLayer { set { _targetLayer = value;} }
   
    private float _detectRadius = 10.0f; 

    private GameObject _attackTarget = null;
    public GameObject AttackTarget { get { return _attackTarget; } }

    public Bot Bot;
    public GameObject gameObject { get { return Bot.gameObject; } }
    public Transform transform { get { return Bot.transform; } }

    public void Update(){
        if (_attackTarget == null){
            var detectRadius = _detectRadius;
            var layerMask = _targetLayer.value;

            // if (_verbose)
                // Debug.Log(string.Format(
                    // "_targetLayer.value: {0}", _targetLayer.value));

            var colliders = Physics.OverlapSphere(
                    transform.position, detectRadius, layerMask);

            var targetColliders = new List<Collider>();
            _attackTarget = null;
            for (int i = 0; i < colliders.Length; i++){
                var collider = colliders[i];
                if (collider.gameObject != gameObject){
                    targetColliders.Add(collider);
                     
                    var actor = collider.GetComponent<Actor>();
                    if (actor.IsAlive){
                        _attackTarget = collider.gameObject;
                        // TODO: validate raycast
                        Debug.Log("Detect _attackTarget: " + _attackTarget);
                        break;
                    }
                }
            }
        }
    }
}

public class PatrolAction {
}

public class AttackAction {
    private GameObject _attackTarget;
    public GameObject AttackTarget { set { _attackTarget = value; } }

    public Bot Bot;
    public Transform transform { get { return Bot.transform; } }

    private float _attackRange = 12.0f;

    public bool Handle() {
        var detectEnemy = _attackTarget != null;

        if (detectEnemy){
            var pos2Target = _attackTarget.transform.position - 
                transform.position;
            var sqrAttackRange = _attackRange * _attackRange;
            var inRange = sqrAttackRange >= pos2Target.sqrMagnitude; 

            var botRadius = 0.5f;
            // var attackAngle = 1.0f;  // TODO:
            var attackAngle = Mathf.Asin(botRadius / pos2Target.magnitude) * Mathf.Rad2Deg;
            var forward = transform.forward;
            forward.y = 0;
            var targetDirection = pos2Target;
            targetDirection.y = 0;
            var angle = Vector3.Angle(forward, targetDirection);
            var inAngle = attackAngle >= angle; 
            // if (_verbose)
            //     Debug.Log(string.Format(
            //                 "detectEnemy distance: {0}, angle: {1}, attackAngle: {2}",
            //                 pos2Target.magnitude, angle, attackAngle));

            // TODO:
            // AttackAction := Goto -> Shoot
            if (!(inRange && inAngle)){
                // interrupt the current action
                // and trigger goto action
                // ??? why cannt in status idle if (status != idle){
                    Bot.TakeGotoAction(
                        _attackTarget.transform.position, _attackRange,
                        targetDirection, 1.0f, true);
                // ??? why cannt in status idle }
            }else{
                var status = Bot.status;
                // if (_verbose){
                //     if (status != "shoot")
                //         Debug.Log("change status to shoot");
                //     else
                //         Debug.Log("status is already shoot");
                // }

                if (status != "shoot"){
                    // TODO: remove the code
                    Bot.status = "shoot";
                }
            }
        }
        return detectEnemy;
    }
} 

public class DoorAction {
    public Bot Bot;

    private SmartDoor _door;
    public SmartDoor Door { set { _door = value; } }

    public bool Handle() {
        if (_door != null){
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

public class BulletAction {
    public bool Handle() { return false; }
}

public class FoodAction {
    public bool Handle() { return false; }
}

public class InspectAction {
    public Bot Bot;
    public Transform transform { get { return Bot.transform; } }

    public PatrolTask patrolTask;

    public bool Handle() {
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

public class Bot : Actor {

    #region Fields

    private const string idle = "idle";

    private Task _activeTask;

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

                    _door = null;
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

    [SerializeField]
    private bool _verbose = true;
    // TODO: status editor view
    [SerializeField]
    public string _patrolStatus = "inspecting";

    private Sensor __sensor;
    private Sensor _sensor {
        get { return __sensor ?? (__sensor = new Sensor(){
                Bot = this,
                TargetLayer = _targetLayer
            });
        }
    }

    // [SerializeField]
    // private GameObject _attackTarget = null;
    // [SerializeField]
    // private float _detectRadius = 10.0f;

    // [SerializeField]
    // private float _attackRange = 12.0f;
    [SerializeField]
    private LayerMask _targetLayer;

    void OnDrawGizmos(){
        // if (_attackTarget != null){
        //     Gizmos.DrawLine(transform.position,
        //             _attackTarget.transform.position);
        // }

        // Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }

    // TODO: use action to handle SmartDoor
    SmartDoor _door;
    void ReceiveSmartObject(GameObject object_){
        if (_verbose)
            Debug.Log(string.Format("ReceiveSmartObject object_: {0}", object_));
        _door = object_.GetComponent<SmartDoor>();
        if (_door != null){
            // TODO: push door
            // Q: why take action on receive
        }
    }

    private bool StartPatrolTask(Task task_){
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            // not important
            _patrolStatus = "inspecting";
            _attackAction = new AttackAction(){
                Bot = this,
            };
            _doorAction = new DoorAction(){
                Bot = this,
            };
            _inspectAction = new InspectAction(){
                Bot = this,
            };
            _inspectAction.patrolTask = patrolTask;
            return true;
        }else{
            return false;
        }
    }

    private bool TickPatrolTask(Task task_){
        var patrolTask = task_ as PatrolTask;
        if (patrolTask != null){
            // Is shooting
            if (status == "shoot") return true;

            _sensor.Update();

            var _attackTarget = _sensor.AttackTarget;

            // TODO: handle _attackTarget destroyed
            var detectEnemy = _attackTarget != null;

            // change status
            if (detectEnemy){
                // only when detect enemy the first time
                if (_patrolStatus == "inspecting"){
                    patrolTask.Interrupt();
                }

                var _attackTargetActor = _attackTarget.GetComponent<Actor>();
                if (_attackTargetActor.IsAlive){
                    _patrolStatus = "attacking";
                }else{
                    detectEnemy = false;
                    _attackTarget = null;
                }
            }

            _attackAction.AttackTarget = _sensor.AttackTarget;
            if (_attackAction.Handle()){
                return true;
            }

            _doorAction.Door = _door;
            if (_doorAction.Handle()){
                return true;
            }

            // do nothing but set status
            if (_inspectAction.Handle()){
                // return;
            }

            if (patrolTask.IsSatisfied()){
                OnTaskDone();
                // TODO: handle patrol failure: bot die
            }
            return true;

/*Python

update(sensor):
    active: detect enemy
    passive: hit door, hit food -> hit smart object

-->  

detect_enemy = check_detect_enemy(sensor)
if detect_enemy:
    handle_enemy()  # attack, quite interesting part
    return

hit_door = check_hit_door(sensor)
if hit_door:
    handle_door()  # open the door
    return

find_food = check_find_food(sensor)
if find_food:
    handle_food()  # pick and eat
    return

find_bullet = check_find_bullet(sensor)
if find_bullet:
    handle_bullet  # pick
    return

(...)

goto_next_patrolpoint();

-->

if handle_enemy():
    return;

if handle_door():
    return

if handle_food():
    return

if handle_bullet():
   return

(...) :
    handle_other_actions
    make_other_desicions

handle_goto_next_patrolpoint()


Q: which is door failed to open?

**/
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

    private AttackAction _attackAction;
    private DoorAction _doorAction;
    private InspectAction _inspectAction;

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
            Debug.Log("EnterOpenDoor");
        _beginOpenDoorTime = Time.realtimeSinceStartup;
        _door.SendMessage("Trigger");
    }

    private bool UpdateOpenDoor(){
        return Time.realtimeSinceStartup - _beginOpenDoorTime > _openDoorDuration;  // animation time
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
