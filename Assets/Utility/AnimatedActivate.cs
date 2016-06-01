using UnityEngine;
using System;
using System.Collections;

public class AnimatedActivate : MonoBehaviour {

    enum STATE {
        Activating,
        Active,
        DisActivating,
        DisActive,
    }
    private STATE _state = STATE.DisActive;

    [SerializeField]
    private float _delaySeconds = 5;

    [SerializeField]
    private float _speed = 5;

    private Animator _animator;
    public Animator Animator {
        get { return _animator ?? (_animator = GetComponent<Animator>()); }
    }

    public void SetActive(bool value_) {
        if (value_ && _state == STATE.Active
            || !value_ && _state == STATE.DisActive) {
            return;
        }

        if (_state == STATE.DisActivating || _state == STATE.Activating) {
            // return;
            StopAllCoroutines();
        }

        if (value_) {
            _state = STATE.Activating;
            gameObject.SetActive(value_);
        }else{
            _state = STATE.DisActivating;
        }

        Animator.speed = _speed;
        StartCoroutine(CoAnimate(value_));
    }

    public void SetActiveAnimated(bool value_, Action callback_) {
        if (value_ && _state == STATE.Active
            || !value_ && _state == STATE.DisActive) {
            return;
        }

        if (_state == STATE.DisActivating || _state == STATE.Activating) {
            // return;
            StopAllCoroutines();
        }

        if (value_) {
            _state = STATE.Activating;
            gameObject.SetActive(value_);
        }else{
            _state = STATE.DisActivating;
        }

        Animator.speed = 1;
        StartCoroutine(CoAnimate(value_, _delaySeconds, callback_));
    }

    IEnumerator CoAnimate(bool value_, float delaySeconds_ = 0, Action callback_ = null) {
        if (delaySeconds_ > 0) yield return new WaitForSeconds(delaySeconds_);

        Animator.SetBool("Active", value_);
        while (true) {
            if (Animator.IsInTransition(0)) {
                yield return null;
            }

            var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
            switch (_state) {
                case STATE.Activating:
                    {
                        if (stateInfo.IsName("Active")) {
                            _state = STATE.Active;
                        }
                    }
                    break;
                case STATE.DisActivating:
                    {
                        if (stateInfo.IsName("DisActive")) {
                            _state = STATE.DisActive;
                            gameObject.SetActive(false);
                        }
                    }
                    break;
                default:
                    break;
            }

            if (_state == STATE.Active || _state == STATE.DisActive) {
                if (callback_ != null) {
                    callback_();
                }
                break;
            }

            yield return null;  // TODO
        }
    }
}
