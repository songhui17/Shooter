using UnityEngine;
using System;

[Serializable]
public class AttackAction {
    public enum ATTACK_STATE {
        NoEnemy,
        GotoEnemy,
        Attacking,
    }
    private ATTACK_STATE _attackState = ATTACK_STATE.NoEnemy;
    public ATTACK_STATE AttackState {
        get { return _attackState; }
        set { _attackState = value; }
    }

    [Serializable]
    public class AttackDebugState {
        public bool DetectEnemy;
        public bool CanHit;
        public bool InRange;
        public bool InAngle;
        public bool IsAlive;

        public void Reset (){
            DetectEnemy = false;
            CanHit = false;
            InRange = false;
            InAngle = false;
            IsAlive = false;
        }
    }
    private AttackDebugState _debugState = new AttackDebugState();

    private GameObject _attackTarget;
    public GameObject AttackTarget { set { _attackTarget = value; } }

    public Bot Bot;
    private Transform transform { get { return Bot.transform; } }
    private Sensor Sensor { get { return Bot.Sensor; } }

    private float _attackRange = 12.0f;

    private bool ValidateTarget(GameObject attackTarget_){
        if (attackTarget_ == null) return false;

        var weapon = Bot.Weapon as RayWeapon;
        bool canHit = weapon.CanHit(attackTarget_);
        _debugState.CanHit = canHit;
        if (!canHit) return false;

        var actor = attackTarget_.GetComponent<Actor>();
        _debugState.IsAlive = actor.IsAlive;
        if (!actor.IsAlive){
            return false;
        }
        return true;
    }

    private void ChooseTarget(){
        var targetList = Sensor.AttackTargetList;
        var minDistance = float.MaxValue;
        for (int i = 0; i < targetList.Count; i++){
            var target = targetList[i];

            try{
                if (!ValidateTarget(target)) continue;
            }catch(MissingReferenceException){
                continue;
            }

            var distance = (transform.position - target.transform.position).sqrMagnitude;
            if (distance < minDistance){
                minDistance = distance;
                _attackTarget = target;
            }
        }
//        Debug.Log(string.Format("_attackTarget: {0}, minDistance: {1}",
//                   _attackTarget != null ? _attackTarget.ToString() : "null", minDistance));
    }

    private bool ValidateGotoEnemy(){
        try{
            return ValidateTarget(_attackTarget);
        }catch(MissingReferenceException){
            _attackTarget = null;
            return false;
        }
    }
    
    private bool InAttackRange(){
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

        _debugState.InRange = inRange;
        _debugState.InAngle = inAngle;
        return inRange && inAngle;
    }

//    private bool ValidateAttackEnemy(){
//        return InAttackRange();
//    }

    private bool ValidateAttacking(){
        try{
            return ValidateTarget(_attackTarget);
        }catch(MissingReferenceException){
            _attackTarget = null;
            return false;
        }
    }

    public bool Handle() {
        _debugState.Reset();

        switch (AttackState){
            case ATTACK_STATE.NoEnemy:
                {
                    var detectEnemy = false;
                    _attackTarget = null;
                    ChooseTarget();
                    detectEnemy = _attackTarget != null;

                    _debugState.DetectEnemy = detectEnemy;
                    if (detectEnemy){
                        AttackState = ATTACK_STATE.GotoEnemy;
                        Bot._patrolStatus = "attacking";
                    }
                }
                break;
            case ATTACK_STATE.GotoEnemy:
                {
                    if (!ValidateGotoEnemy()){
                        AttackState = ATTACK_STATE.NoEnemy;
                        break;
                    }

//                    var pos2Target = _attackTarget.transform.position - 
//                        transform.position;
//                    var sqrAttackRange = _attackRange * _attackRange;
//                    var inRange = sqrAttackRange >= pos2Target.sqrMagnitude; 
//
//                    var botRadius = 0.5f;
//                    // var attackAngle = 1.0f;  // TODO:
//                    var attackAngle = Mathf.Asin(botRadius / pos2Target.magnitude) * Mathf.Rad2Deg;
//                    var forward = transform.forward;
//                    forward.y = 0;
//                    var targetDirection = pos2Target;
//                    targetDirection.y = 0;
//                    var angle = Vector3.Angle(forward, targetDirection);
//                    var inAngle = attackAngle >= angle; 
//
//                    _debugState.InRange = inRange;
//                    _debugState.InAngle = inAngle;

//                    if (!(inRange && inAngle)){
                    if (!InAttackRange()){
                        var targetDirection = _attackTarget.transform.position
                            - transform.position;
                        targetDirection.y = 0;
                        Bot.TakeGotoAction(
                            _attackTarget.transform.position, _attackRange,
                            targetDirection, 1.0f, true);
                    }else{
                        AttackState = ATTACK_STATE.Attacking;
                    }
                }
                break;
            case ATTACK_STATE.Attacking:
                {
                    if (!ValidateAttacking()){
                        AttackState = ATTACK_STATE.NoEnemy;
                        break;
                    }

                    if (!InAttackRange()){
                        AttackState = ATTACK_STATE.GotoEnemy;
                        break;
                    }

                    var status = Bot.status;
                    // if (_verbose){
                    //     if (status != "shoot")
                    //         Debug.Log("change status to shoot");
                    //     else
                    //         Debug.Log("status is already shoot");
                    // }
                    if (status != "shoot"){
                        Bot.status = "shoot";
                    }
                }
                break;
            default:
                throw new Exception("Invalide AttackState: " + AttackState);
        }

        return AttackState != ATTACK_STATE.NoEnemy;
    }
} 
