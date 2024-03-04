using Unity.Netcode;
using UnityEngine;

public class Void : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.GetComponent<PlayerStats>().DeadServerRpc();
    }
}
