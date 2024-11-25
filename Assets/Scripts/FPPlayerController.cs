using System;
using System.Collections.Generic;
using UnityEngine;

public class FPPlayerController : BasePlayerController
{
    [Header("Movement")]
    [Tooltip("Basic controller speed")]
    [SerializeField] private float walkSpeed;
    
    [Tooltip("Running controller speed")]
    [SerializeField] private float runMultiplier;

    [Tooltip("Force of the jump with which the controller rushes upwards")]
    [SerializeField] private float jumpForce;

    [Tooltip("Gravity, pushing down controller when it jumping")]
    [SerializeField] private float gravity = -9.81f;

    [Header("Mouse Look")] 
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float mouseSensivity;
    [SerializeField] private float mouseVerticalClamp;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode runKey = KeyCode.LeftShift;

    
    //Private movement variables
    private float _horizontalMovement;
    private float _verticalMovement;
    private float _currentSpeed;
    private Vector3 _moveDirection;
    private Vector3 _velocity;
    private CharacterController _characterController;
    private bool _isRunning;
    
    //Private mouselook variables
    private float _verticalRotation;
    private float _yAxis;
    private float _xAxis;
    private bool _activeRotation;
    
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        Movement();
        MouseLook();
    }

    //Character controller movement
    private void Movement()
    {
        if (_characterController.isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        
        if (Input.GetKey(jumpKey) && _characterController.isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        }
        
        _horizontalMovement = Input.GetAxis("Horizontal");
        _verticalMovement = Input.GetAxis("Vertical");

        _moveDirection = transform.forward * _verticalMovement + transform.right * _horizontalMovement;
        
        _isRunning = Input.GetKey(runKey);
        _currentSpeed = walkSpeed * (_isRunning ? runMultiplier : 1f);
        _characterController.Move(_moveDirection * _currentSpeed * Time.deltaTime);

        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);

    }

    private void MouseLook()
    {   
        _xAxis = Input.GetAxis("Mouse X"); 
        _yAxis = Input.GetAxis("Mouse Y");

        _verticalRotation += -_yAxis * mouseSensivity;
        _verticalRotation = Mathf.Clamp(_verticalRotation, -mouseVerticalClamp, mouseVerticalClamp);
        playerCamera.transform.localRotation = Quaternion.Euler(_verticalRotation, 0, 0);
        transform.rotation *= Quaternion.Euler(0, _xAxis * mouseSensivity, 0);
    }
}

//Variables for footstep system list
[System.Serializable]
public class GroundLayer
{
    public string layerName;
    public Texture2D[] groundTextures;
    public AudioClip[] footstepSounds;
}
