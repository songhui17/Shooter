using UnityEngine;

public class RayBullet : MonoBehaviour {

    private Vector3 _startPosition;
    private Vector3 _direction;
    private Vector3 _targetPosition;

    private float _maxLength = 2;
    private float _speed = 20;

    private float _head;
    private float _tail;
    private float _target;

    void Update(){
        _tail += _speed * Time.deltaTime;
        _head += _speed * Time.deltaTime;
        var tail = Mathf.Min(_target, Mathf.Max(0, _tail));
        var head = Mathf.Min(_target, _head);

        if (tail == _target && head == _target){
            Destroy(gameObject);
            return;
        }

        var tailPosition = _startPosition + _direction * tail;
        var headPosition = _startPosition + _direction * head;
        Debug.DrawLine(tailPosition, headPosition, Color.yellow);
    }

    public void Fire(Vector3 position_, Vector3 direction_){
        _direction = direction_.normalized;
        _startPosition = position_;

        RaycastHit hit;
        // TODO: handle block
        if (Physics.Raycast(_startPosition, _direction, out hit)){
            _targetPosition = hit.point;
        }else{
            _targetPosition = position_ + direction_ * 100;
        }

        _target = (_targetPosition - _startPosition).magnitude;
        _head = 0;
        _tail = -_maxLength;
    }
}
