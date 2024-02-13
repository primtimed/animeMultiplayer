using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Wall", menuName = "Wall")]
public class Wall : BaseAbillitie
{
    public Transform _player;

    public GameObject _wall;

    ToServer _serverV;
    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player.transform;
        _serverV = keep.AddComponent<ToServer>();
        _serverV.OpenWall(this);
    }

    void Start() { }

    public override void Start(InputAction.CallbackContext context)
    {
        //alles werkt maar void word niet aangeroepen op een manier als join persoon
        _serverV.spawnWallServerRpc();

        if (_serverV)
        {
            Debug.Log(_serverV.gameObject.name);
        }
    }

    public override void Stop(InputAction.CallbackContext context)
    {

    }

    public override void LateUpdate()
    {

    }
}