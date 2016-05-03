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

    // public GameObject gameObject { get { return Bot.gameObject; } }
    // public Transform transform { get { return Bot.transform; } }

    SmartDoor _door;
    public SmartDoor Door { get { return _door; } }

    SmartJumpObstacle _obstacle;
    public SmartJumpObstacle Obstacle { get { return _obstacle; } }

    void ReceiveSmartObject(GameObject object_){
        if (_verbose)
            Debug.Log(string.Format("ReceiveSmartObject object_: {0}", object_));
        _door = object_.GetComponent<SmartDoor>();
        if (_door != null){
            // TODO: push door
            // Q: why take action on receive
        }

        _obstacle = object_.GetComponent<SmartJumpObstacle>();
        if (_obstacle != null){
            // _obstacle.TriggerAnimation
        }
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
