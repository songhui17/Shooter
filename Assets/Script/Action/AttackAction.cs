using UnityEngine;

public class AttackAction {
    private GameObject _attackTarget;
    public GameObject AttackTarget { set { _attackTarget = value; } }

    public Bot Bot;
    private Transform transform { get { return Bot.transform; } }
    private Sensor Sensor { get { return Bot.Sensor; } }

    private float _attackRange = 12.0f;

    private void ChooseTarget(){
        var targetList = Sensor.AttackTargetList;
        var minDistance = float.MaxValue;
        for (int i = 0; i < targetList.Count; i++){
            var target = targetList[i];
            var distance = (transform.position - target.transform.position).sqrMagnitude;
            if (distance < minDistance){
                minDistance = distance;
                _attackTarget = target;
            }
        }
//        Debug.Log(string.Format("_attackTarget: {0}, minDistance: {1}",
//                   _attackTarget != null ? _attackTarget.ToString() : "null", minDistance));
    }

    public bool Handle() {
        var detectEnemy = _attackTarget != null;

        if (!detectEnemy){
            ChooseTarget();
        }

        if (detectEnemy){
            // TODO: weapon
            var weapon = Bot.Weapon as RayWeapon;
            bool canHit = weapon.CanHit(_attackTarget);
            if (!canHit) return false;

            var actor = _attackTarget.GetComponent<Actor>();
            if (!actor.IsAlive){
                return false;
            }

            Bot._patrolStatus = "attacking";

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
