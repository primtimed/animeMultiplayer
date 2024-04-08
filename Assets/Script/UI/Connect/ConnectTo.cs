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
using Unity.Networking.Transport.Relay;

public class ConnectTo : NetworkBehaviour
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
            //Lobby
            //CreateLobbyOptions optionsCreate = new CreateLobbyOptions { IsPrivate = _privete };
            //Lobby lobbyCreate = await LobbyService.Instance.CreateLobbyAsync(_name, _maxPlayers, optionsCreate);

            //RPC
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(99);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            RelayServerData serverData = new RelayServerData(allocation, "dtls");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);
            NetworkManager.Singleton.StartHost();

            _ui.SetActive(false);
            SetGameID(joinCode);

            Destroy(gameObject);
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
            //await Lobbies.Instance.JoinLobbyByIdAsync(joinCode.text);
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode.text);

            //RelayServerData serverData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
            NetworkManager.Singleton.StartClient();

            _ui.SetActive(false);
            SetGameID(joinCode.text);
            Debug.Log("Joining Relay with " + joinCode.text);

            Destroy(gameObject);
        }
        catch (RelayServiceException e)
        {
            Debug.LogWarning(e);
        }
    }

    public async void JoinRelayPublic(string joinCode)
    {
        try
        {
            Debug.Log("Joining Relay with " + joinCode);

            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            RelayServerData serverData = new RelayServerData(allocation, "dtls");
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(serverData);

            _ui.SetActive(false);
            SetGameID(joinCode);
            NetworkManager.Singleton.StartClient();

            Destroy(gameObject);
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
            _ui.GetComponent<ServerIn>().SetServerSettings(lobby.Name, lobby.Players.Count, lobby.MaxPlayers, lobby.Id);
        }
    }

    void SetGameID(string _ID)
    {
        GameObject.Find("Keep").GetComponent<MatchStats>()._gameID = _ID;
    }
}
