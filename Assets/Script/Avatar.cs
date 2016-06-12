using UnityEngine;
using System.Collections;

public class Avatar : MonoBehaviour {
    private Bot _bot;
    
    [SerializeField]
    private LayerMask _layerMask;
    [SerializeField]
    private HPView _enemyHPView;

    [SerializeField]
    private Transform _mainCamera;

    void Start() {
        _bot = GetComponent<Bot>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.F)){
            _bot.status = "open_door";
        }

        Fire(_mainCamera.position, _mainCamera.forward);
    }

    // TODO: copy from RayBullet.cs
    public void Fire(Vector3 position_, Vector3 direction_){
        var _direction = direction_.normalized;
        var _startPosition = position_;

        RaycastHit hit;
        if (Physics.Raycast(_startPosition, _direction, out hit,
                100, _layerMask)){
            var bot = hit.collider.GetComponent<Actor>();
            if (bot != null){
                _enemyHPView.gameObject.SetActive(true);
                _enemyHPView.DataContext = bot;

                var hpCanvasPosition = bot.transform.position + Vector3.up;
                var eventCamera = _mainCamera.GetComponent<Camera>();
                var screenPoint = eventCamera.WorldToScreenPoint(hpCanvasPosition);
                screenPoint.z = 0;
                _enemyHPView.transform.position = screenPoint;
            }else{
                _enemyHPView.gameObject.SetActive(false);
            }
        }else{
            _enemyHPView.gameObject.SetActive(false);
        }
    }
}
