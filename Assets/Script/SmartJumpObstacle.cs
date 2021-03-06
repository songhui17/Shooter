using UnityEngine;

public class SmartJumpObstacle : MonoBehaviour {
    public float Height = 1.0f;
    public bool Handled = false;

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

    void OnTriggerEnter(Collider collider_){
        Debug.Log(string.Format(
                    "OnTriggerEnter collider_.gameObject: {0}",
                    collider_.gameObject));

        collider_.gameObject.SendMessage(
                "ReceiveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);
    }

    // characterController change cause re-trigger
    // cause OnTriggerExit && OnTriggerEnter
    void OnTriggerExit(Collider collider_){
        Debug.Log(string.Format(
                    "OnTriggerExit collider_.gameObject: {0}",
                    collider_.gameObject));

        collider_.gameObject.SendMessage(
                "LeaveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);
    }
}
