using UnityEngine;
using UnityEngine.InputSystem;

public class Admin : MonoBehaviour
{
    PlayerControlls _input;
    InputAction _action;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _action = _input.Admin.Spectate;

        _action.started += lolll;
    }

    private void OnDisable()
    {
        _input.Disable();

        _action.started -= lolll;
    }

    void lolll(InputAction.CallbackContext context)
    {
        if (!GetComponent<PlayerStats>()) return;

        if (GetComponent<PlayerStats>().IsOwner == false) return;

        if (GetComponent<Movement>()._spectator)
        {
            GetComponent<Movement>()._spectator = false;
        }

        else
        {
            GetComponent<Movement>()._spectator = true;
        }
    }
}
