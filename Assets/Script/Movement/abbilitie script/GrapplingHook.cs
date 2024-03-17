using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Grapplinghook", menuName = "Grapplinghook")]
public class GrapplingHook : BaseAbillitie
{
    public Texture _icon;

    [Header("MainSettings")]
    public float _maxDistance;
    public float _aimAssist;
    public float _spring;

    [Header("")]
    public LayerMask _whatIsGrappleable;
    public GameObject _gun;

    GameObject _player;
    GameObject _gunBarrel;

    LineRenderer _lr;
    SpringJoint _joint;

    Vector3 _grapplePoint;
    Transform _camera;

    Movement _movement;
    GameUI _gameUI;

    Vector3 _currentGrapplePosition;

    Image _crosseair;

    public float _cooldown;
    float _timer;

    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player;

        if (!_gunBarrel)
        {
            _gunBarrel = Instantiate(_gun, manager._graplingLoc);
        }

        _lr = _gunBarrel.GetComponent<LineRenderer>();
        _movement = _player.GetComponent<Movement>();
        _camera = _player.GetComponentInChildren<Camera>().gameObject.transform;
        _gameUI = player.GetComponent<GameUI>();

        _crosseair = _player.GetComponentInChildren<Image>();

        _lr.positionCount = 0;

        _gameUI._abbilIcon.texture = _icon;
    }

    void Start() { }

    public override void Update()
    {
        if (_movement)
        {
            if (!_movement._back._grappling)
            {
                _timer -= Time.deltaTime;
            }

            _gameUI._time = _timer;
        }
    }

    public override void LateUpdate()
    {
        if (!_movement) { return; }

        RaycastHit hit1;

        if (Physics.SphereCast(_camera.position, _aimAssist, _camera.forward, out hit1, _maxDistance, _whatIsGrappleable))
        {
            _crosseair.color = Color.cyan;
        }

        else
        {
            _crosseair.color = Color.red;
        }

        GrapplingSlide();

        DrawRope();
    }


    public override void Start(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.SphereCast(_camera.position, _aimAssist, _camera.forward, out hit, _maxDistance, _whatIsGrappleable) && _timer <= 0)
        {
            _currentGrapplePosition = _gunBarrel.transform.position;

            _grapplePoint = hit.point;
            _joint = _player.transform.gameObject.AddComponent<SpringJoint>();
            _joint.autoConfigureConnectedAnchor = false;
            _joint.connectedAnchor = _grapplePoint;

            float distanceFromPoint = Vector3.Distance(_player.transform.position, _grapplePoint);

            _joint.maxDistance = distanceFromPoint * .2f;
            _joint.minDistance = distanceFromPoint * .1f;

            _joint.spring = _spring;
            _joint.damper = 2;
            _joint.massScale = 2;

            _lr.positionCount = 2;
        }
    }

    public override void Stop(InputAction.CallbackContext context)
    {
        if (_movement._back._grappling)
        {
            Destroy(_player.GetComponent<SpringJoint>());

            _lr.positionCount = 0;
            _gunBarrel.transform.rotation = new quaternion(0, 0, 0, 0);
            _movement._back._grappling = false;
            _timer = _cooldown;
        }
    }

    void GrapplingSlide()
    {
        Vector3 _speed = new Vector3(_movement._back._rb.velocity.x, 0, _movement._back._rb.velocity.z);

        if (_movement._back._grapplingSlide && _speed.magnitude < _movement._sprintSpeed)
        {
            _movement._speedAcceleration = 100000;
            _movement._back._grapplingSlide = false;
        }
    }


    void DrawRope()
    {
        if (!_joint) return;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _grapplePoint, Time.deltaTime * 8f);
        _movement._back._rb.drag = 0;

        _lr.SetPosition(0, _gunBarrel.transform.position);
        _lr.SetPosition(1, _currentGrapplePosition);

        _movement._back._grappling = true;
        _movement._back._grapplingSlide = true;
        _movement._speedAcceleration = 0;

        float distanceFromPoint = Vector3.Distance(_player.transform.position, _grapplePoint);

        _joint.maxDistance = distanceFromPoint * .9f;
        _joint.minDistance = distanceFromPoint * .0f;

        _gunBarrel.transform.LookAt(_currentGrapplePosition);
    }

    bool IsGrappling()
    {
        return _joint != null;
    }

    Vector3 GetGrapplePoint()
    {
        return _grapplePoint;
    }
}
