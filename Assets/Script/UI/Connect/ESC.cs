using Unity.Netcode;
using UnityEngine.InputSystem;

public class ESC : NetworkBehaviour
{
    PlayerControlls _input;
    InputAction _esc;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _esc = _input.UI.Esc;

        _esc.started += Disconnect;
    }

    void Disconnect(InputAction.CallbackContext context)
    {
        if (IsHost)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (IsClient)
        {
            NetworkManager.Singleton.Shutdown();
        }
        else if (IsServer)
        {
            NetworkManager.Singleton.Shutdown();
        }
    }
}
