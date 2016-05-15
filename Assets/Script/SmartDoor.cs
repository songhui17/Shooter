using UnityEngine;
using System.Collections;

public class SmartDoor : MonoBehaviour {
    [SerializeField]
    private Animator _animator;
    public bool Opened { 
        get { 
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName("SmartDoorOpen");
        }
    }

    [SerializeField]
    private GameObject _hintCanvas;
    private bool _showHint = false;
    private bool ShowHint {
        set {
            _showHint = value;
            if (_hintCanvas != null) {
                _hintCanvas.SetActive(_showHint);
            }
        }
    }
    
    void Awake(){
        ShowHint = false;
    }

    void OnCollisionEnter(Collision collision_){
        Debug.Log(string.Format(
                    "OnCollisionEnter collision_.gameObject: {0}",
                    collision_.gameObject));
        // TODO:
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody != null) rigidBody.isKinematic = true;

        collision_.gameObject.SendMessage(
                "ReceiveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);

        ShowHint = true;
    }

    void OnTriggerEnter(Collider collider_){
        Debug.Log(string.Format(
                    "OnCollisionEnter collider_.gameObject: {0}",
                    collider_.gameObject));

        collider_.gameObject.SendMessage(
                "ReceiveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);

        if (!Opened) {
            ShowHint = true;
        }
    }

    void OnTriggerExit(Collider collider_){
        Debug.Log(string.Format(
                    "OnTriggerExit collider_.gameObject: {0}",
                    collider_.gameObject));

        collider_.gameObject.SendMessage(
                "LeaveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);

        ShowHint = false;
    }

    void Trigger(){
        _animator.SetBool("open", true);
        ShowHint = false;
    }
}
