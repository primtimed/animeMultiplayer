using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Wall", menuName = "Wall")]
public class Wall : BaseAbillitie
{
    public Texture _icon;

    Transform _player;
    GameUI _gameUI;

    public GameObject _wall;

    GameObject _spawnedObject;

    public float _cooldown;
    float _timer;
    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player.transform;
        _gameUI = _player.GetComponent<GameUI>();

        _gameUI._abbilIcon.texture = _icon;
    }

    void Start() { }

    public override void Update()
    {
        if (_gameUI)
        {
            _timer -= Time.deltaTime;

            _gameUI._time = _timer;
        }
    }

    public override void Start(InputAction.CallbackContext context)
    {
        SpawnServerRpc();
    }

    [ServerRpc]
    void SpawnServerRpc()
    {
        if (_spawnedObject != null)
        {
            Destroy(_spawnedObject);
        }

        _spawnedObject = Instantiate(_wall, _player);
        var instanceNetworkObject = _spawnedObject.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    public override void Stop(InputAction.CallbackContext context)
    {

    }

    public override void LateUpdate()
    {

    }
}