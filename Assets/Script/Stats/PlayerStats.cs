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
    public NetworkVariable<float> _hpNow;
    public float _Kills;
    public float _deads;

    public float _kdr;

    public bool _dead;

    bool _endGame;

    [HideInInspector] public Movement _movement;
    [HideInInspector] public Rigidbody _rb;
    [HideInInspector] public Collider _coll;
    [HideInInspector] public MeshRenderer _mash;

    public MeshRenderer _mapMash;
    public GameObject _gun, _player;

    public GameUI _gameUI;

    public Material _team1, _team2, _normal;

    private void Start()
    {
        _hpNow.Value = _hp;

        _mash = GetComponentInChildren<MeshRenderer>();

        if (IsLocalPlayer)
        {
            _playerID = SystemInfo.graphicsDeviceID;

            _movement = GetComponent<Movement>();
            _rb = GetComponent<Rigidbody>();
            _coll = GetComponent<Collider>();

            _match = GameObject.Find("Keep").GetComponent<MatchStats>();
        }
    }

    [ServerRpc]
    public void SetTeam1ServerRpc()
    {
        _team.Value = Team.Team1;
    }

    [ServerRpc]
    public void SetTeam2ServerRpc()
    {
        _team.Value = Team.Team2;
    }

    [ServerRpc]
    public void SetTeamFreeServerRpc()
    {
        _team.Value = Team.FreeForAll;
    }


    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(float damage)
    {
        _hpNow.Value -= damage;

        Debug.LogWarning(damage.ToString() + "Damage To ServerRpc");
    }

    void SetCollor()
    {
        if (_mash)
        {
            if (_team.Value == Team.Team1)
            {
                _mash.material = _team1;
                _mapMash.material = _team1;
            }

            else if (_team.Value == Team.Team2)
            {
                _mash.material = _team2;
                _mapMash.material = _team2;
            }

            else
            {
                _mash.material = _normal;
                _mapMash.material = _normal;
            }
        }
    }

    bool _spectaitCheck;
    void Spectate()
    {
        if (_spectaitCheck != _movement._spectator)
        {
            _spectaitCheck = _movement._spectator;

            if (_movement._spectator)
            {
                GetComponent<Rigidbody>().useGravity = false;
                GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);
                _gun.SetActive(false);
                GetComponent<AbilitieManager>().enabled = false;
            }

            else
            {
                GetComponent<Rigidbody>().useGravity = true;
                _gun.SetActive(true);
                GetComponent<AbilitieManager>().enabled = true;
            }
        }
    }

    public void GameEnd()
    {
        _endGame = true;

        _gameUI._deadUI.SetActive(false);
        _gameUI._uiEnd.SetActive(true);

        GetComponent<Movement>()._speedAcceleration = 0;
        GetComponent<Movement>()._canJump = false;
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

        _gun.SetActive(false);
    }

    private void Update()
    {
        SetCollor();
        Spectate();

        if (GameObject.Find("Keep").GetComponent<MatchStats>()._teamWon.Value != Team.None)
        {
            GameEnd();
        }

        if (_endGame) return;

        if (_movement._spectator) return;

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

    [ServerRpc(RequireOwnership = false)]
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

    [ServerRpc(RequireOwnership = false)]
    void SetHpServerRpc()
    {
        _hpNow.Value = _hp;
    }


    // Respawn //
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
        if (_hpNow.Value <= 0 && IsLocalPlayer && _dead)
        {
            if (_team.Value == Team.Team1)
            {
                GameObject.Find("Keep").GetComponent<MatchStats>().AddPointServerRpc(1);
            }

            else if (_team.Value == Team.Team2)
            {
                GameObject.Find("Keep").GetComponent<MatchStats>().AddPointServerRpc(2);
            }

            SetHpServerRpc();

            int _int = Random.Range(0, GameObject.Find("Spawns").GetComponent<Respawn>()._spawns.Length);
            transform.position = GameObject.Find("Spawns").GetComponent<Respawn>()._spawns[_int].position;
        }
    }
}
