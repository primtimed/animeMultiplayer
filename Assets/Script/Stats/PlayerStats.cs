using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    public float _hpNow;
    public float _Kills;
    public float _deads;

    public float _kdr;

    bool _dead;

    Movement _movement;
    Rigidbody _rb;
    Collider _coll;
    MeshRenderer _mash;
    BaseGun _gun;

    GameUI _gameUI;

    private void Awake()
    {
        _playerID = SystemInfo.graphicsDeviceVendorID;
    }

    private void Start()
    {
        _hpNow = _hp;

        _movement = GetComponent<Movement>();
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _mash = GetComponentInChildren<MeshRenderer>();
        _gun = GetComponentInChildren<BaseGun>();

        _gameUI = GetComponent<GameUI>();
    }

    private void Update()
    {
        _gameUI._hpSlider.value = _hpNow;
        _gameUI._hpText.text = _hpNow.ToString("f0");
    }


   [ServerRpc]
    public void TakeDamageServerRpc(float damage)
    {
        _hpNow -= damage;

        if (_hpNow < 0)
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
            _movement._speedAcceleration = 0;
            _movement._canJump = false;
            _rb.useGravity = false;
            _rb.velocity = new Vector3(0, 0, 0);
            _mash.enabled = false;
            _coll.enabled = false;
            _gun.gameObject.SetActive(false);

            _deads += 1;

            _kdr = _Kills / _deads;

            //if (IsOwner)
            //{
            //    _gameUI.SetKillDead();
            //}

            _dead = true;

            transform.position = Vector3.zero;

            _hpNow = _hp;

            _movement._speedAcceleration = 4000;
            _movement._canJump = true;
            _rb.useGravity = true;
            _mash.enabled = true;
            _coll.enabled = true;
            _gun.gameObject.SetActive(true);

            _dead = false;
        }
    }


    void Kill()
    {
        _Kills += 1;

        _kdr = _Kills / _deads;
        _gameUI.SetKillDead();
    }
}
