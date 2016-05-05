using UnityEngine;
using System;

[Serializable]
public class CrouchAction {
    private enum CrouchState{
        Enter = 0,
        Stay,
        Exit,
        InActive,
    }

    private CrouchState _state = CrouchState.InActive;

    public Bot Bot;
    private Transform transform { get { return Bot.transform; } }
    private AutoMotor Motor { get { return Bot.Motor; } }
    private Sensor Sensor { get { return Bot.Sensor; } }
    private LayerMask _obstacleLayerMask;

    private SmartJumpObstacle _obstacle;
    private Vector3 _targetPosition;

    // TODO: status of Bot;
    private bool BotCrouched { get { return Bot._botCrouched; } }

    private bool CheckObstacleFront(){
        var obstacleFront = false;
        if (_obstacle != null){
            var halfHeight = 1.0f;
            var radius = 0.5f;
            var layerMask = 1 << LayerMask.NameToLayer("SmartObject");
            var startCenter = transform.position - Vector3.up * halfHeight;
            var endCenter = transform.position + Vector3.up * halfHeight;
            var targetPosition = _targetPosition;
            var direction = targetPosition - transform.position;
            var maxDistance = direction.magnitude;
            direction.Normalize();
            var willHit = Physics.CapsuleCast(
                    startCenter, endCenter, radius,
                    direction, maxDistance, layerMask);
            if (willHit){
                obstacleFront = true;
            }
        }
        return obstacleFront;
    }

    private bool CheckObstacleFront(SmartJumpObstacle obstacle_){
        if (obstacle_ == null) return false;

        var willEnter = false;
        var direction = _targetPosition - transform.position;
        willEnter = Vector3.Dot(direction, obstacle_.transform.forward) > 0;
        return willEnter;
    }

    private bool HandleInActiveState(){
        if (!(Motor != null && Motor.IsMoving)){
            return false;
        }
        _targetPosition = Motor.TargetPosition;
        _obstacle = Sensor.Obstacle;
        var obstacleFront = CheckObstacleFront(_obstacle);
        if (obstacleFront){
            if (!BotCrouched){
                var notCrouching = Bot.status != "crouch";
                if (notCrouching){
                    Debug.Log(string.Format("Trigger crouch action"));
                    Bot._patrolStatus = "crouch";
                    Bot.status = "crouch";
                    _state = CrouchState.Enter;
                }
            }else{
                _state = CrouchState.Stay;
            }
            Sensor.Handle(_obstacle);
            return true;
        }else{
            return false;
        }
    }
    
    private bool HandleStayState(){
        _obstacle = Sensor.Obstacle;
        if (_obstacle == null) return false;

        var obstacleFront = CheckObstacleFront(_obstacle);
        if (!obstacleFront){
            if (BotCrouched){
                var notStandingUp = Bot.status != "standup";
                if (notStandingUp){
                    Debug.Log(string.Format("Trigger standup action"));
                    Bot._patrolStatus = "crouch";
                    Bot.status = "standup";
                    _state = CrouchState.Exit;
                    Sensor.Handle(_obstacle);
                    return true;
                }
            }else{
                _state = CrouchState.InActive;
            }
            Sensor.Handle(_obstacle);
        }
        return false;
    }

    private bool HandleEnter(){
        if (BotCrouched){
            _state = CrouchState.Stay;
        }
        return true;
    }

    private bool HandleExit(){
        if (!BotCrouched){
            _state = CrouchState.InActive;
            return false;
        }else{
            return true;
        }
    }

    public bool Handle(){
        switch (_state){
            case CrouchState.InActive:
                return HandleInActiveState();
            case CrouchState.Enter:
                return HandleEnter();
            case CrouchState.Stay:
                return HandleStayState();
            case CrouchState.Exit:
                return HandleExit();
            default:
                return false;
        }    
    }
}
