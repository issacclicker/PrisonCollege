<<<<<<< Updated upstream
using System.Collections;
=======
ï»¿using System.Collections;
>>>>>>> Stashed changes
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _gravity = -9.81f;

    [Header("Sprint")]
    [SerializeField] private float _sprintMultiplier = 1.7f;

    [Header("Mouse Look")]
    [SerializeField] private float _mouseSensitivity = 100f;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private float _maxLookAngle = 80f;

    private CharacterController _controller;
    private Vector3 _velocity;
    private float _xRotation = 0f;
    private float _speedRate = 1;



    private void Start()
    {
        _controller = GetComponent<CharacterController>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }



    private void Update()
    {
        HandleMouseLook();
        HandleMovement();
    }



    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * _mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * _mouseSensitivity;

        _xRotation -= mouseY;
        _xRotation = Mathf.Clamp(_xRotation, -_maxLookAngle, _maxLookAngle);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
        transform.Rotate(Vector3.up * mouseX);
    }



    private void HandleMovement()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

<<<<<<< Updated upstream
        // Shift ÀÔ·Â °¨Áö
=======
        // Shift ë‹¬ë¦¬ê¸°
>>>>>>> Stashed changes
        bool isSprinting = Input.GetKey(KeyCode.LeftShift);

        float currentSpeed = isSprinting
            ? _moveSpeed * _sprintMultiplier
            : _moveSpeed;

        Vector3 move = transform.right * x + transform.forward * z;
        _controller.Move(move * currentSpeed * Time.deltaTime * _speedRate);

<<<<<<< Updated upstream
        // Áß·Â Ã³¸®
=======
        // ì¤‘ë ¥
>>>>>>> Stashed changes
        if (_controller.isGrounded && _velocity.y < 0)
            _velocity.y = -2f;

        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }
<<<<<<< Updated upstream
}
=======
}
>>>>>>> Stashed changes
