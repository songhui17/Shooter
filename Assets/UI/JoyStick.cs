using UnityEngine;
using UnityEngine.EventSystems;

public class JoyStick : MonoBehaviour
        , IPointerDownHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler {

    private Vector3 _initPosition;

    [SerializeField]
    private RectTransform rectTransform;

    [SerializeField]
    private RectTransform rectTransform2;

    private float _prevHorizontal;
    private float _horizontal;
    private float _mouseX;

    private float _prevVertical;
    private float _vertical;
    private float _mouseY;

    private void UpdatePosition() {
        var normalized = rectTransform2.localPosition.x / rectTransform.rect.width;
        _horizontal = Mathf.Clamp(normalized, -1, 1);

        normalized = rectTransform2.localPosition.y / rectTransform.rect.height;
        _vertical = Mathf.Clamp(normalized, -1, 1);
    }

    public static JoyStick Left { get; private set; }
    public static JoyStick Right { get; private set; }

    [SerializeField]
    private string _key = "main";
    
    void Awake() {
        if (_key == "left") {
            Left = this;
            // TODO
            DontDestroyOnLoad(transform.root);
        }else if (_key == "right") {
            Right = this;
            // TODO
            DontDestroyOnLoad(transform.root);
        }
    }

    void Start() {
        _initPosition = rectTransform.position;
    }

    public void OnPointerDown(PointerEventData eventData) {
        rectTransform.position = eventData.position;
        rectTransform2.localPosition = Vector3.zero;

        _prevHorizontal = 0;
        _mouseX = 0;
        _prevVertical = 0;
        _mouseY = 0;

        UpdatePosition();
    }
    
    public void OnPointerUp(PointerEventData eventData) {
        rectTransform.position = _initPosition;
        rectTransform2.localPosition = Vector3.zero;

        _horizontal = 0;
        _prevHorizontal = 0;
        _mouseX = 0;

        _vertical = 0;
        _prevVertical = 0;
        _mouseY = 0;
    }

    public void OnBeginDrag(PointerEventData eventData) {
        // Debug.Log("OnPointerEnter:" + eventData);
        // _initPosition = rectTransform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        // rectTransform.position = eventData.position;
        rectTransform2.position = eventData.position;

        var local = rectTransform2.localPosition;
        var magnitude = local.magnitude;
        var max = rectTransform.rect.width;
        if (max < magnitude) {
            magnitude = Mathf.Min(max, magnitude);
            rectTransform2.localPosition = local.normalized * magnitude;
        }

        UpdatePosition();
        // _mouseX = _horizontal - _prevHorizontal;
        // _mouseY = _vertical - _prevVertical;
        // _prevHorizontal = _horizontal;
        // _prevVertical = _vertical;
    }

    void LateUpdate() {
        _prevHorizontal = _horizontal;
        _prevVertical = _vertical;
    }

    public void OnEndDrag(PointerEventData eventData) {
    }

    public float GetAxis(string axis_) {
#if UNITY_ANDROID
        if (axis_ == "Horizontal") {
            return _horizontal;
        }

        if (axis_ == "Mouse X") {
            _mouseX = _horizontal - _prevHorizontal;
            return _mouseX;
        }

        if (axis_ == "Vertical") {
            return _vertical;
        }

        if (axis_  == "Mouse Y") {
            _mouseY = _vertical - _prevVertical;
            return _mouseY;
        }
        return 0;
#else
        return Input.GetAxis(axis_);
#endif
    }
}
