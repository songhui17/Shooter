using UnityEngine;

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
