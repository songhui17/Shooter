using UnityEngine;
using System.Collections;

public class MouseLook : MonoBehaviour
{
    bool _inited = false;

    Vector3 _lastMousePosition;
    Vector3 lastMousePosition
    {
        get { if (!_inited) { _lastMousePosition = Input.mousePosition; _inited = true; } return _lastMousePosition; }
        set { _lastMousePosition = value; }
    }

    void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        var mousePosition = Input.mousePosition;
        var delta = mousePosition - lastMousePosition;
        var deltaX = delta.x / Screen.width;

        var run = Input.GetKey(KeyCode.LeftShift);
        var rotateSpeed = run ? 100 : 400;
        //transform.Rotate(Vector3.up, deltaX * rotateSpeed * Time.deltaTime, Space.Self);

        lastMousePosition = mousePosition;
    }
}
