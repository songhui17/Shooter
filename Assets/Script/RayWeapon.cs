using UnityEngine;

public class RayWeapon : Weapon {
    private GameObject _bulletPrefab;
    public int TotalBullet = 18;
    public int LeftBullet = 18;

    public Transform Muzzle;
    public Animator GunAnimator;

    void Start(){
        _bulletPrefab = Resources.Load("RayBullet", typeof(GameObject)) as GameObject;
    }

    void Update(){
        if (Input.GetButton("Fire1")){
            var currentStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(0);
            if (currentStateInfo.IsName("default")){
                if (GunAnimator.IsInTransition(0)){
                    // var nextStateInfo = GunAnimator.GetNextAnimatorStateInfo(0);
                    // if (!nextStateInfo.IsName("MachineGun_shoot")){
                        // Attack();
                    // }
                }else{
                    Attack();
                }
            }
        }else{
            // AnimatorStateInfo animatorStateInfo;
            // if (GunAnimator.IsInTransition(0)){
                // animatorStateInfo = GunAnimator.GetNextAnimatorStateInfo(0);
            // }else{
                // animatorStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(0);
            // }
            // if (animatorStateInfo.IsName("MachineGun_shoot")){
                // GunAnimator.SetTrigger("Shoot");
                // // GunAnimator.SetBool("shoot", false);
            // }
        }
        
        if (Input.GetKeyDown(KeyCode.R)){
            if (GunAnimator.IsInTransition(0)){
                var animatorStateInfo = GunAnimator.GetNextAnimatorStateInfo(0);
                if (!animatorStateInfo.IsName("MachineGun_reload")){
                    Reload();
                } 
            }else{
                var animatorStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(0);
                if (!animatorStateInfo.IsName("MachineGun_reload")){
                    Reload();
                } 
            }
        }
    }

    public override bool Attack(){
        var info = "Attack:";
        info += "LeftBullet: " + LeftBullet + "\n";
        if (LeftBullet == 0) {
            var animatorStateInfo = GunAnimator.GetCurrentAnimatorStateInfo(0);
            if (GunAnimator.IsInTransition(0)){
                animatorStateInfo = GunAnimator.GetNextAnimatorStateInfo(0);
            }
            if (!animatorStateInfo.IsName("MachineGun_open")){
                GunAnimator.SetTrigger("DoOpen");
            }
            Debug.Log(info);
            return false;
        }else{
            GunAnimator.SetTrigger("Shoot");
            // GunAnimator.SetBool("shoot", true);
            LeftBullet--;

            var bullet = Instantiate(_bulletPrefab) as GameObject;
            bullet.transform.position = Muzzle.position;
            bullet.transform.rotation = Muzzle.rotation;
            var script = bullet.GetComponent<RayBullet>();
            script.Fire(Muzzle.position, Muzzle.forward);

            Debug.Log(info);
            return true;
        }
    }

    public override bool Reload(){
        var left = LeftBullet;
        LeftBullet += 18;
        GunAnimator.SetTrigger("DoReload");
        Debug.Log(string.Format(
            "Reload LeftBullet {0} -> {1}", left, LeftBullet));
        return true;
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
