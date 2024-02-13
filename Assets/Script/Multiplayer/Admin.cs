using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class Admin : MonoBehaviour
{
    public bool _admin;

    GameObject _player;

    [SerializeField] GameObject _console;

    PlayerControlls _input;
    InputAction _commands;

    private void Awake()
    {
        if (!_admin)
        {
            enabled = false;
        }

        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _commands = _input.Admin.Console;

        _commands.started += Console;
    }

    private void OnDisable()
    {
        _input.Disable();
    }

    private void Start()
    {
        Instantiate(_console);
    }

    void Console(InputAction.CallbackContext context)
    {
        if (GameObject.FindWithTag("Player").GetComponent<OwnerCheck>().IsOwner)
        {
            _player = GameObject.FindWithTag("Player");
        }
    }
}
