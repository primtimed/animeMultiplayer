using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class AbilitieManager : NetworkBehaviour
{
    PlayerControlls _input;
    InputAction _abbilitieKey1;

    public BaseAbillitie _abbilitie;
    public BasePassive _passive;

    public Transform _graplingLoc;

    GameObject _network; // word niet gevonden
    GameObject _keep; // word niet gevonden

    public BaseAbillitie _baseAbillitie;
    public BasePassive _basePassive;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    public void SetAbbilities(BaseAbillitie abbilitie)
    {
        _abbilitie = abbilitie;
    }

    public void SetPassive(BasePassive passive)
    {
        _passive = passive;
    }

    public void startX()
    {


        _network = gameObject;
        _keep = GameObject.Find("Keep");

        if (_abbilitie == null)
        {
            _abbilitie = _baseAbillitie;
            _passive = _basePassive;
        }

        Debug.LogError("AbilitieManager");

        _abbilitie.Open(gameObject, this, _network, _keep);
        _passive.Open(gameObject);

        OnEnable();
    }

    private void OnEnable()
    {
        _input.Enable();

        _abbilitieKey1 = _input.Abbilities._1;

        if (_abbilitie)
        {
            _abbilitieKey1.started += _abbilitie.Start;
            _abbilitieKey1.canceled += _abbilitie.Stop;
        }
    }

    private void OnDisable()
    {
        _abbilitieKey1.started -= _abbilitie.Start;
        _abbilitieKey1.canceled -= _abbilitie.Stop;
    }

    private void LateUpdate()
    {
        if (_abbilitie)
        {
            _abbilitie.LateUpdate();
        }
    }

    private void Update()
    {
        if (_abbilitie)
        {
            _abbilitie.Update();
        }
    }
}
