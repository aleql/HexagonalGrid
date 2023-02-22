using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField] private PlayerInput _cameraActions;
    private InputAction _cameraMovement;
    private InputAction _cameraRotation;
    private InputAction _cameraZoom;

    [SerializeField] private Transform _cameraTransform;

    [SerializeField] private float _maxSpeed = 5f;
    private float _speed;
    [SerializeField] private float _acceleration = 10f;
    [SerializeField] private float _damping = 15f;
    [SerializeField] private float _stepSize = 2f;
    [SerializeField] private float _zoomDampening = 7.5f;
    [SerializeField] private float _minHeight = 5f;
    [SerializeField] private float _maxHeight = 50f;
    [SerializeField] private float _zoomSpeed = 2f;
    [SerializeField] private float _maxRotationSpeed = 1f;



    [SerializeField]
    [Range(0f, 0.1f)]
    private float _edgeTolerance = 0.05f;

    //value set in various functions 
    //used to update the position of the camera base object.
    private Vector3 _targetPosition;

    private float _zoomHeight;

    //used to track and maintain velocity w/o a rigidbody
    private Vector3 _horizontalVelocity;
    private Vector3 _lastPosition;

    private bool _controlsEnabled;

    private void Awake()
    {
        if (_cameraActions == null)
        {
            _cameraActions = GetComponent<PlayerInput>();
        }
        if (_cameraTransform == null)
        {
            _cameraTransform = GetComponentInChildren<Camera>().transform;
        }
        _controlsEnabled = false;
    }
    public void EnableControllers()
    {
        _zoomHeight = _cameraTransform.localPosition.y;
        _cameraTransform.LookAt(transform);
        _lastPosition = transform.position;

        _cameraMovement = _cameraActions.actions["cameraMovement"];
             
        _cameraRotation = _cameraActions.actions["cameraRotation"];
        _cameraRotation.performed += _cameraRotation_performed;

        _cameraZoom = _cameraActions.actions["cameraZoom"];
        _cameraZoom.performed += _cameraZoom_performed;

        _controlsEnabled = true;
    }

    private void OnDisable()
    {
        _cameraRotation.performed -= _cameraRotation_performed;
        _cameraZoom.performed -= _cameraZoom_performed;
    }

    private void Update()
    {
        if (_controlsEnabled)
        {
            GetKeyboardMovement();
            CheckMouseAtScreenEdge();
            UpdateVelocity();
            UpdateBasePosition();
            UpdateCameraPosition();
            UpdateCameraBound();
        }
    }

    private void UpdateVelocity()
    {
        _horizontalVelocity = (transform.position - _lastPosition) / Time.deltaTime;
        _horizontalVelocity.y = 0;
        _lastPosition = transform.position;
    }

    private void GetKeyboardMovement()
    {
        Vector3 inputValue = _cameraMovement.ReadValue<Vector2>().x * GetCameraRight() +
            _cameraMovement.ReadValue<Vector2>().y * GetCameraForward();
        inputValue = inputValue.normalized;

        // Is actually reading relevant values
        if(inputValue.sqrMagnitude > 0.1f)
        {
            _targetPosition += inputValue;
        }
    }
    private Vector3 GetCameraRight()
    {
        Vector3 rightDirection = _cameraTransform.right;
        rightDirection.y = 0;
        return rightDirection;
    }

    private Vector3 GetCameraForward()
    {
        Vector3 forwardDirection = _cameraTransform.forward;
        forwardDirection.y = 0;
        return forwardDirection;
    }

    private void UpdateBasePosition()
    {
        // Accelerate
        if(_targetPosition.sqrMagnitude > 0.1f)
        {
            _speed = Mathf.Lerp(_speed, _maxSpeed, Time.deltaTime * _acceleration);
            transform.position += _targetPosition * _speed * Time.deltaTime;
        }
        // Decelerate
        else
        {
            _horizontalVelocity = Vector3.Lerp(_horizontalVelocity, Vector3.zero, Time.deltaTime * _damping);
            transform.position += _targetPosition * Time.deltaTime;
        }

        _targetPosition = Vector3.zero;
    }


    private void _cameraRotation_performed(InputAction.CallbackContext inputValue)
    {
        if (Mouse.current.middleButton.isPressed)
        {
            float value = inputValue.ReadValue<Vector2>().x;
            transform.rotation = Quaternion.Euler(0f, value * _maxRotationSpeed + transform.rotation.eulerAngles.y, 0f);
        }
    }
    private void _cameraZoom_performed(InputAction.CallbackContext inputValue)
    {
        float zoomValue = -inputValue.ReadValue<Vector2>().y / 100f;
        if(!Mathf.Approximately(Mathf.Abs(zoomValue), 0.0f))
        {
            _zoomHeight = _cameraTransform.localPosition.y + zoomValue * _stepSize;
            if(_zoomHeight < _minHeight)
            {
                _zoomHeight = _minHeight;
            }
            else if (_zoomHeight > _maxHeight)
            {
                _zoomHeight = _maxHeight;
            }
        }
    }

    private void CheckMouseAtScreenEdge()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Vector3 moveDirection = Vector3.zero;

        if((mousePosition.x < _edgeTolerance * Screen.width))
        {
            moveDirection += -GetCameraRight();
        }
        else if(mousePosition.x > (1f - _edgeTolerance) * Screen.width)
        {
            moveDirection += GetCameraRight();
        }
        if (mousePosition.y < _edgeTolerance * Screen.height)
        {
            moveDirection += -GetCameraForward();
        }
        else if (mousePosition.y > (1f - _edgeTolerance) * Screen.height)
        {
            moveDirection += GetCameraForward();
        }
        _targetPosition += moveDirection;
    }

    private void UpdateCameraPosition()
    {
        Vector3 zoomTarget = new Vector3(_cameraTransform.localPosition.x, _zoomHeight, _cameraTransform.localPosition.z);
        zoomTarget -= _zoomSpeed * (_zoomHeight - _cameraTransform.localPosition.y) * Vector3.forward;

        _cameraTransform.localPosition = Vector3.Lerp(_cameraTransform.localPosition, zoomTarget, Time.deltaTime * _zoomDampening);
        _cameraTransform.LookAt(transform);
    }

    private void UpdateCameraBound()
    {
        if (transform.position.x > 20.0f)
        {
            transform.position = new Vector3(20.0f, transform.position.y, transform.position.z);
        }
        if (transform.position.x < -20.0f)
        {
            transform.position = new Vector3(-20.0f, transform.position.y, transform.position.z);
        }
        if (transform.position.z > 20.0f)
        {
            transform.position = new Vector3(transform.position.z, transform.position.y, 20.0f);
        }
        if (transform.position.z < -20.0f)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y, -20.0f);
        }
    }
}
