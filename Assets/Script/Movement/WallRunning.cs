using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.VersionControl.Message;
using static UnityEngine.GraphicsBuffer;

public class WallRunning : MonoBehaviour
{
    public LayerMask _wallRunLayer;
    public Movement _movement;
    public float _wallrunAngle;
    public GameObject _feet;
    Rigidbody _rigidbody;

    RaycastHit _hitL;
    RaycastHit _hitR;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _movement = GetComponent<Movement>();
    }

    private void FixedUpdate()
    {
        if (Physics.Raycast(_feet.transform.position, -_feet.transform.right, out _hitL, 1.5f, _wallRunLayer))
        {
            StartWallRunning();
        }

        else if (Physics.Raycast(_feet.transform.position, _feet.transform.right, out _hitR, 1.5f, _wallRunLayer))
        {
            StartWallRunning();
        }

        else
        {
            StopWallRunning();
        }
    }

    void StartWallRunning()
    {
        _movement._back._wallrunning = true;
        _rigidbody.useGravity = false;
    }

    void StopWallRunning()
    {
        _movement._back._wallrunning = false;
        _rigidbody.useGravity = true;
    }
}
