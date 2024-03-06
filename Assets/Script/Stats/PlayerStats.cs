using Unity.Netcode;
using UnityEngine;
using Random = UnityEngine.Random;

public enum Team
{
    None,
    Team1,
    Team2,
    Spectate,
    FreeForAll
}

public class PlayerStats : NetworkBehaviour
{
    public int _playerID;
    public NetworkVariable<Team> _team = new NetworkVariable<Team>();

    MatchStats _match;

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

    public Material _team1, _team2;

    private void Start()
    {
        //_playerID.Value = SystemInfo.graphicsDeviceID;
        _playerID = SystemInfo.graphicsDeviceID;

        _hpNow.Value = _hp;

        _movement = GetComponent<Movement>();
        _rb = GetComponent<Rigidbody>();
        _coll = GetComponent<Collider>();
        _mash = GetComponentInChildren<MeshRenderer>();
        _gun = GetComponentInChildren<BaseGun>();

        _match = GameObject.Find("Keep").GetComponent<MatchStats>();

        if (IsLocalPlayer)
        {
            _gameUI = GetComponent<GameUI>();
        }
    }

    public void SetTeam(TeamSellect team)
    {
        _team.Value = team._team;

        if(_team.Value == Team.Team1)
        {
            GetComponentInChildren<MeshRenderer>().material = _team1;
        }

        else if (_team.Value == Team.Team2)
        {
            GetComponentInChildren<MeshRenderer>().material = _team2;
        }
    }

    private void Update()
    {
        if (IsLocalPlayer)
        {
            _gameUI._hpSlider.value = _hpNow.Value;
            _gameUI._hpText.text = _hpNow.Value.ToString("f0");
        }
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        _hpNow.Value -= damage;

        if (_hpNow.Value <= 0)
        {
            DeadServerRpc();
        }
    }

    [ClientRpc]
    public void TakeDamageClientRpc(float damage)
    {
        if (!IsHost)
        {
            _hpNow.Value -= damage;
        }
    }

    [ServerRpc]
    void NoPlayerServerRpc()
    {

    }

    [ServerRpc]
    public void DeadServerRpc()
    {
        if (!_dead)
        {
            _dead = true;

            _movement._speedAcceleration = 0;
            _movement._canJump = false;
            _rb.useGravity = false;
            _rb.velocity = new Vector3(0, 0, 0);
            _mash.enabled = false;
            _coll.enabled = false;
            _gun.gameObject.SetActive(false);

            _deads += 1;
            SetMatchServerRpc();

            _kdr = _Kills / _deads;

            if (IsLocalPlayer)
            {
                _gameUI.SetKillDead();
            }

            SetSpawnClientRpc();

            _movement._speedAcceleration = 40000;
            _movement._canJump = true;
            _rb.useGravity = true;
            _mash.enabled = true;
            _coll.enabled = true;
            _gun.gameObject.SetActive(true);

            _dead = false;
            _hpNow.Value = _hp;
        }
    }

    [ClientRpc]
    public void SetSpawnClientRpc()
    {
        int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
        transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;

        _hpNow.Value = _hp;
    }

    [ServerRpc(RequireOwnership = false)]
    public void SetMatchServerRpc()
    {
        if (_team.Value == Team.Team1)
        {
            _match._team2Points.Value += 1;
        }

        else if (_team.Value == Team.Team2)
        {
            _match._team1Points.Value += 1;
        }

        SetMatchClientRpc();
    }

    [ClientRpc]
    public void SetMatchClientRpc()
    {
        if (!IsHost)
        {
            if (_team.Value == Team.Team1)
            {
                _match._team2Points.Value += 1;
            }

            else if (_team.Value == Team.Team2)
            {
                _match._team1Points.Value += 1;
            }
        }
    }


    void Kill()
    {
        _Kills += 1;

        _kdr = _Kills / _deads;
        _gameUI.SetKillDead();
    }
}
