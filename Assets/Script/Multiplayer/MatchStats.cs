using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class MatchStats : MonoBehaviour
{
    public string _gameID;
    public int _winningPoints;

    public NetworkVariable<int> _team1Points = new NetworkVariable<int>();
    public NetworkVariable<int> _team2Points = new NetworkVariable<int>();

    public NetworkVariable<List<GameObject>> _team1 = new NetworkVariable<List<GameObject>>();
    public NetworkVariable<List<GameObject>> _team2 = new NetworkVariable<List<GameObject>>();

    private void Update()
    {
        if(_team1Points.Value >= _winningPoints)
        {
            Debug.Log("team 1 wins");
        }

        else if (_team2Points.Value >= _winningPoints)
        {
            Debug.Log("team 2 wins");
        }
    }
}
