using UnityEngine;
using System.Collections;

public class AvatarShow : MonoBehaviour {
    public Animator _animator;
    public CharacterController _controller;
    public float _fallingSpeed;
    public float _startHeight = 1.0f;

    private float _rotateSpeed = 0;
    [SerializeField]
    private float _drag = 0.1f;

    [SerializeField]
    private float _multiplier = 100;
    public JoyStick _joystick;

    void Start() {
        StartCoroutine(WaitForLoadingDone());
    }

    IEnumerator WaitForLoadingDone(){
        while (true) {
            if (Blackboard.Instance.LastLoadingDone) {
                break;
            }
            yield return null;
        }
        Enter();
    }

    void Enter() {
        var localPosition = transform.localPosition;
        localPosition.y = _startHeight;
        transform.localPosition = localPosition;
        _animator.SetTrigger("jump");
        _animator.SetBool("IsGrounded", false);
    }

    void Update() {
        var state = _animator.GetCurrentAnimatorStateInfo(0);
        if (state.IsName("soldierFalling")){
            _controller.Move(Vector3.down * _fallingSpeed * Time.deltaTime);
            if (_controller.isGrounded){
                _animator.SetBool("IsGrounded", true);
            }
        }else if (state.IsName("soliderIdleRelaxed")) {

        }

        var rotateY = -_joystick.GetAxis("Mouse X");
        _rotateSpeed += rotateY * Time.deltaTime;
        if (Mathf.Abs(_rotateSpeed) > 0) {
            var newSpeed = _rotateSpeed;
            if (rotateY == 0)
                newSpeed += (-_rotateSpeed / Mathf.Abs(_rotateSpeed)) * _drag * Time.deltaTime;

            if (newSpeed * _rotateSpeed < 0) {
                _rotateSpeed = 0;
            }else{
                _rotateSpeed = newSpeed;
            }
            transform.Rotate(Vector3.up, _multiplier * _rotateSpeed * Time.deltaTime, Space.World);
        }
    }
}
