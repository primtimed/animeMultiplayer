using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode.Transports.UTP;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Relay.Models;
using Unity.Services.Relay;
using UnityEngine;
using static BaseGun;

public class ServerIn : MonoBehaviour
{
    public TextMeshProUGUI _name, _players;

    string _code;

    public void SetServerSettings(string name, int nowPlayers, int MaxPlayers, string GameCode)
    {
        _code = GameCode;
        _name.text = name;
        _players.text = nowPlayers.ToString() + " / " + MaxPlayers.ToString();
    }

    public void JoinLobby()
    {
        GetComponentInParent<ConnectTo>().JoinRelayPublic(_code);
    }
}
