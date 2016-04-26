using UnityEngine;
using System;
using System.Collections;

public class AutoMotor : MonoBehaviour {

    private bool _idle = true;
    private Vector3 _targetPosition;

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

    void Update(){
        if (_idle) return;

        var position = transform.position;
        var vec2Target = _targetPosition - position;
        if (vec2Target.sqrMagnitude > 0.01f){
            var speed = 4.0f;
            var normalized = vec2Target.normalized;
            _controller.Move(normalized * speed * Time.deltaTime);
        }else{
            _idle = true;
            OnMovementDone();
        }
    }

    public void SetDestination(Vector3 position_){
        _targetPosition = position_;
        _idle = false;
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
}
