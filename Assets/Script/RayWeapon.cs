using UnityEngine;

public class RayWeapon : Weapon {
    private GameObject _bulletPrefab;
    public Transform Muzzle;

    void Start(){
        _bulletPrefab = Resources.Load("RayBullet", typeof(GameObject)) as GameObject;
    }

    public override void Attack(){
        Debug.Log("Attack");
        var bullet = Instantiate(_bulletPrefab) as GameObject;
        bullet.transform.position = Muzzle.position;
        bullet.transform.rotation = Muzzle.rotation;
        var script = bullet.GetComponent<RayBullet>();
        script.Fire(Muzzle.position, Muzzle.forward);
    }

    public override bool CanHit(GameObject target_){
        var _startPosition = transform.position;
        var _direction = target_.transform.position - Muzzle.position;
        _direction.y = 0;

        RaycastHit hit;
        // TODO: handle block
        var layerMask = ~(1 << LayerMask.NameToLayer("SmartTrigger"));
        if (Physics.Raycast(_startPosition, _direction, out hit,
                _direction.magnitude, layerMask)){
            var hitTarget = target_ == hit.collider.gameObject;
            if (!hitTarget){
                Debug.DrawLine(
                    transform.position, hit.collider.transform.position,
                    Color.red);
                // Debug.Break();
                // Debug.Log(hit.collider.gameObject);
            }
            return hitTarget;
        }else{
            return false;
        }
    }

    void OnDrawGizmos(){
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
