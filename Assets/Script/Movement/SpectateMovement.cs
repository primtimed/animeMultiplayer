using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder;
using UnityEngine.Windows;
using UnityEngine.EventSystems;
using TMPro;

public class SpectateMovement : MonoBehaviour
{
    [Header("Main Settings")]
    public float _speedAcceleration;
    public float _sensetivitie;

    [Header("Back Settings")]
    public float _walkSpeed;
    public float _sprintSpeed;

    float _speed, _x, _y;
    bool _flyUp, _fluDown, _sprinting;

    Rigidbody _rb;

    PlayerControlls _playerControlls;
    InputAction _move, _mouse, _space, _croush, _sprint;

    void Awake()
    {
        _playerControlls = new PlayerControlls();
    }

    void OnEnable()
    {
        _playerControlls.Enable();

        _move = _playerControlls.Movement.Movement;
        _mouse = _playerControlls.Movement.Rotation;
        _sprint = _playerControlls.Movement.Sprint;
        _space = _playerControlls.Movement.Jump;
        _croush = _playerControlls.Movement.Croush;
    }

    void OnDisable()
    {
        _playerControlls.Disable();
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        _speed = _walkSpeed;
    }

    void Update()
    {
        if(_flyUp)
        {
            _rb.AddForce(Vector3.up * _speedAcceleration);
        }

        if(_fluDown)
        {
            _rb.AddForce(-Vector3.up * _speedAcceleration);

        }
    }

    void Move(Vector2 _move)
    {
        Vector3 _moveDirection = (transform.forward * _move.y + transform.right * _move.x) * Time.deltaTime;
        _rb.AddForce(_moveDirection * _speedAcceleration);

        SpeedControle();
    }

    //speedcontroler
    void SpeedControle()
    {
        Vector3 _nowSpeed = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        if (_nowSpeed.magnitude > _speed)
        {
            Vector3 _speedLimited = _nowSpeed.normalized * _speed;
            _rb.velocity = new Vector3(_speedLimited.x, _rb.velocity.y, _speedLimited.z);
        }
    }

    void Rotation(Vector2 _rotateV2)
    {
        float _xB = _rotateV2.x * _sensetivitie;
        float _yB = _rotateV2.y * _sensetivitie;

        _x += _xB;
        _y -= _yB;

        //_y = Math.Clamp(_y, -85, 85);

        transform.localRotation = Quaternion.Euler(0, _x, 0);
        transform.localRotation = Quaternion.Euler(_y, 0, 0);
    }
}
