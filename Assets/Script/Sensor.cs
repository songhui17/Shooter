using UnityEngine;
using System.Collections.Generic;

public class Sensor : MonoBehaviour {
    private bool _verbose = true;

    [SerializeField]
    private LayerMask _targetLayer;
    public LayerMask TargetLayer { set { _targetLayer = value;} }
   
    private float _detectRadius = 10.0f; 

    [SerializeField]
    private GameObject _attackTarget = null;
    public GameObject AttackTarget { get { return _attackTarget; } }

    private Bot _bot;
    public Bot Bot {
        get { return _bot ?? (_bot = GetComponent<Bot>()); }
    }

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

    public void Update() {
        DetecteAttackTarget();
        DetectSmartObstacle();
    }

    void DetecteAttackTarget(){
        if (_attackTarget == null){
            var detectRadius = _detectRadius;
            var layerMask = _targetLayer.value;

            // if (_verbose)
                // Debug.Log(string.Format(
                    // "_targetLayer.value: {0}", _targetLayer.value));

            var colliders = Physics.OverlapSphere(
                    transform.position, detectRadius, layerMask);

            var targetColliders = new List<Collider>();
            _attackTarget = null;
            for (int i = 0; i < colliders.Length; i++){
                var collider = colliders[i];
                if (collider.gameObject != gameObject){
                    targetColliders.Add(collider);
                     
                    var actor = collider.GetComponent<Actor>();
                    // TODO: copy from AttackAction.cs
                    var weapon = Bot.Weapon as RayWeapon;
                    bool canHit = weapon.CanHit(collider.gameObject);

                    if (canHit && actor.IsAlive){
                        _attackTarget = collider.gameObject;
                        // TODO: validate raycast
                        Debug.Log("Detect _attackTarget: " + _attackTarget);
                        break;
                    }
                }
            }
        }
    }

    void DetectSmartObstacle(){
        var motor = Motor;
        if (motor == null) return;
    }

    void OnDrawGizmos(){
        if (_attackTarget != null){
            Gizmos.DrawLine(transform.position,
                    _attackTarget.transform.position);
        }

        Gizmos.DrawWireSphere(transform.position, _detectRadius);
    }
}
