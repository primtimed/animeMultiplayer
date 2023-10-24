using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEditor.SearchService;
using UnityEngine;

public class ConnectTo : MonoBehaviour
{
    public GameObject _ui;

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
        _ui.SetActive(false);
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
        _ui.SetActive(false);
    }

    public void Server()
    {
        NetworkManager.Singleton.StartServer();
        _ui.SetActive(false);
    }
}
