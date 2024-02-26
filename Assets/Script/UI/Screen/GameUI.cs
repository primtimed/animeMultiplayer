using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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

    public float _time;

    public StatsSettings _stats = new StatsSettings();

    PlayerControlls _input;
    InputAction _statsButton;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _statsButton = _input.UI.Stats;

        _statsButton.started += Stats;
        _statsButton.canceled += Stats;
    }

    [Serializable]
    public class StatsSettings
    {
        [SerializeField] public GameObject _UI;
        [SerializeField] public TextMeshProUGUI _killUI, _deadUI;
    }

    PlayerStats _playerStats;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _playerStats = GetComponent<PlayerStats>();

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
        _stats._deadUI.text = _playerStats._Deads.ToString() + " Deads";
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
    }
}
