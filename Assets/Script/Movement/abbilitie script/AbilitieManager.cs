using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitieManager : NetworkBehaviour
{
    PlayerControlls _input;
    InputAction _abbilitieKey1;

    public BaseAbillitie _abbilitie;
    public BasePassive _passive;

    public Transform _graplingLoc;

    GameObject _network;
    GameObject _keep;

    public void start()
    {
        _input = new PlayerControlls();

        _network = gameObject;
        _keep = GameObject.Find("Keep");


        onenable();
    }

    private void onenable ()
    {
        _input.Enable();

        _abbilitieKey1 = _input.Abbilities._1;

        _abbilitieKey1.started += _abbilitie.Start;
        _abbilitieKey1.canceled += _abbilitie.Stop;

        _abbilitie.Open(gameObject, this, _network, _keep);
        _passive.Open(gameObject);
    }

    private void OnDisable()
    {
        _abbilitieKey1.started -= _abbilitie.Start;
        _abbilitieKey1.canceled -= _abbilitie.Stop;
    }

    private void LateUpdate()
    {
        if (_abbilitie)
        {
            _abbilitie.LateUpdate();
        }
    }

    private void Update()
    {
        if (_abbilitie)
        {
            _abbilitie.Update();
        }
    }
}
