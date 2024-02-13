using UnityEngine;
using UnityEngine.InputSystem;

public class BaseAbillitie : ScriptableObject
{
    public virtual void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {

    }

    void Start() { }

    public virtual void Start(InputAction.CallbackContext context)
    {

    }
    public virtual void Stop(InputAction.CallbackContext context)
    {

    }

    public virtual void LateUpdate()
    {

    }

    public virtual void Update()
    {

    }
}
