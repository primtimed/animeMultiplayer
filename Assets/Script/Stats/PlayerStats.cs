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

    public bool _dead;

    [HideInInspector] public Movement _movement;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _coll;
    [HideInInspector] public MeshRenderer _mash;
    [HideInInspector] public BaseGun _gun;

    GameUI _gameUI;

    public Material _team1, _team2;

    private void Start()
    {
        _hpNow.Value = _hp;

        if (IsLocalPlayer)
        {
            _gameUI = GetComponent<GameUI>();

            _playerID = SystemInfo.graphicsDeviceID;

            _movement = GetComponent<Movement>();
            _rb = GetComponent<Rigidbody>();
            _coll = GetComponent<Collider>();
            _mash = GetComponentInChildren<MeshRenderer>();
            _gun = GetComponentInChildren<BaseGun>();

            _match = GameObject.Find("Keep").GetComponent<MatchStats>();
        }
    }

    public void SetTeam(TeamSellect team)
    {
        Debug.Log(team.ToString());

        //_team.Value = team._team;
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        _hpNow.Value -= damage;

        Debug.LogWarning(damage.ToString());
    }

    private void Update()
    {
        Debug.Log(_hpNow.Value);
        Debug.Log(_dead);

        if (_hpNow.Value <= 0 && !_dead)
        {
            Dead();
        }
    }


    void Dead()
    {
        Debug.Log("dead");

        if(IsLocalPlayer)
        {
            Debug.Log("dead2");

            _dead = true;

            _movement._speedAcceleration = 0;
            _movement._canJump = false;
            _rb.useGravity = false;
            _rb.velocity = new Vector3(0, 0, 0);
            _gun.gameObject.SetActive(false);

            _gameUI._deadUI.SetActive(true);
        }
    }

    public void Alive()
    {
        if (IsLocalPlayer)
        {
            _hpNow.Value = _hp;
            _dead = false;

            _movement._speedAcceleration = 40000;
            _movement._canJump = true;
            _rb.useGravity = true;
            _gun.gameObject.SetActive(true);


            int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
            transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;
        }
    }
}
