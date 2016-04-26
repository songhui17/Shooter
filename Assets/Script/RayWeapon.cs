using UnityEngine;

public class RayWeapon : Weapon {
    public override void Attack(){
        Debug.Log("Attack");
        var bullet = new GameObject("RayBullet");
        bullet.transform.position = transform.position;
        bullet.transform.rotation = transform.rotation;
        var script = bullet.AddComponent<RayBullet>();
        script.Fire(transform.position, transform.forward);
    }
}
