using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Dead : MonoBehaviour
{
    PlayerControlls _input;
    InputAction _respawnButton;

    [HideInInspector] public Movement _movement;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _coll;
    [HideInInspector] public MeshRenderer _mash;
    [HideInInspector] public BaseGun _gun;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _respawnButton = _input.Movement.Jump;

        _respawnButton.started += Respawn;
    }

    private void OnDisable()
    {
        _input.Disable();

        _respawnButton.started -= Respawn;
    }

    void Respawn(InputAction.CallbackContext context)
    {
        GetComponentInParent<PlayerStats>().Alive();
        GetComponentInParent<PlayerStats>().SetSpawnClientRpc();

        gameObject.SetActive(false);
    }
}
