using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    public float maxDistance;
    public LayerMask whatIsGrappleable;
    public Transform gunTip, camera, player;

    LineRenderer lr;
    Vector3 grapplePoint;
    SpringJoint joint;

    Movement _movement;

    PlayerControlls _input;
    InputAction _grappling;

    void Awake()
    {
        _input = new PlayerControlls();

        lr = GetComponent<LineRenderer>();
        _movement = GetComponent<Movement>();
    }

    private void OnEnable()
    {
        _input.Enable();

        _grappling = _input.Weapon.Grappling;

        _grappling.started += StartGrapple;
        _grappling.canceled += StopGrapple;
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    void LateUpdate()
    {
        DrawRope();
    }

    void StartGrapple(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.SphereCast(camera.position, 1.5f , camera.forward, out hit, maxDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            joint = player.gameObject.AddComponent<SpringJoint>();
            joint.autoConfigureConnectedAnchor = false;
            joint.connectedAnchor = grapplePoint;

            float distanceFromPoint = Vector3.Distance(player.position, grapplePoint);

            joint.maxDistance = distanceFromPoint * 0.8f;
            joint.minDistance = distanceFromPoint * 0.25f;

            joint.spring = 10f;
            joint.damper = 10f;
            joint.massScale = 10f;

            lr.positionCount = 2;
            currentGrapplePosition = gunTip.position;
        }
    }

    void StopGrapple(InputAction.CallbackContext context)
    {
        StartCoroutine(StopSlide());

        lr.positionCount = 0;
        Destroy(joint);
    }

    private Vector3 currentGrapplePosition;

    void DrawRope()
    {
        if (!joint) return;

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, Time.deltaTime * 8f);

        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, currentGrapplePosition);

        _movement._back._grappling = true;
    }

    bool IsGrappling()
    {
        return joint != null;
    }

    Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    IEnumerator StopSlide()
    {
        yield return new WaitForSeconds(.4f);
        _movement._back._grappling = false;
    }
}
