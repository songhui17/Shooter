using UnityEngine;

public class SpinningActivate : MonoBehaviour {
    [SerializeField]
    private Animator _animator;

    [SerializeField]
    private float _spinningSpeed = 1;

    public void SetActive(bool value_) {
        gameObject.SetActive(value_);
        if (value_) {
            _animator.speed = _spinningSpeed;
            _animator.SetTrigger("restart");
        }
    }
}
