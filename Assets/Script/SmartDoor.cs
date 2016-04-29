using UnityEngine;
using System.Collections;

public class SmartDoor : MonoBehaviour {
    [SerializeField]
    private Animator _animator;

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
    }

    void Trigger(){
        _animator.SetBool("open", true);
    }
}
