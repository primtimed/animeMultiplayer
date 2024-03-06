using TMPro;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

public class ConnectTo : MonoBehaviour
{
    public GameObject _ui;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        _ui.SetActive(false);
        Debug.Log("You are the HOST");
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        _ui.SetActive(false);
        Debug.Log("You are Connected");
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
        _ui.SetActive(false);
        Debug.Log("You are the SERVER");
    }

    private async void Start()
    {
        await UnityServices.InitializeAsync();
        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void CreateRelay()
    {
        try
        {
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(99);
            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            _ui.SetActive(false);

            Debug.Log($"{joinCode}");

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            SetGameID(joinCode);
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
            JoinAllocation allocation = await RelayService.Instance.JoinAllocationAsync(joinCode.text);
            _ui.SetActive(false);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData,
                allocation.HostConnectionData
            );

            SetGameID(joinCode.text);
            NetworkManager.Singleton.StartClient();
        }
        catch (RelayServiceException e)
        {
            Debug.Log(e);
        }
    }

    void SetGameID(string _ID)
    {
        GameObject.Find("Keep").GetComponent<MatchStats>()._gameID = _ID;
    }
}
