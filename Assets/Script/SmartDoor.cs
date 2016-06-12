using UnityEngine;
using System.Collections;

public class SmartDoor : MonoBehaviour {
    [SerializeField]
    private bool _autoOpen = false;

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
        // Debug.Log(string.Format(
        //             "OnCollisionEnter collision_.gameObject: {0}",
        //             collision_.gameObject));
        // TODO:
        var rigidBody = GetComponent<Rigidbody>();
        if (rigidBody != null) rigidBody.isKinematic = true;

        collision_.gameObject.SendMessage(
                "ReceiveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);

        ShowHint = true;
    }

    void OnTriggerEnter(Collider collider_){
        if (collider_.tag != "Player") return;
        Debug.Log(string.Format(
                    "OnTriggerEnter collider_.gameObject: {0}",
                    collider_.gameObject));

        collider_.gameObject.SendMessage(
                "ReceiveSmartObject", gameObject,
                SendMessageOptions.DontRequireReceiver);
        if (!Opened) {
            ShowHint = true;

            if (_autoOpen) {
                Trigger();
            }
        }
    }

    void OnTriggerExit(Collider collider_){
        if (collider_.tag != "Player") return;
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
        StartCoroutine(CoWaitForOpen());
    }


    [SerializeField]
    private Vector2[] _occupiedTiles;

    IEnumerator CoWaitForOpen() {
        while (true) {
            yield return null;

            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if(stateInfo.IsName("SmartSlideDoorOpen")) {
                // Debug.Break();
                foreach(var tile in _occupiedTiles) {
                    var columnCount = TileMap.Instance.ColumnCount;
                    TileMap.Instance.Tiles[(int)(tile.x * columnCount + tile.y)] = 0;
                }
                break;
            }
        }
    }
}
