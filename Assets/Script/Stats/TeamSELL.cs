using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamSELL : MonoBehaviour
{
    public GameObject _freeForAll;
    public GameObject _teams;

    MatchStats _match;

    NetworkBehaviour _network;

    private void Start()
    {
        _match = GameObject.Find("Keep").GetComponent<MatchStats>();
        _network = GetComponentInParent<Movement>().GetComponent<NetworkBehaviour>();

        if (GetComponentInParent<OwnerCheck>().IsHost)
        {
            _freeForAll.SetActive(true);
        }

        else if (_match._freeForAll.Value == true)
        {
            _freeForAll.SetActive(true);
            _teams.SetActive(false);
        }
    }

    [ServerRpc]
    public void FreeForAllServerRpc()
    {
        _match._freeForAll.Value = true;
    }

    [ServerRpc]
    public void NotFreeForAllServerRpc()
    {
        _match._freeForAll.Value = false;
    }
}
