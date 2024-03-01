using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Team
{
    None,
    Team1,
    Team2,
    Spectate
}

public class PlayerStats : NetworkBehaviour
{
    public NetworkVariable<int> _playerID = new NetworkVariable<int>();
    public NetworkVariable<Team> _team = new NetworkVariable<Team>();

    public float _hp;
    public NetworkVariable<float> _hpNow = new NetworkVariable<float>();
    public float _Kills;
    public float _deads;

    public float _kdr;

    [HideInInspector] public bool _dead;

    Movement _movement;
    Rigidbody _rb;
    Collider _coll;
    MeshRenderer _mash;
    BaseGun _gun;

    GameUI _gameUI;

    private void Start()
    {
        //_playerID.Value = SystemInfo.graphicsDeviceID;
        _playerID.Value = SystemInfo.graphicsDeviceID;

        _hpNow.Value = _hp;

        _movement = GetComponent<Movement>();
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _mash = GetComponentInChildren<MeshRenderer>();
        _gun = GetComponentInChildren<BaseGun>();

        _gameUI = GetComponent<GameUI>();

        _team.Value = Random.value < .5 ? Team.Team1 : Team.Team2;
    }

    private void Update()
    {
        _gameUI._hpSlider.value = _hpNow.Value;
        _gameUI._hpText.text = _hpNow.Value.ToString("f0");
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        _hpNow.Value -= damage;
        Debug.Log(damage);

        if (_hpNow.Value <= 0)
        {
            DeadServerRpc();
        }
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
            _dead = true;
            _hpNow.Value = _hp;

            _movement._speedAcceleration = 0;
            _movement._canJump = false;
            _rb.useGravity = false;
            _rb.velocity = new Vector3(0, 0, 0);
            _mash.enabled = false;
            _coll.enabled = false;
            _gun.gameObject.SetActive(false);

            _deads += 1;

            _kdr = _Kills / _deads;

            if (IsLocalPlayer)
            {
                _gameUI.SetKillDead();
            }


            //word niet gedaan op host 
            SetSpawnClientRpc();
            //transform.position = Vector3.zero;


            _movement._speedAcceleration = 40000;
            _movement._canJump = true;
            _rb.useGravity = true;
            _mash.enabled = true;
            _coll.enabled = true;
            _gun.gameObject.SetActive(true);

            _dead = false;
        }
    }

    [ClientRpc]
    public void SetSpawnClientRpc()
    {
        int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
        transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;
        //transform.position = Vector3.zero;
    }


    void Kill()
    {
        _Kills += 1;

        _kdr = _Kills / _deads;
        _gameUI.SetKillDead();
    }
}
