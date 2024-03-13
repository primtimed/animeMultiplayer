using Unity.Netcode;
using UnityEngine;

public class OwnerCheck : NetworkBehaviour
{
    public bool _isOwner;

    public GameObject _gameUI, _gameui2;

    public AbilitieManager _abilitieManager;
    public GameUI _gameUi;
    public Camera _camer, _miniMap;
    public AudioListener _audioListener;
    public Movement _movement;


    public void Start()
    {
        if (!IsOwner)
        {
            _isOwner = false;

            _gameUi.enabled = false;
            _camer.enabled = false;
            _miniMap.enabled = false;
            _audioListener.enabled = false;
            //_movement.enabled = false;

            _gameUI.SetActive(false);
            _gameui2.SetActive(false);
        }

        else
        {
            _isOwner = true;

            _gameUi.enabled = true;
            _camer.enabled = true;
            _miniMap.enabled = true;
            _audioListener.enabled = true;

            _gameUI.SetActive(true);
            _gameui2.SetActive(true);
        }
    }
}