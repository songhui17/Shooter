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

    void OnDrawGizmos(){
        Gizmos.DrawRay(transform.position, transform.forward);
    }
}
