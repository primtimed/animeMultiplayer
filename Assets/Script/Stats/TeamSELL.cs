using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;

public class TeamSELL : MonoBehaviour
{
    public GameObject _freeForAll;
    public GameObject _teams;

    MatchStats _match;

    NetworkBehaviour _network;

    public TextMeshProUGUI _team1Players;
    public TextMeshProUGUI _team2Players;

    private void Start()
    {
        _match = GameObject.Find("Keep").GetComponent<MatchStats>();
        _network = GetComponentInParent<Movement>().GetComponent<NetworkBehaviour>();

        //if (GetComponentInParent<OwnerCheck>().IsHost)
        //{
        //    _freeForAll.SetActive(true);
        //}

        //else if (_match._freeForAll.Value == true)
        //{
        //    _freeForAll.SetActive(true);
        //    _teams.SetActive(false);
        //}
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

    private void Update()
    {
        _team1Players.text = _match._team1Players.Value.ToString() + " Players";     
        _team2Players.text = _match._team2Players.Value.ToString() + " Players";
    }
}
