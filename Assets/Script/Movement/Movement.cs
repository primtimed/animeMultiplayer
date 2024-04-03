using System;
using System.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public enum Passive
{
    None,
    WallRun,
    DubbleJump
}
public class Movement : NetworkBehaviour
{
    public bool _spectator;

    [Header("Main Settings")]
    public float _speedAcceleration;
    public float _sensetivitie;
    [HideInInspector] public float _gameSens;

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
    public float maxSlopeAngle;
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

    [HideInInspector] public float _x, _y;
    float _moveSpeed;
    float _wallRunCameraTilt;
    float _timer;

    [HideInInspector] public bool _grounded, _sprinting, _jumping, _slidding, _chroushing, _wallrunning;
    bool _wallrunL, _wallrunR, _climming, _slideDelay, _wall;

    [Serializable]
    public class BackSettings
    {
        [SerializeField] public GameObject _camera;
        [SerializeField] public Rigidbody _rb;
        [SerializeField] public GameObject _center;
        [SerializeField] public bool _grappling, _dash, _grapplingSlide;

        public Vector2 _move;
        public Vector2 _rot;
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

    private void Start()
    {
        _sensetivitie = PlayerPrefs.GetFloat("Sens");

        _back._camera.GetComponent<Camera>().fieldOfView = PlayerPrefs.GetFloat("FOV");

        _gameSens = _sensetivitie;
    }

    void Update()
    {
        if (!IsLocalPlayer) return;

        Move(_move.ReadValue<Vector2>());
        Rotate(_mouse.ReadValue<Vector2>() * Time.smoothDeltaTime);

        Grounded();
        CrouchUpdate();
        WallCheck();
        WallRunInput();
        WallclimeUpdate();
    }

    //movement
    Vector3 _moveDirection;
    void Move(Vector2 _moveV2)
    {
        _back._move = _moveV2;

        if (_moveV2.y != 0 || _moveV2.x != 0 && !_wall)
        {
            if (OnSlope())
            {
                _back._rb.AddForce(_moveDirection * _speedAcceleration);
            }

            else if (_grounded && !_slidding || _jumping || _climming || _spectator)
            {
                _moveDirection = (_back._center.transform.forward * _moveV2.y + _back._center.transform.right * _moveV2.x) * Time.deltaTime;
                _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);

                _back._rb.AddForce(_moveDirection * _speedAcceleration);

                if (!_back._grappling || _grounded)
                {
                    SpeedControle();
                }
            }
        }

        else if (_grounded && !_jumping && !_back._grappling && !_slidding && !_wallrunning)
        {
            _back._rb.velocity = new Vector3(0, _back._rb.velocity.y, 0);
        }
    }

    //speedcontroler
    void SpeedControle()
    {
        if (_back._dash || _back._grapplingSlide)
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

    //slope 
    RaycastHit hit;
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out hit, _playerHight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, hit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }


    //camera rotation
    void Rotate(Vector2 _rotateV2)
    {
        float _xB = _rotateV2.x * _gameSens;
        float _yB = _rotateV2.y * _gameSens;

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
        if (!IsLocalPlayer) return;

        if (_canJump && !_jumping)
        {
            _jumping = true;
            StartCoroutine(JumpTime());

            if (_grounded)
            {
                _back._rb.AddForce(transform.up * _jumpHight, ForceMode.Impulse);

                return;
            }

            if (_wallrunning)
            {
                if (_wallrunL)
                {
                    _back._rb.AddForce(transform.right * (_jumpHight * 2.5f), ForceMode.Impulse);
                }

                else if (_wallrunR)
                {
                    _back._rb.AddForce(-transform.right * (_jumpHight * 2.5f), ForceMode.Impulse);
                }

                _back._rb.AddForce(transform.up * (_jumpHight * 1.5f), ForceMode.Impulse);

                return;
            }

            if (_passive == Passive.DubbleJump && _dubbleJump)
            {
                Vector3 _moveDirection = (_back._center.transform.forward * _back._move.y + _back._center.transform.right * _back._move.x) * Time.deltaTime;
                _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
                _back._rb.AddForce(_moveDirection * _speedAcceleration / 10 * _back._rb.velocity.magnitude);
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

        if (_back._dash) return;
        if (_spectator) return;

        if (Physics.SphereCast(_orientation.transform.position, .5f, -transform.up, out hit, _playerHight))
        {
            _grounded = true;
            _dubbleJump = true;
            _timer = 0;
            return;
        }

        else
        {
            if (_back._move.y > 0 && !_back._grappling && !_wall)
            {
                Vector3 _strafeDiraction = (_back._rb.transform.forward * _back._move.y * 10 + _back._rb.transform.right * _back._move.x * 10) * Time.deltaTime;
                _strafeDiraction = new Vector3(_strafeDiraction.x, 0, _strafeDiraction.z);
                _back._rb.velocity += _strafeDiraction;
            }

            _back._rb.velocity += new Vector3(0, -0.05f, 0);
            _grounded = false;
        }
    }

    void WallCheck()
    {
        RaycastHit _wallhit;

        Vector3 _diraction = (_back._rb.transform.forward * _back._move.y + _back._rb.transform.right * _back._move.x);

        if (Physics.Raycast(transform.position, _diraction, out _wallhit, 1))
        {
            _wall = true;
        }

        else { _wall = false; }
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

        Vector3 _speed = new Vector3(_back._rb.velocity.x, 0, _back._rb.velocity.z);
        float _WallSpeed = math.lerp(_speed.magnitude, _wallrunSpeed, 5);

        _back._rb.AddForce(transform.forward * _WallSpeed);

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
        if (_spectator) return;

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

        if (Physics.SphereCast(_feet.transform.position, .1f, _feet.transform.forward, out hit, 2) && _climming && _timer < _walclimHight && !_spectator)
        {
            _timer += Time.deltaTime;
            _back._rb.velocity = new Vector3(_back._rb.velocity.x, _wallclimSpeed, _back._rb.velocity.z);
        }

        else if (_spectator && _climming)
        {
            _back._rb.AddForce(transform.up * (_speedAcceleration / 100));
        }

        else
        {
            _climming = false;
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
