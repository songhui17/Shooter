using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Bot : Actor {

    #region Fields

    private bool _verbose = true;

    private const string idle = "idle";

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
                default:
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
                case "jump":
                    // EnterJump();
                    break;
                case "crouch":
                    EnterCrouch();
                    break;
                case "standup":
                    EnterStandup();
                    break;
                default:
                    break;
            }
        }
    }

    [SerializeField]
    private Sensor _sensor;
    public Sensor Sensor {
        get {
            if (_sensor != null) return _sensor;

            _sensor = GetComponent<Sensor>();
            if (_sensor != null) return _sensor;

            Debug.Assert(false, "Sensor is not attached");
            return null;
        }
    }

    private TaskPlanner _taskPlanner;
    public TaskPlanner TaskPlanner {
        get { return _taskPlanner ?? (_taskPlanner = GetComponent<TaskPlanner>()); }
    }

    public string _patrolStatus {
        get { return TaskPlanner._patrolStatus; }
        set { TaskPlanner._patrolStatus = value; }
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
            case "jump":
                // if (UpdateJump()){
                    // // TODO: handle next status other than idle
                    // if (_verbose)
                        // Debug.Log("jump action is done.");
                    // status = idle;
                // }
                break;
            case "crouch":
                if (UpdateCrouch()){
                    // TODO: handle next status other than idle
                    if (_verbose)
                        Debug.Log("crouch action is done.");

                    status = idle;
                }
                break;
            case "standup":
                if (UpdateStandup()){
                    // TODO: handle next status other than idle
                    if (_verbose)
                        Debug.Log("standup action is done.");

                    status = idle;
                }
                break;
            default:
                status = idle;
                break;
        }
        Profiler.EndSample();
    }
    
    void ApplyDamage(int damage_){
        Debug.Log("I got hit damage_: " + damage_);
        HP--;
        if (HP <= 0)
        {
            Destroy(gameObject);
            HP = 0;
        }
    }

    #region Action Methods

    private void EnterIdle(){
    }

    private bool UpdateIdle(){
        return true;
    }

    private bool _gotoDone = false;
    private string _gotoDoneReason = "DestinationReached";

    private AutoMotor _motor;
    public AutoMotor Motor{
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
    public Weapon Weapon { get { return _weapon; } }

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

    [SerializeField]
    private Transform _head;
    [SerializeField]
    private float _crouchHeight = 1.0f;
    [SerializeField]
    private float _crouchDuration = 1.0f;
    private float _crouchSpeed = 1.0f;

    [SerializeField]
    private Vector3 _crouchCharacterControllerCenter = new Vector3(0, -0.5f, 0);
    [SerializeField]
    private float _crouchCharacterControllerHeight = 1.0f;

    private Vector3 _standCharacterControllerCenter = Vector3.zero;
    private float _standCharacterControllerHeight = 2.0f;

    private Vector3 _crouchPosition;
    private Vector3 _standPosition;
    [HideInInspector]
    public bool _botCrouched = false;
    private void EnterCrouch(){
        if (_verbose)
            Debug.Log("EnterCrouch");

        // TODO: _botCrouched

        _crouchPosition = new Vector3(0, _crouchHeight, 0);
        _standPosition = _head.localPosition;
        _crouchSpeed = (_crouchHeight - _standPosition.y) / _crouchDuration;

        var characterController = GetComponent<CharacterController>();
        _standCharacterControllerCenter = characterController.center;
        _standCharacterControllerHeight = characterController.height;
    }

    private bool UpdateCrouch(){
        // _head.localPosition = Vector3.Lerp(
        //     _head.localPosition, _crouchPosition, Time.deltaTime * 0.5f);
        var deltaHeight = _crouchSpeed * Time.deltaTime;
        _head.localPosition = _head.localPosition + new Vector3(0, deltaHeight, 0);

        var height = _head.localPosition.y;
        if (Mathf.Abs(height - _crouchHeight) <= 0.01f || height < _crouchHeight){
            var characterController = GetComponent<CharacterController>();
            // cause OnTriggerExit && OnTriggerEnter
            characterController.height = _crouchCharacterControllerHeight;
            characterController.center = _crouchCharacterControllerCenter;
            _head.localPosition = _crouchPosition;
            _botCrouched = true;
            return true;
        }
        else
            return false;
    }

    private void EnterStandup(){
        if (_verbose)
            Debug.Log("EnterStandup");
    }

    private bool UpdateStandup(){
        // TODO: standupSpeed
        var standupSpeed = -_crouchSpeed;
        var deltaHeight = standupSpeed * Time.deltaTime;
        _head.localPosition = _head.localPosition + new Vector3(0, deltaHeight, 0);

        var height = _head.localPosition.y;
        var standHeight = _standPosition.y;
        if (Mathf.Abs(height - standHeight) <= 0.01f || height > standHeight){
            var characterController = GetComponent<CharacterController>();
            characterController.height = _standCharacterControllerHeight;
            characterController.center = _standCharacterControllerCenter;
            _head.localPosition = _standPosition;
            _botCrouched = false;
            return true;
        }
        else
            return false;
    }

    #endregion
}
