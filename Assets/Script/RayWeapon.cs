using UnityEngine;

public class RayWeapon : Weapon {
    private GameObject _bulletPrefab;
    void Start(){
        _bulletPrefab = Resources.Load("RayBullet", typeof(GameObject)) as GameObject;
    }
    public override void Attack(){
        Debug.Log("Attack");
//        var bullet = new GameObject("RayBullet");
        var bullet = Instantiate(_bulletPrefab) as GameObject;
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        // var script = bullet.AddComponent<RayBullet>();
        var script = bullet.GetComponent<RayBullet>();
        script.Fire(transform.position, transform.forward);
    }

    public bool CanHit(GameObject target_){
        var _startPosition = transform.position;
        var _direction = target_.transform.position - transform.position;
        _direction.y = 0;

        RaycastHit hit;
        // TODO: handle block
        if (Physics.Raycast(_startPosition, _direction, out hit)){
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
