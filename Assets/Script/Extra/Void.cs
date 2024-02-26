using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Void : NetworkBehaviour
{
    private void OnTriggerStay(Collider other)
    {
        other.GetComponent<PlayerStats>().DeadServerRpc();
    }
}
