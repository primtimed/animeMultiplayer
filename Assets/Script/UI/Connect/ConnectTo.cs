using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
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
}
