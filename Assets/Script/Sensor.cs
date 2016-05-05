using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Sensor : MonoBehaviour {
    private bool _verbose = true;

    [SerializeField]
    private LayerMask _targetLayer;
    public LayerMask TargetLayer { set { _targetLayer = value;} }
   
    private float _detectRadius = 10.0f; 

    private List<GameObject> _attackTargetList;
    public List<GameObject> AttackTargetList {
        get { return _attackTargetList ?? (_attackTargetList = new List<GameObject>()); }
    }

    private Bot _bot;
    public Bot Bot {
        get { return _bot ?? (_bot = GetComponent<Bot>()); }
    }

    [SerializeField]
    private Transform _headTransform;

    private AutoMotor Motor {
        get { return Bot.Motor; }
    }

    // SmartDoor _door;
    private List<SmartDoor> _pendingSmartDoor = new List<SmartDoor>();
    private List<SmartDoor> _handledSmartDoor = new List<SmartDoor>();
    public SmartDoor Door {
        get {
            if (_pendingSmartDoor.Count > 0){
                return _pendingSmartDoor[0];
            }else{
                return null;
            }
        }
    }

    // SmartJumpObstacle _obstacle;
    private List<SmartJumpObstacle> _pendingJumpObstacle = new List<SmartJumpObstacle>();
    public SmartJumpObstacle Obstacle {
        get {
            if (_pendingJumpObstacle.Count > 0){
                return _pendingJumpObstacle[0];
            }else{
                return null;
            }
        }
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

    public void Handle(SmartJumpObstacle obstacle_){
        if (obstacle_ == null) return;
        _pendingJumpObstacle.Remove(obstacle_);
    }

    public void Handle(SmartDoor door_){
        if (door_ == null) return;
        _pendingSmartDoor.Remove(door_);
    }

    void Start(){
        StartCoroutine(UpdateSensor());
    }

    IEnumerator UpdateSensor(){
        var updateDuration = 0.2f;
        var waitForSeconds = new WaitForSeconds(updateDuration);
        while (true){
            yield return waitForSeconds;

            DetecteAttackTarget();
            DetectSmartObstacle();
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

    void DetectSmartObstacle(){
        var motor = Motor;
        if (motor == null) return;
    }

    void OnDrawGizmos(){
        // if (_attackTarget != null){
        //     Gizmos.DrawLine(transform.position,
        //             _attackTarget.transform.position);
        // }

        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
}
