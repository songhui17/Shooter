using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BotSensor : Sensor {
    private bool _verbose = true;

    [SerializeField]
    protected Transform _headTransform;

    [SerializeField]
    private LayerMask _targetLayer;
    public LayerMask TargetLayer { set { _targetLayer = value;} }
   
    private float _detectRadius = 10.0f; 

    void Start(){
        StartCoroutine(UpdateSensor());
    }

    IEnumerator UpdateSensor(){
        var updateDuration = 0.2f;
        var waitForSeconds = new WaitForSeconds(updateDuration);
        while (true){
            yield return waitForSeconds;

            DetecteAttackTarget();
            // DetectSmartObstacle();
        }
    }

    void DetecteAttackTarget(){
        // if (_attackTarget == null){
            var detectRadius = _detectRadius;
            var layerMask = _targetLayer.value;

            var colliders = Physics.OverlapSphere(
                    transform.position, detectRadius, layerMask);

            AttackTargetList.Clear();
            for (int i = 0; i < colliders.Length; i++){
                var collider = colliders[i];
                var targetGameObject = collider.gameObject;
                if (targetGameObject != gameObject){
                     
                    var actor = targetGameObject.GetComponent<Actor>();
                    var alive = actor.IsAlive;

                    var canSee = true;
                    var head = _headTransform.position;
                    var targetPosition = targetGameObject.transform.position;
                    var direction = targetPosition - head;
                    var distance = direction.magnitude;
                    var environmentLayer = ~(1 << LayerMask.NameToLayer("Default"));
                    direction.Normalize();
                    RaycastHit hit;
                    if (Physics.Raycast(head, direction, out hit,
                                        distance, environmentLayer)){
                    }else{
                        canSee = false;
                        continue;
                    }

                    if (canSee && actor.IsAlive){
                        // Debug.Log("Detect targetGameObject: " + targetGameObject);
                        AttackTargetList.Add(targetGameObject);
                    }
                }
            }
        // }
    }

    void OnDrawGizmos(){
        // if (_attackTarget != null){
        //     Gizmos.DrawLine(transform.position,
        //             _attackTarget.transform.position);
        // }

        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }

    void ReceiveSmartObject(GameObject object_){
        if (_verbose)
            Debug.Log(string.Format("ReceiveSmartObject object_: {0}", object_));
        var _door = object_.GetComponent<SmartDoor>();
        if (_door != null){
            _pendingSmartDoor.Add(_door);
        }

        // CharacterController change size cause OnTriggerExit && OnTriggerEnter
//        var _obstacle = object_.GetComponent<SmartJumpObstacle>();
//        if (_obstacle != null){
//            _pendingJumpObstacle.Add(_obstacle);
//        }
    }

    private void LeaveSmartObject(GameObject object_){
        if (_verbose)
            Debug.Log(string.Format("LeaveSmartObject object_: {0}", object_));

        var _door = object_.GetComponent<SmartDoor>();
        if (_door != null){
            _pendingSmartDoor.Remove(_door);
        }

        // CharacterController change size cause OnTriggerExit && OnTriggerEnter
//        var _obstacle = object_.GetComponent<SmartJumpObstacle>();
//        if (_obstacle != null){
//            _pendingJumpObstacle.Remove(_obstacle);
//        }
        var _obstacle = object_.GetComponent<SmartJumpObstacle>();
        if (_obstacle != null){
            _pendingJumpObstacle.Add(_obstacle);
        }
    }

}
