using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
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
    public GameObject _gun, _player;

    public GameUI _gameUI;

    public Material _team1, _team2;

    private void Start()
    {
        _hpNow.Value = _hp;

        if (IsLocalPlayer)
        {
            //_gameUI = GetComponent<GameUI>();

            _playerID = SystemInfo.graphicsDeviceID;

            _movement = GetComponent<Movement>();
            _rb = GetComponent<Rigidbody>();
            _coll = GetComponent<Collider>();
            _mash = GetComponentInChildren<MeshRenderer>();

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

        Debug.LogWarning(damage.ToString() + "Damage To ServerRpc");
    }

    private void Update()
    {
        if (_hpNow.Value <= 0)
        {
            _gameUI._deadUI.SetActive(true);

            _dead = true;

            GetComponent<Movement>()._speedAcceleration = 0;
            GetComponent<Movement>()._canJump = false;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

            _gun.SetActive(false);

            SetvisibleServerRpc(false);
        }

        else 
        {
            _gameUI._deadUI.SetActive(false);

            _dead = false;

            GetComponent<Movement>()._speedAcceleration = 40000;
            GetComponent<Movement>()._canJump = true;
            GetComponent<Rigidbody>().useGravity = true;

            _gun.SetActive(true);

            SetvisibleServerRpc(true);
        }
    }

    [ServerRpc]
    void SetvisibleServerRpc(bool _bool)
    {
        _player.SetActive(_bool);

        SetvisibleClientRpc(_bool);
    }

    [ClientRpc]
    void SetvisibleClientRpc(bool _bool)
    {
        _player.SetActive(_bool);
    }

    void Dead()
    {
        _dead = true;

        _movement._speedAcceleration = 0;
        _movement._canJump = false;
        _rb.useGravity = false;
        _rb.velocity = new Vector3(0, 0, 0);
        _gun.gameObject.SetActive(false);
    }

    public void Alive()
    {
        SetHpServerRpc();

        _dead = false;

        _movement._speedAcceleration = 40000;
        _movement._canJump = true;
        _rb.useGravity = true;

        _gun.gameObject.SetActive(true);

        int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
        transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;
    }



    [ServerRpc(RequireOwnership = false)]
    void SetHpServerRpc()
    {
        _hpNow.Value = _hp;
    }

    PlayerControlls _input;
    InputAction _respawnButton;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _respawnButton = _input.Movement.Jump;

        _respawnButton.started += Respawn;
    }

    private void OnDisable()
    {
        _input.Disable();

        _respawnButton.started -= Respawn;
    }

    void Respawn(InputAction.CallbackContext context)
    {
        if (_hpNow.Value <= 0 && IsLocalPlayer)
        {
            SetHpServerRpc();

            int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
            transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;
        }
    }
}
