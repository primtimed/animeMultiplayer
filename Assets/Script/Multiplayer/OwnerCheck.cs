using System.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

public class OwnerCheck : NetworkBehaviour
{
    public GameObject _gameUI;

    public AbilitieManager _abilitieManager;
    public GameUI _gameUi;
    public Camera _camer, _miniMap;
    public AudioListener _audioListener;
    public Movement _movement;


    public void Start()
    {
        if (!IsOwner)
        {
            //_abilitieManager.enabled = false;
            _gameUi.enabled = false;
            _camer.enabled = false;
            _miniMap.enabled = false;
            _audioListener.enabled = false;
            _movement.enabled = false;

            _gameUI.SetActive(false);
        }

        else
        {
            //_abilitieManager.enabled = true;
            _abilitieManager.start();
            _gameUi.enabled = true;
            _camer.enabled = true;
            _miniMap.enabled = true;
            _audioListener.enabled = true;
            _movement.enabled = true;

            _gameUI.SetActive(true);
        }
    }
}
                       