using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchStats : NetworkBehaviour
{
    public string _gameID;
    public int _winningPoints;

    public NetworkVariable<int> _team1Points;
    public NetworkVariable<int> _team2Points;

    public NetworkVariable<bool> _freeForAll;

    private void Update()
    {
        if (_team1Points.Value >= _winningPoints)
        {
            Debug.Log("team 1 wins");
        }

        else if (_team2Points.Value >= _winningPoints)
        {
            Debug.Log("team 2 wins");
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void AddPointServerRpc(int team)
    {
        if(team == 1)
        {
            _team2Points.Value++;
            return;
        }

        else if (team == 2)
        {
            _team1Points.Value++;
            return;
        }
    }
}
