using UnityEngine;

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
    private LayerMask _obstacleLayerMask;

    public SmartJumpObstacle Obstacle;
    private Vector3 _targetPosition;

    // TODO: status of Bot;
    private bool BotCrouched { get { return Bot._botCrouched; } }

    private bool CheckObstacleFront(){
        var obstacleFront = false;
        if (Obstacle != null){
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
            //if (_state == CrouchState.Stay){
            //    Debug.Log(
            //        string.Format("willHit: {0}", willHit));
            //    if (!willHit){
            //        Debug.Break();
            //    }
            //}
        }
        return obstacleFront;
    }

    private bool HandleInActiveState(){
        if (!(Motor != null && Motor.IsMoving)){
            return false;
        }

        _targetPosition = Motor.TargetPosition;
        var obstacleFront = CheckObstacleFront();
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
            return true;
        }else{
            return false;
        }
    }
    
    private void HandleStayState(){
        var obstacleFront = CheckObstacleFront();
        if (!obstacleFront){
            if (BotCrouched){
                var notStandingUp = Bot.status != "standup";
                if (notStandingUp){
                    Debug.Log(string.Format("Trigger standup action"));
                    Bot._patrolStatus = "crouch";
                    Bot.status = "standup";
                    _state = CrouchState.Exit;
                }
            }else{
                _state = CrouchState.InActive;
            }
        }
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
        var info = "CrouchAction Handle:";
        switch (_state){
            case CrouchState.InActive:
                return HandleInActiveState();
            case CrouchState.Enter:
                return HandleEnter();
            case CrouchState.Stay:
                HandleStayState();
                return false;  // move ...
            case CrouchState.Exit:
                return HandleExit();
            default:
                return false;
        }    

        // var obstacleFront = false;
        // if (Obstacle != null){
            // if (Motor != null && Motor.IsMoving){
                // var halfHeight = 1.0f;
                // var radius = 0.5f;
                // var layerMask = 1 << LayerMask.NameToLayer("SmartObject");
                // var startCenter = transform.position - Vector3.up * halfHeight;
                // var endCenter = transform.position + Vector3.up * halfHeight;
                // var targetPosition = Motor.TargetPosition;
                // var direction = targetPosition - transform.position;
                // var maxDistance = direction.magnitude;
                // direction.Normalize();
                // var willHit = Physics.CapsuleCast(
                        // startCenter, endCenter, radius,
                        // direction, maxDistance, layerMask);
                // if (willHit){
                    // obstacleFront = true;
                // }
            // }
        // }
// 
        // if (obstacleFront){
            // if (!BotCrouched){
                // var notCrouching = Bot.status != "crouch";
                // info += string.Format(
                    // "Bot.status: {0}, notCrouching {1}",
                    // Bot.status, notCrouching);
                // Debug.Log(info);
                // if (notCrouching){
                    // Debug.Log(string.Format("Trigger crouch action"));
// 
                    // Bot._patrolStatus = "crouch";
                    // Bot.status = "crouch";
                // }
                // return true;
            // }else{
                // return false;
            // }
        // }else{
            // return false;
        // }
    }
}
