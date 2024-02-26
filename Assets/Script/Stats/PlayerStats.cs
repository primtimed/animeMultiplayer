using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public enum Team
{
    None,
    Team1,
    Team2,
    Spectate
}

public class PlayerStats : NetworkBehaviour
{
    public int _playerID;
    public Team _team;

    public float _hp;
    public float _HpNow;
    public float _Kills;
    public float _Deads;

    bool _dead;

    Movement _movement;
    Rigidbody _rb;
    Collider _coll;
    MeshRenderer _mash;

    GameUI _gameUI;

    private void Awake()
    {
        _playerID = SystemInfo.graphicsDeviceVendorID;
    }

    private void Start()
    {
        _HpNow = _hp;

        _movement = GetComponent<Movement>();
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _mash = GetComponentInChildren<MeshRenderer>();

        _gameUI = GetComponent<GameUI>();
    }

    private void Update()
    {
        _gameUI._hpSlider.value = _hp;
        _gameUI._hpText.text = _hp.ToString("f0");
    }


    [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        _HpNow -= damage;
    }


    [ServerRpc]
    void NoPlayerServerRpc()
    {
        
    }

    [ServerRpc]
    public void DeadServerRpc()
    {
        if(!_dead)
        {
            _movement._speedAcceleration = 0;
            _movement._canJump = false;
            _rb.useGravity = false;
            _rb.velocity = new Vector3(0, 0, 0);
            _mash.enabled = false;
            _coll.enabled = false;

            _Deads += 1;

            _gameUI.SetKillDead();
            StartCoroutine(Respawn());

            _dead = true;
        }
    }

    IEnumerator Respawn()
    {
        yield return new WaitForSecondsRealtime(5);

        transform.position = Vector3.zero;

        _HpNow = _hp;

        _movement._speedAcceleration = 4000;
        _movement._canJump = true;
        _rb.useGravity = true;
        _mash.enabled = true;
        _coll.enabled = true;

        _dead = false;
    }

    void Kill()
    {
        _Kills += 1;
    }
}
