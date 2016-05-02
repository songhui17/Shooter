using UnityEngine;
using System;
using System.Collections;

public class AutoMotor : MonoBehaviour {
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

    private bool _forceStopMoving = true;

    private bool _forceDirection = false;
    private Vector3 _targetForward;
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
    }

    void FixedUpdate(){
        if (_idle) return;

        var position = transform.position;
        var vec2Target = _targetPosition - position;
        var sqrMagnitude = vec2Target.sqrMagnitude;
        var inRange =  sqrMagnitude <= SqrStoppingDistance;

        var forward = transform.forward;
        forward.y = 0;
        _targetForward.y = 0;
        var angle = Vector3.Angle(forward, _targetForward);
        var inAngle = !_forceDirection || angle <= StoppingAngle;

        if (!(inRange && inAngle)){
            var speed = _speed;
            var rotateSpeed = _rotateSpeed;

            var normalized = vec2Target.normalized;
            if (!inRange)
                _controller.Move(normalized * speed * Time.deltaTime);
            else if (!_forceStopMoving)
                _controller.Move(normalized * speed * Time.deltaTime);

            if (!inAngle){
                var deltaAngle = _rotateSpeed * Time.deltaTime;
                deltaAngle *= Mathf.Deg2Rad;
                var newDirection = Vector3.RotateTowards(
                    forward, _targetForward, deltaAngle, 0);
                transform.rotation = Quaternion.LookRotation(newDirection, Vector3.up);
            }
            else if (_jumpDirection){
                transform.rotation = Quaternion.LookRotation(
                        _targetForward, Vector3.up);
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
    }

    private void UpdateMovement(bool inRange_, bool inAngle_){
    }
    public void SetDestination(Vector3 position_, float stoppingDistance_ = 0.1f){
        _targetPosition = position_;
        SqrStoppingDistance = stoppingDistance_ * stoppingDistance_;
        _idle = false;

        _forceDirection = false;
    }

    public void SetDestination(
            Vector3 position_, Vector3 direction_,
            float stoppingDistance_ = 0.1f, float stoppingAngle_ = 5.0f ){
        _targetPosition = position_;
        SqrStoppingDistance = stoppingDistance_ * stoppingDistance_;
        _idle = false;

        _forceDirection = true;
        _targetForward = direction_;
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
        if (_forceDirection) Gizmos.DrawRay(transform.position, _targetForward);

        Gizmos.color = svColor;
    }
}
