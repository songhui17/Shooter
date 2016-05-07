using UnityEngine;
using System;
using System.Collections;

public class AutoMotor : MonoBehaviour {
    #region Support Auto && Manual

    private enum CONTROL_TYPE {
        Auto,
        Manual,
    }
    public bool EnableManual = false;

    private class Snapshot {
        public bool _idle = false;
        public Vector3 _targetPosition;
        public Vector3 _targetDirection;
        public bool _forceDirection;

        public override string ToString(){
            var info = "";
            info += "_idle: " + _idle + "\n";
            info += "_targetPosition: " + _targetPosition + "\n";
            info += "_targetDirection: " + _targetDirection + "\n";
            info += "_forceDirection: " + _forceDirection + "\n";
            return info;
        }
    }
    private Snapshot _lastState;

    private CONTROL_TYPE _controlType = CONTROL_TYPE.Auto;
    private CONTROL_TYPE ControlType {
        get { return _controlType; }
        set {
            if (value == _controlType) return;

            switch (value){
                case CONTROL_TYPE.Auto:
                    {
                        if (_lastState != null){
                            Debug.Log(string.Format("Set _lastState:{0}", _lastState));
                            _idle = _lastState._idle;
                            _targetPosition = _lastState._targetPosition;
                            _targetDirection = _lastState._targetDirection;
                            _forceDirection = _lastState._forceDirection;
                        }
                    }
                    break;
                case CONTROL_TYPE.Manual:
                    {
                        if (_lastState == null) _lastState = new Snapshot();
                        _lastState._idle = _idle;
                        _lastState._targetPosition = _targetPosition;
                        _lastState._targetDirection = _targetDirection;
                        _lastState._forceDirection = _forceDirection;
                    }
                    break;
                default:
                    throw new Exception("Invalid CONTROL_TYPE: " + value);
            }

            _controlType = value;
        }
    }
    #endregion

    private enum MOVEMENT_STATE {
        Walking,
        Jumping,
        Idle,
    }
    
    private MOVEMENT_STATE _movementState = MOVEMENT_STATE.Idle;
    private MOVEMENT_STATE MovementState {
        get { return _movementState; }
        set {
            _movementState = value;
        }
    }
    
    private Vector3 _jumpVelocity = Vector3.zero;

    [SerializeField]
    private Color _line2TargetPositionColor = Color.red;
    [SerializeField]
    private Color _line2TargetDirectionColor = Color.yellow;

    [SerializeField]
    private float _speed = 2.0f;
    [SerializeField]
    private float _rotateSpeed = 40.0f;

    private bool _idle = true;
    public bool IsMoving { get { return !_idle; } }
    private Vector3 _targetPosition;
    public Vector3 TargetPosition { get { return _targetPosition; } }

    private float _sqrStoppingDistance;
    private float SqrStoppingDistance {
        get { return _sqrStoppingDistance > 0 ? _sqrStoppingDistance :
                (_sqrStoppingDistance =  0.01f); }
        set { _sqrStoppingDistance = value; }
    }

    // private bool _forceStopMoving = true;

    private bool _forceDirection = false;
    private Vector3 _targetDirection;
    private float _stopingAngle;
    private float StoppingAngle {
        get { return _stopingAngle > 0 ? _stopingAngle : (_stopingAngle = 5.0f); }
        set { _stopingAngle = value; }
    }
    // TODO: wrong
    private bool _jumpDirection = true;

    [SerializeField]
    private CharacterController _controller;

    void Start(){
        _controller = GetComponent<CharacterController>();
        if (_controller == null){
            enabled = false;
            Debug.Log("There is not CharacterController attached");
        }

        _targetPosition = transform.position;
        Cursor.lockState = CursorLockMode.Locked;
    }


    void UpdateAuto(){
        if (!EnableManual) return;

        var manualMoving = Input.GetAxis("Horizontal") != 0
                           || Input.GetAxis("Vertical") != 0;
        // var manualRotating = Input.GetAxis("Mouse X") > 0
        //                      || Input.GetAxis("Mouse Y") > 0;
        if (manualMoving){
            ControlType = CONTROL_TYPE.Manual;
        }
    }

    void UpdateManual(){
        switch (MovementState){
            case MOVEMENT_STATE.Walking:
            case MOVEMENT_STATE.Idle:
                {
                    var moveX = Input.GetAxis("Horizontal");
                    var moveZ = Input.GetAxis("Vertical");
                    var velocity = new Vector3(moveX, 0, moveZ);
                    velocity = transform.TransformDirection(velocity);
                    var movement =  velocity * _speed;

                    if (velocity.sqrMagnitude > 0.01f){
                        movement *= Time.deltaTime;
                        movement.y -= 0.08f;  // down a bit so that isGround return true;
                        _controller.Move(movement);
                    }

                    var rotateY = Input.GetAxis("Mouse X");
                    // var rotateZ = Input.GetAxis("Mouse Y");
                    transform.Rotate(Vector3.up, rotateY * _rotateSpeed * Time.deltaTime, Space.World);

                    var jumpButton = Input.GetButton("Jump");
                    var jump = jumpButton || !_controller.isGrounded;
                    if (jump){
                        Debug.Log(string.Format(
                                    "Jump jumpButton:{0}, _controller.isGrounded: {1}",
                                    jumpButton, _controller.isGrounded));
                        MovementState = MOVEMENT_STATE.Jumping;
                        _jumpVelocity = velocity * _speed;
                        if (jumpButton) _jumpVelocity.y += 5.0f;
                    }
                }
                break;
            case MOVEMENT_STATE.Jumping:
                {
                    var gravity = -10.0f;
                    var movement = _jumpVelocity * Time.deltaTime;

                    _jumpVelocity.y += gravity * Time.deltaTime;
                    movement += _jumpVelocity * Time.deltaTime;
                    movement *= 0.5f;
                    _controller.Move(movement);
                    
                    if (_controller.isGrounded){
                        MovementState = MOVEMENT_STATE.Walking;
                    }
                }
                break;
            default:
                throw new Exception("Invalid MovementState: " + MovementState);
        }
    }

    void Update(){
        if (Input.GetMouseButton(0)){
            if (Cursor.lockState != CursorLockMode.Locked){
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
        switch (ControlType){
            case CONTROL_TYPE.Auto:
                UpdateAuto();
                break;
            case CONTROL_TYPE.Manual:
                UpdateManual();
                break;
            default:
                throw new Exception("Invalid ControlType: " + ControlType);
        }
    }

    private enum AVOID_MODE {
        Normal,
        Avoid,
    }
    private AVOID_MODE _avoidMode = AVOID_MODE.Normal;
    private float _rotateAngle = 90;

    void FixedUpdate(){
        if (_idle) return;

        var position = transform.position;
        var vec2Target = _targetPosition - position;
        vec2Target.y = 0;
        var sqrMagnitude = vec2Target.sqrMagnitude;
        var inRange =  sqrMagnitude <= SqrStoppingDistance;

        var forward = transform.forward;
        forward.y = 0;
        _targetDirection.y = 0;
        var angle = Vector3.Angle(forward, _targetDirection);
        var inAngle = !_forceDirection || angle <= StoppingAngle;

        if (!(inRange && inAngle)){
            var speed = _speed;
            var rotateSpeed = _rotateSpeed;

            var targetDirection = _targetDirection;

            var normalized = vec2Target.normalized;
            if (!inRange) {
                switch (_avoidMode){
                    case AVOID_MODE.Normal:
                        {
                            var prePosition = transform.position;
                            var movement = normalized * speed * Time.deltaTime;
                            _controller.Move(movement);
                            var afterPosition = transform.position;

                            movement.y = 0;
                            var actualMovement = afterPosition - prePosition;
                            actualMovement.y = 0;
                            var delta = movement - actualMovement;
                            if (delta.sqrMagnitude / movement.sqrMagnitude > 0.04f){
                                if ((_controller.collisionFlags & CollisionFlags.Sides) != 0){
                                    // radius!
                                    // rotate ccw | cw
                                    var cross = Vector3.Cross(movement, actualMovement);
                                    var rotateAngle = 0;
                                    if (cross.y > 0){
                                        rotateAngle = 90;
                                    }else{
                                        rotateAngle = -90;
                                    }
                                    var colliders = Physics.OverlapSphere(_targetPosition, 2.0f, (1<<LayerMask.NameToLayer("Bot")));
                                    if (colliders.Length > 0){
                                        var center = Vector3.zero;
                                        foreach ( var collider_ in colliders){
                                            center += collider_.transform.position;
                                        }
                                        center /= colliders.Length;
                                        var vec2Center = center - transform.position;
                                        vec2Center.y = 0;
                                        cross = Vector3.Cross(vec2Target, vec2Center);
                                        if (cross.y > 0){
                                            rotateAngle = -90;
                                        }else{
                                            rotateAngle = 90;
                                        }
                                    }
                                    _rotateAngle = rotateAngle;
                                    var matrix = Matrix4x4.TRS(Vector3.zero,
                                            Quaternion.Euler(0, rotateAngle, 0), Vector3.one); 
                                    var rotatedDirection = matrix.MultiplyVector(vec2Target);
                                    rotatedDirection.Normalize();
                                    Debug.DrawRay(transform.position, rotatedDirection, Color.black);
                                   
                                    var additional = (delta).magnitude * rotatedDirection; 
                                    Debug.DrawRay(transform.position + Vector3.up, additional / Time.deltaTime, Color.gray);
                                    _controller.Move(additional);
                                    targetDirection = transform.position - prePosition;
                                    targetDirection.y = 0;
                                    _avoidMode = AVOID_MODE.Avoid;
                                }
                            }
                        }
                        break;
                    case AVOID_MODE.Avoid:
                        {
                            var prePosition = transform.position;
                            var movement = normalized * speed * Time.deltaTime;
                            _controller.Move(movement);
                            var afterPosition = transform.position;

                            movement.y = 0;
                            var actualMovement = afterPosition - prePosition;
                            actualMovement.y = 0;
                            var delta = movement - actualMovement;
                            if (delta.sqrMagnitude / movement.sqrMagnitude > 0.04f){
                                if ((_controller.collisionFlags & CollisionFlags.Sides) != 0){
                                    var rotateAngle = _rotateAngle;
                                    var matrix = Matrix4x4.TRS(Vector3.zero,
                                            Quaternion.Euler(0, rotateAngle, 0), Vector3.one); 
                                    var rotatedDirection = matrix.MultiplyVector(vec2Target);
                                    rotatedDirection.Normalize();
                                    Debug.DrawRay(transform.position, rotatedDirection, Color.black);
                                   
                                    var additional = (delta).magnitude * rotatedDirection; 
                                    Debug.DrawRay(transform.position + Vector3.up, additional / Time.deltaTime, Color.gray);
                                    _controller.Move(additional);
                                    targetDirection = transform.position - prePosition;
                                    targetDirection.y = 0;
                                }
                            }else{
                                _avoidMode = AVOID_MODE.Normal;
                            }
                        }
                        break;
                    default:
                        throw new Exception("Invalid _avoidMode: " + _avoidMode);
                }
            }
            // else if (!_forceStopMoving)
                // _controller.Move(normalized * speed * Time.deltaTime);

            if (!inAngle){
                var deltaAngle = _rotateSpeed * Time.deltaTime;
                deltaAngle *= Mathf.Deg2Rad;
                var newDirection = Vector3.RotateTowards(
                    forward, targetDirection, deltaAngle, 0);
                transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            }
            else if (_jumpDirection){
                transform.rotation = Quaternion.LookRotation(
                        _targetDirection, Vector3.up);
            }
        }else{
            _idle = true;
            var info = "MovementDone:\n";
            info += "angle: " + angle + "\n";
            info += "StoppingAngle: " + StoppingAngle + "\n";
            info += "sqrMagnitude: " + sqrMagnitude + "\n";
            info += "SqrStoppingDistance: " + SqrStoppingDistance + "\n";
            info += "_forceDirection: " + _forceDirection + "\n";
            Debug.Log(info);
            OnMovementDone();
        }

        Debug.DrawRay(transform.position, transform.forward, Color.blue);
        Debug.DrawRay(transform.position, transform.right, Color.red);
    }

    private void UpdateMovement(bool inRange_, bool inAngle_){
    }
    public void SetDestination(Vector3 position_, float stoppingDistance_ = 0.1f){
        _targetPosition = position_;
        SqrStoppingDistance = stoppingDistance_ * stoppingDistance_;
        _idle = false;
        _avoidMode = AVOID_MODE.Normal;

        _forceDirection = false;
    }

    public void SetDestination(
            Vector3 position_, Vector3 direction_,
            float stoppingDistance_ = 0.1f, float stoppingAngle_ = 5.0f ){
        _targetPosition = position_;
        SqrStoppingDistance = stoppingDistance_ * stoppingDistance_;
        _idle = false;

        _forceDirection = true;
        _targetDirection = direction_;
        StoppingAngle = stoppingAngle_;
    }

    public void ClearDestination(){
        _idle = true;
        OnMovementDone("DestinationCleared");
    }


    // TODO: define Param of MovementDone event
    public event Action<object> MovementDone;
    private void OnMovementDone(string reason_ = "DestinationReached"){
        if (MovementDone != null){
            MovementDone(reason_);
        }
    }

    void OnDrawGizmos(){
        // if (_idle) return;
        var svColor = Gizmos.color;
        Gizmos.color = _line2TargetPositionColor;
        Gizmos.DrawLine(transform.position, _targetPosition);

        Gizmos.color = _line2TargetDirectionColor;
        if (_forceDirection) Gizmos.DrawRay(transform.position, _targetDirection);

        Gizmos.color = svColor;
    }
}
