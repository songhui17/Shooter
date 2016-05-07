using UnityEngine;

public class MeleeWeapon : Weapon {
    private GameObject _bulletPrefab;
    public Transform Muzzle;

    void Start(){
        _bulletPrefab = Resources.Load("RayBullet", typeof(GameObject)) as GameObject;
    }

    public override bool Attack(){
        Debug.Log("Attack");
        var bullet = Instantiate(_bulletPrefab) as GameObject;
        bullet.transform.position = Muzzle.position;
        bullet.transform.rotation = Muzzle.rotation;
        var script = bullet.GetComponent<RayBullet>();
        script.Fire(Muzzle.position, Muzzle.forward);
        return true;
    }
    
    public override bool CanHit(GameObject target_){
        if (target_ == null) return false;
        var toTarget = target_.transform.position - transform.position;
        toTarget.y = 0;
        if (toTarget.sqrMagnitude <= 4) {
            return true;
        }else{
            return false;
        }
    }
}
