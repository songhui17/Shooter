using UnityEngine;
using System.Collections;

public class Motor : MonoBehaviour
{
    #region Fields

    [SerializeField]
    private CharacterController _controller;
    
    #endregion

    #region MonoBehaviors
    
    void Update()
    {
        UpdateMovement();
    }

    #endregion

    #region Methods

    void UpdateMovement()
    {
        var forwardSpeed = 0;

        var run = Input.GetKey(KeyCode.LeftShift);
        if (Input.GetKey(KeyCode.W))
        {
            forwardSpeed += run ? 4 : 1;
        }

        if (Input.GetKey(KeyCode.S))
        {
            forwardSpeed -= run ? 4 : 1;
        }

        var rotateSpeed = 0;
        if (Input.GetKey(KeyCode.Q))
        {
            rotateSpeed -= run ? 100 : 40;
        }

        if (Input.GetKey(KeyCode.E))
        {
            rotateSpeed += run ? 100 : 40;
        }

        transform.Rotate(Vector3.up, rotateSpeed * Time.deltaTime, Space.Self);

        var movement = transform.forward * forwardSpeed * Time.deltaTime;
        _controller.Move(movement);
    }

    #endregion
}
