using System;
using System.Collections;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    Rigidbody _rb;
    public TextMeshProUGUI _speedUI, _abbilUI;
    public RawImage _abbilIcon;

    //Health
    public Slider _hpSlider;
    public TextMeshProUGUI _hpText;

    public RawImage _blood;
    Color _bloodColor = Color.red;

    public float _time;

    public StatsSettings _stats = new StatsSettings();

    PlayerControlls _input;
    InputAction _statsButton, _settingsButton;

    MatchStats _matchStats;

    [Header("")]

    public GameObject _deadUI;

    public Slider _team1Slider;
    public TextMeshProUGUI _team1Text;

    public Slider _team2Slider;
    public TextMeshProUGUI _team2Text;

    public TextMeshProUGUI _gameID;

    public GameObject _settings;

    [Header("EndGame")]
    public GameObject _uiEnd;

    private void Awake()
    {
        _input = new PlayerControlls();
    }


    private void OnEnable()
    {
        _input.Enable();

        _statsButton = _input.UI.Stats;
        _settingsButton = _input.UI.Esc;

        _statsButton.started += Stats;
        _statsButton.canceled += Stats;

        _settingsButton.started += Settings;
    }

    [Serializable]
    public class StatsSettings
    {
        [SerializeField] public GameObject _UI;
        [SerializeField] public TextMeshProUGUI _killUI, _deadUI, _kdrUI;
    }

    PlayerStats _playerStats;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerStats = GetComponent<PlayerStats>();

        _matchStats = GameObject.Find("Keep").GetComponent<MatchStats>();
        _deadUI.SetActive(false);

        _bloodColor.a = 0;
        _blood.color = _bloodColor;

        _gameID.text = _matchStats._gameID;

        _team1Slider.maxValue = _matchStats._winningPoints;
        _team2Slider.maxValue = _matchStats._winningPoints;

        StartCoroutine(Speed());
    }

    public IEnumerator Speed()
    {
        _speedUI.text = "Speed  " + (int)_rb.velocity.magnitude;
        yield return new WaitForSeconds(.2f);

        StartCoroutine(Speed());
    }

    public void SetKillDead()
    {
        _stats._killUI.text = _playerStats._Kills.ToString() + " Kills";
        _stats._deadUI.text = _playerStats._deads.ToString() + " Deads";
        _stats._kdrUI.text = _playerStats._kdr.ToString("f2") + " KDR";
    }

    void Stats(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _stats._UI.SetActive(true);
        }

        else if (context.canceled)
        {
            _stats._UI.SetActive(false);
        }
    }

    private void Update()
    {
        if (_time >= 0)
        {
            _abbilUI.text = _time.ToString("f0");
        }

        else
        {
            _abbilUI.text = null;
        }

        _hpSlider.value = _playerStats._hpNow.Value;
        _hpText.text = _playerStats._hpNow.Value.ToString();

        _team1Slider.value = _matchStats._team1Points.Value;
        _team1Text.text = _matchStats._team1Points.Value.ToString();

        _team2Slider.value = _matchStats._team2Points.Value;
        _team2Text.text = _matchStats._team2Points.Value.ToString();

        if (_playerStats._hpNow.Value < 50)
        {
            float hpRotate = _playerStats._hpNow.Value;

            _bloodColor.a = 1.1f / hpRotate;
            _blood.color = _bloodColor;
        }

        else
        {
            _bloodColor.a = 0;
            _blood.color = _bloodColor;
        }
    }

    [HideInInspector] public Movement _move;
    [HideInInspector] public AbilitieManager _abil;
    [HideInInspector] public BaseGun _gun;
    [HideInInspector] public mouseLock _lock;
     void Settings(InputAction.CallbackContext context)
    {
        if (_settings.activeSelf)
        {
            _settings.SetActive(false);

            _move.enabled = true;

            _abil.enabled = true;

            _gun.enabled = true;

            _lock.SetLock(true);
        }

        else
        {
            _settings.SetActive(true);

            _move = GetComponent<Movement>();
            _move.enabled = false;

            _abil = GetComponent<AbilitieManager>();
            _abil.enabled = false;

            _gun = GetComponentInChildren<BaseGun>();
            _gun.enabled = false;

            _lock = GetComponentInChildren<mouseLock>();
            _lock.SetLock(false);
        }
    }
}