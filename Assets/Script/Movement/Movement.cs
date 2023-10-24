using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : NetworkBehaviour
{
    [Header("Main Settings")]
    public float _moveSpeed;
    public float _sensetivitie;

    [Header("Extra Settings")]
    public float _sprintSpeed;
    public float _walkSpeed;
    public float _jumpHight;
    public float _playerHight;

    [Header("")]
    public BackSettings _back = new BackSettings();

    //private
    PlayerControlls _input;
    InputAction _move, _mouse, _sprint, _jump;

    float _x, _y;

    bool _grounded, _sprinting, _jumping;

    [Serializable]
    public class BackSettings
    {
        [SerializeField] public GameObject _camera;
        [SerializeField] public Rigidbody _rb;
        [SerializeField] public GameObject _center;
    }

    void Awake()
    {
        if (!IsOwner)
        {
            GameObject.Find("Keep").GetComponent<GameUI>()._rb = _back._rb;
            GameObject.Find("Keep").GetComponent<GameUI>().StartAll();

            _input = new PlayerControlls();
        }
    }

    void OnEnable()
    {
        _moveSpeed = _walkSpeed;

        _input.Enable();

        _move = _input.Movement.Movement;
        _mouse = _input.Movement.Rotation;
        _sprint = _input.Movement.Sprint;
        _jump = _input.Movement.Jump;

        _jump.started += Jump;

        _sprint.started += Sprint;
        _sprint.canceled += Sprint;
    }

    void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        Move(_move.ReadValue<Vector2>());
        Rotate(_mouse.ReadValue<Vector2>());

        Grounded();
        SpeedControle();
    }

    void Move(Vector2 _moveV2)
    {
        if (_moveV2.y != 0 || _moveV2.x != 0 && _grounded)
        {
            Vector3 _moveDirection = _back._center.transform.forward * -_moveV2.x + _back._center.transform.right * _moveV2.y;
            _back._rb.AddForce(_moveDirection * _moveSpeed, ForceMode.Force);
        }

        else if (_grounded)
        {
            _back._rb.velocity = new Vector3(0, 0, 0);
        }
    }

    void Rotate(Vector2 _rotateV2)
    {
        float _xB = _rotateV2.x * _sensetivitie * Time.deltaTime;
        float _yB = _rotateV2.y * _sensetivitie * Time.deltaTime;

        _x += _xB;
        _y -= _yB;

        _y = Math.Clamp(_y, -85, 85);

        transform.localRotation = Quaternion.Euler(0, _x, 0);
        _back._camera.transform.localRotation = Quaternion.Euler(_y, 0, 0);
    }

    void SpeedControle()
    {
        Vector3 _speed = new Vector3(_back._rb.velocity.x, 0, _back._rb.velocity.z);

        if (_speed.magnitude > _moveSpeed)
        {
            Vector3 _speedLimited = _speed.normalized * _moveSpeed;
            _back._rb.velocity = new Vector3(_speedLimited.x, _back._rb.velocity.y, _speedLimited.z);
        }
    }

    void Grounded()
    {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.up, out hit, _playerHight))
        {
            _grounded = true;
        }

        else
        {
            _back._rb.velocity += new Vector3(0, -0.05f, 0);
            _grounded = false;
        }
    }

    void Jump(InputAction.CallbackContext context)
    {
        if (_grounded)
        {
            _back._rb.AddForce(transform.up * _jumpHight, ForceMode.Impulse);
        }
    }

    void Sprint(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            _sprinting = true;
            _moveSpeed = _sprintSpeed;
        }

        else if (context.canceled)
        {
            _sprinting = false;
            _moveSpeed = _walkSpeed;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!_grounded)
        {
            _back._rb.velocity += new Vector3(0, -1, 0);
        }
    }
}
