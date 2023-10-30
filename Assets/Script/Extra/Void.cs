using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Void : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        GameObject _spawn = NetworkBehaviour.Instantiate(other.gameObject);
        Destroy(other.gameObject);
    }
}
