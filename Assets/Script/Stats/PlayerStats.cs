using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerStats : NetworkBehaviour
{
    public float _hp;
    public float _Kills;
    public float _Deads;


    [ServerRpc]
    void TakeDamageServerRpc(float damage)
    {
        _hp -= damage;
    }


    [ServerRpc]
    void NoPlayerServerRpc()
    {
        
    }
}
