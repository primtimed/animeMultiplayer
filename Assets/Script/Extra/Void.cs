using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class Void : NetworkBehaviour
{
    public Transform _spawnloc;
    private void OnTriggerEnter(Collider other)
    {
        other.transform.position= Vector3.zero;

        //GameObject _spawnPlayer = Instantiate(other.gameObject, _spawnloc);
        //_spawnPlayer.GetComponent<NetworkObject>().IsOwner= true;
        //Destroy(other.gameObject);

    }
}
