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

        Debug.Log("sp0");
    }

    private void OnDisable()
    {
        _input.Disable();

        _action.started -= lolll;
    }

    void lolll(InputAction.CallbackContext context)
    {
        Debug.Log("sp1");

        if (GetComponent<PlayerStats>().IsOwner == false) return;

        Debug.Log("sp2");

        if (GetComponent<Movement>()._spectator)
        {
            GetComponent<Movement>()._spectator = false;

            Debug.Log("sp3");
        }

        else
        {
            GetComponent<Movement>()._spectator = true;

            Debug.Log("sp4");
        }
    }
}
