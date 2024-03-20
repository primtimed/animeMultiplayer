using TMPro;
using UnityEngine;

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
        GetComponentInParent<ConnectTo>().JoinRelayPublic(_name.text);
    }
}
