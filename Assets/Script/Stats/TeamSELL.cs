using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class TeamSELL : NetworkBehaviour
{
    public GameObject _freeForAll;
    public GameObject _teams;

    private void Start()
    {
        if (GetComponentInParent<OwnerCheck>().IsHost)
        {
            _freeForAll.SetActive(true);
            GameObject.Find("Keep").GetComponent<MatchStats>()._freeForAll.Value = true;
        }

        else if (GameObject.Find("Keep").GetComponent<MatchStats>()._freeForAll.Value == true)
        {
            _freeForAll.SetActive(true);
            _teams.SetActive(false);
        }
    }
}
