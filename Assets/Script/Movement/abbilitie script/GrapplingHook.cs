using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Grapplinghook", menuName = "Grapplinghook")]
public class GrapplingHook : BaseAbillitie
{
    [Header("MainSettings")]
    public float _maxDistance;
    public float _aimAssist;
    public float _spring;
    public float _jumpBoost;
    public float _jumpTime;

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
    PlayerControlls _input;
    InputAction _grappling, _jump;

    Vector3 _currentGrapplePosition;
    bool _grappleJump;

    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player;

        if(!_gunBarrel)
        {
            _gunBarrel = Instantiate(_gun, manager._graplingLoc);
        }

        _lr = _gunBarrel.GetComponent<LineRenderer>();
        _movement = _player.GetComponent<Movement>();
        _camera = _player.GetComponentInChildren<Camera>().gameObject.transform;

        _lr.positionCount = 0;
    }

    void Start() { }

    public override void LateUpdate()
    {
        DrawRope();
    }

    public override void Start(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.SphereCast(_camera.position, _aimAssist, _camera.forward, out hit, _maxDistance, _whatIsGrappleable))
        {
            _grappleJump = true;

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
        _lr.positionCount = 0;
        _gunBarrel.transform.rotation = new quaternion(0,0,0,0);
        _movement._back._grappling = false;
        Destroy(_joint);
    }


    void DrawRope()
    {
        if (!_joint) return;

        _currentGrapplePosition = Vector3.Lerp(_currentGrapplePosition, _grapplePoint, Time.deltaTime * 8f);

        _lr.SetPosition(0, _gunBarrel.transform.position);
        _lr.SetPosition(1, _currentGrapplePosition);

        _movement._back._grappling = true;

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

    void Jump(InputAction.CallbackContext context)
    {
        if (_grappleJump && !_movement._back._grappling)
        {
            _grappleJump = false;

            _movement._back._rb.AddForce(_player.transform.up * _jumpBoost, ForceMode.Impulse);
            _movement._back._rb.AddForce(_player.transform.forward * _jumpBoost, ForceMode.Impulse);
        }
    }
}
