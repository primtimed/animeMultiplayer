using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.VisualScripting;

public class ConnectTo : MonoBehaviour
{
    public GameObject _ui;

    public GameObject _publicUI, _publicServer;

    string _name;
    int _maxPlayers;
    bool _privete;

    public TMP_InputField _nameField;
    public TMP_Dropdown _playersField;
    public TMP_Dropdown _priveteField;

    public void setServerStats()
    {
        _name = _nameField.text;
        _maxPlayers = (_playersField.value + 1) * 2;

        if(_priveteField.value == 0)
        {
            _privete = false;
        }

        else
        {
            _privete = true;
        }
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        FindLobby();
    }

    public async void CreateRelay()
    {
        try
        {
            CreateLobbyOptions options = new CreateLobbyOptions {IsPrivate = _privete };

            Lobby lobby = await LobbyService.Instance.CreateLobbyAsync(_name, _maxPlayers, options);
            Debug.Log(lobby.LobbyCode);

            _ui.SetActive(false);
            SetGameID(lobby.LobbyCode);
            NetworkManager.Singleton.StartHost();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }
    public async void JoinRelay(TMP_InputField joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            await Lobbies.Instance.JoinLobbyByCodeAsync(joinCode.text);
            _ui.SetActive(false);

            SetGameID(joinCode.text);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    public async void JoinRelayPublic(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);
            await Lobbies.Instance.JoinLobbyByIdAsync(joinCode);
            _ui.SetActive(false);

            SetGameID("");
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    async void FindLobby()
    {
        QueryResponse _lobby = await Lobbies.Instance.QueryLobbiesAsync();

        foreach (Lobby lobby in _lobby.Results)
        {
            GameObject _ui = Instantiate(_publicServer, _publicUI.transform);
            Debug.LogError(lobby.LobbyCode);
            _ui.GetComponent<ServerIn>().SetServerSettings(lobby.Name, lobby.Players.Count, lobby.MaxPlayers, lobby.Id);
        }
    }

    void SetGameID(string _ID)
    {
        GameObject.Find("Keep").GetComponent<MatchStats>()._gameID = _ID;
    }
}
