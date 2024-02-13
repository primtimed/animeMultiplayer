using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public bool _spectator;

    [Header("Main Settings")]
    public float _speedAcceleration;
    public float _sensetivitie;

    [Header("")]
    public bool _canJump;
    public bool _canSprint;
    public bool _canSlide;
    public bool _canWallrun;
    public bool _canWallClime;

    [Header("Back Settings")]
    public float _walkSpeed;
    [ConditionalHide("_canSprint")] public float _sprintSpeed;
    [ConditionalHide("_canJump")] public float _jumpHight;
    public float _playerHight;
    [ConditionalHide("_canWallrun")] public float _wallrunSpeed;
    [ConditionalHide("_canWallrun")] public float _wallrunAngle;
    [ConditionalHide("_canWallrun")] public float _wallrunMagnet;
    [ConditionalHide("_canWallrun")] public LayerMask _wallrunLayer;
    [ConditionalHide("_canWallClime")] public float _wallclimSpeed;
    [ConditionalHide("_canWallClime")] public float _walclimHight;

    [Header("")]
    public BackSettings _back = new BackSettings();

    public GameObject _orientation, _feet;

    //private
    PlayerControlls _input;
    InputAction _move, _mouse, _sprint, _jump, _slide, _croush;

    float _x, _y;
    float _moveSpeed;
    float _wallRunCameraTilt;
    float _timer;

    bool _grounded, _sprinting, _jumping, _slidding, _chroushing, _wallrunning;
    bool _wallrunL, _wallrunR, _climming, _slideDelay;

    [Serializable]
    public class BackSettings
    {
        [SerializeField] public GameObject _camera;
        [SerializeField] public Rigidbody _rb;
        [SerializeField] public GameObject _center;
        [SerializeField] public bool _grappling, _dash;

        public Vector2 _move;
    }

    public enum Passive
    {
        None,
        WallRun,
        DubbleJump
    }

    public Passive _passive;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    void OnEnable()
    {
        _moveSpeed = _walkSpeed;

        _input.Enable();

        _move = _input.Movement.Movement;
        _mouse = _input.Movement.Rotation;
        _sprint = _input.Movement.Sprint;
        _slide = _input.Movement.Slide;
        _jump = _input.Movement.Jump;
        _croush = _input.Movement.Croush;

        _sprint.started += Sprint;
        _sprint.canceled += Sprint;

        _slide.started += Slide;
        _slide.canceled += Slide;

        _croush.started += Croush;
        _croush.canceled += Croush;

        _jump.started += Jump;

        _jump.started += Wallclime;
        _jump.canceled += Wallclime;
    }

    void OnDisable()
    {
        _input.Disable();
    }

    void Update()
    {
        Move(_move.ReadValue<Vector2>());
        Rotate(_mouse.ReadValue<Vector2>() * Time.deltaTime);

        Grounded();
        WallRunInput();
        WallclimeUpdate();
        CrouchUpdate();
    }

    //movement
    void Move(Vector2 _moveV2)
    {
        _back._move = _moveV2;

        if (_moveV2.y != 0 || _moveV2.x != 0)
        {
            if (_grounded && !_slidding || _jumping || _climming)
            {
                Vector3 _moveDirection = (_back._center.transform.forward * _moveV2.y + _back._center.transform.right * _moveV2.x) * Time.deltaTime;
                _moveDirection = new Vector3 (_moveDirection.x, 0, _moveDirection.z);
                _back._rb.AddForce(_moveDirection * _speedAcceleration);

                if (!_back._grappling || _grounded)
                {
                    SpeedControle();
                }
            }
        }

        else if (_grounded && !_jumping && !_back._grappling && !_slidding && !_wallrunning)
        {
            _back._rb.velocity = new Vector3(0, 0, 0);
        }
    }

    //speedcontroler
    void SpeedControle()
    {
        if (_back._dash)
        {

        }

        else
        {
            Vector3 _speed = new Vector3(_back._rb.velocity.x, 0, _back._rb.velocity.z);

            if (_speed.magnitude > _moveSpeed)
            {
                Vector3 _speedLimited = _speed.normalized * _moveSpeed;
                _back._rb.velocity = new Vector3(_speedLimited.x, _back._rb.velocity.y, _speedLimited.z);
            }
        }
    }

    //camera rotation
    void Rotate(Vector2 _rotateV2)
    {
        float _xB = _rotateV2.x * _sensetivitie;
        float _yB = _rotateV2.y * _sensetivitie;

        _x += _xB;
        _y -= _yB;

        _y = Math.Clamp(_y, -85, 85);

        transform.localRotation = Quaternion.Euler(0, _x, 0);
        _back._camera.transform.localRotation = Quaternion.Euler(_y, 0, 0);
    }

    //jump

    bool _dubbleJump;

    void Jump(InputAction.CallbackContext context)
    {

        if (_canJump)
        {
            if (_grounded & !_jumping)
            {
                _jumping = true;

                _back._rb.AddForce(transform.up * _jumpHight, ForceMode.Impulse);
                StartCoroutine(JumpTime());

                return;
            }

            if (_wallrunning && !_jumping)
            {
                _jumping = true;

                _back._rb.AddForce(transform.up * (_jumpHight * 1.5f), ForceMode.Impulse);
                StartCoroutine(JumpTime());

                return;
            }

            if (_passive == Passive.DubbleJump && _dubbleJump)
            {
                Debug.Log("Dubble");

                Vector3 _moveDirection = (_back._center.transform.forward * _back._move.y + _back._center.transform.right * _back._move.x) * Time.deltaTime;
                _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
                _back._rb.AddForce(_moveDirection * _speedAcceleration * 2.5f);

                _back._rb.AddForce(transform.up * _jumpHight, ForceMode.Impulse);

                _dubbleJump = false;
                return;
            }
        }
    }
    IEnumerator JumpTime()
    {
        yield return new WaitForSeconds(.2f);
        _jumping = false;
    }

    //look for ground
    void Grounded()
    {
        RaycastHit hit;

        if (Physics.SphereCast(_orientation.transform.position, .5f, -transform.up, out hit, _playerHight))
        {
            _grounded = true;
            _dubbleJump = true;
            _timer = 0;
        }

        else if ( !_wallrunning)
        {
            _back._rb.velocity += new Vector3(0, -0.05f, 0);
            _grounded = false;
        }

        else if (!_back._grappling)
        {
            _back._rb.velocity += new Vector3(0, -0.02f, 0);
            _grounded = false;
        }
    }

    //sprint
    void Sprint(InputAction.CallbackContext context)
    {
        if (_canSprint)
        {
            if (context.started)
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
    }

    //slide
    void Slide(InputAction.CallbackContext context)
    {
        //void word niet uitgevoerd met sprint slide naar rechts

        if (_canSlide)
        {
            if (context.started && _slideDelay == false)
            {
                _slidding = true;
                _slideDelay = true;

                //_back._rb.velocity = new Vector3 (_back._rb.velocity.x *2, _back._rb.velocity.y, _back._rb.velocity.z * 2);

                _back._rb.velocity *= 2;
            }

            else if (context.canceled)
            {
                transform.rotation = new quaternion(0, 0, 0, 0);
                _slidding = false;

                StartCoroutine(SlideDelay());
            }
        }
    }

    IEnumerator SlideDelay()
    {
        yield return new WaitForSeconds(1f);
        _slideDelay = false;
    }

    void Croush(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _chroushing = true;
        }

        if (context.canceled)
        {
            _chroushing = false;
        }
    }

    void CrouchUpdate()
    {
        if (_chroushing & _spectator)
        {
            _back._rb.AddForce(-transform.up * (_speedAcceleration / 100));
        }
    }

    //wallrun
    void WallRunInput()
    {
        if (_canWallrun && _passive == Passive.WallRun)
        {
            CheckForWallRun();
            Look();

            if ((_wallrunR || _wallrunL) && !_grounded)
            {
                StartWallRun();
            }
        }
    }

    void StartWallRun()
    {
        _back._rb.useGravity = false;
        _wallrunning = true;

        _back._rb.AddForce(transform.forward * _wallrunSpeed);

        SpeedControle();

        //makes the player stick to a wall
        if (_wallrunR && !_jumping)
        {
            _back._rb.AddForce(_orientation.transform.right * _wallrunMagnet * Time.deltaTime);
        }
        else if (_wallrunL && !_jumping)
        {
            _back._rb.AddForce(-_orientation.transform.right * _wallrunMagnet * Time.deltaTime);
        }
    }

    void StopWallRun()
    {
        _back._rb.useGravity = true;
        _wallrunning = false;
    }

    void CheckForWallRun()
    {
        _wallrunR = Physics.Raycast(_feet.transform.position, transform.right, 1f, _wallrunLayer);
        _wallrunL = Physics.Raycast(_feet.transform.position, -transform.right, 1f, _wallrunLayer);

        //exit the wallrun
        if (!_wallrunL && !_wallrunR)
        {
            StopWallRun();
        }
    }

    void Look()
    {
        //Tilts camera in .5 second
        if (math.abs(_wallRunCameraTilt) < _wallrunAngle && _wallrunning && _wallrunR)
        {
            _wallRunCameraTilt += Time.deltaTime * _wallrunAngle * 2;
        }
        if (math.abs(_wallRunCameraTilt) < _wallrunAngle && _wallrunning && _wallrunL)
        {
            _wallRunCameraTilt -= Time.deltaTime * _wallrunAngle * 2;
        }

        //Tilts camera back again
        if (_wallRunCameraTilt > 0 && !_wallrunL && !_wallrunR)
        {
            _wallRunCameraTilt -= Time.deltaTime * _wallrunAngle * 2;
        }
        if (_wallRunCameraTilt < 0 && !_wallrunL && !_wallrunR)
        {
            _wallRunCameraTilt += Time.deltaTime * _wallrunAngle * 2;
        }

        transform.rotation = Quaternion.Euler(0, _x, _wallRunCameraTilt);

    }

    //Wallcliming

    void Wallclime(InputAction.CallbackContext context)
    {
        if (_canWallClime || _spectator)
        {
            if (context.started)
            {
                _climming = true;
            }

            if (context.canceled)
            {
                _climming = false;
            }
        }
    }

    void WallclimeUpdate()
    {
        RaycastHit hit;

        if (Physics.SphereCast(_feet.transform.position, .1f, _feet.transform.forward, out hit, 1) && _climming && _timer < _walclimHight && !_spectator)
        {
            _timer += Time.deltaTime;
            _back._rb.velocity = new Vector3(_back._rb.velocity.x, _wallclimSpeed, _back._rb.velocity.z);
        }

        else if (_spectator && _climming)
        {
            _back._rb.AddForce(transform.up * (_speedAcceleration / 100));
        }
    }

    //extra
    void OnCollisionStay(Collision collision)
    {
        if (!_grounded || !_wallrunning || !_climming)
        {
            //_back._rb.velocity += new Vector3(0, -1, 0);
        }
    }
}
