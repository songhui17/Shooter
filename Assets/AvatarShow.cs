using UnityEngine;
using System.Collections;

public class AvatarShow : MonoBehaviour {
    public Animator _animator;
    public CharacterController _controller;
    public float _fallingSpeed;
    public float _startHeight = 1.0f;

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
    }
}
