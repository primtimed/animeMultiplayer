using Unity.Netcode;
using UnityEngine;

public class ToServer : NetworkBehaviour
{
    Wall _wall;
    GameObject _spawnedWall;

    public void OpenWall(Wall wall)
    {
        _wall = wall;
    }

    [ServerRpc]
    public void spawnWallServerRpc()
    {
        Debug.LogError("wall2");

        if (_spawnedWall)
        {
            Destroy(_spawnedWall);
        }

        _spawnedWall = Instantiate(_wall._wall, _wall._player.position, _wall._player.rotation);
        _spawnedWall.GetComponent<NetworkObject>().Spawn();
    }

    public void Test()
    {
        Debug.LogWarning("Test");
    }
}
