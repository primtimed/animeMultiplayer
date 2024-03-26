using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndGameStats : MonoBehaviour
{
    public GameObject _won;
    public GameObject _lost;

    public TextMeshProUGUI _team1Score;   
    public TextMeshProUGUI _team2Score;

    private void OnEnable()
    {
        if(GetComponentInParent<PlayerStats>()._team.Value == GameObject.Find("Keep").GetComponent<MatchStats>()._teamWon.Value)
        {
            _won.SetActive(true);
        }

        else
        {
            _lost.SetActive(true);
        }

        _team1Score.text = GameObject.Find("Keep").GetComponent<MatchStats>()._team1Points.Value.ToString();
        _team2Score.text = GameObject.Find("Keep").GetComponent<MatchStats>()._team2Points.Value.ToString();
    }
}
