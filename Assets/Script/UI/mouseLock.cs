using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.SceneManagement;
using UnityEngine.Windows;

public class mouseLock : MonoBehaviour
{
    public static bool _isLocked;
    private PlayerControlls _playerControlls;
    private InputAction _action;

    private void Awake()
    {
        _playerControlls = new PlayerControlls();
    }

    public void OnEnable()
    {
        _action = _playerControlls.UI.MouseLock;
        _action.Enable();
        _action.performed += Switch;
    }

    public void OnDisable()
    {
        _action.Disable();
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;

        //_isLocked = true;
    }

    public void Switch(InputAction.CallbackContext context)
    {
        if (_isLocked == false)
        {
            _isLocked = true;
        }
        else
        {
            _isLocked = false;
        }
    }

    public void LockSwitch()
    {
        if (_isLocked == false)
        {
            _isLocked = true;
        }
        else
        {
            _isLocked = false;
        }
    }

    private void Update()
    {
        if (_isLocked == true)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else if (_isLocked == false)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
