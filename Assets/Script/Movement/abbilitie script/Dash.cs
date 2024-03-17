using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "Dash", menuName = "Dash")]
public class Dash : BaseAbillitie
{
    public Texture _icon;

    public float _cooldown;

    public float _dashSpeed;

    float _timer;

    Movement _player;
    GameUI _gameUI;

    void Start() { }

    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player.GetComponent<Movement>();
        _gameUI = player.GetComponent<GameUI>();

        _gameUI._abbilIcon.texture = _icon;
    }

    public override async void Start(InputAction.CallbackContext context)
    {
        if (_timer <= 0 && !_player._back._dash)
        {
            _player._back._dash = true;

            Vector3 _dashDirection = _player.transform.forward * _dashSpeed;
            _player._back._rb.AddForce(_dashDirection, ForceMode.Impulse);

            await Task.Delay(200);

            _timer = _cooldown;
            _player._back._dash = false;
        }
    }

    public override void Stop(InputAction.CallbackContext context)
    {

    }

    public override void Update()
    {
        if (_player)
        {
            if (!_player._back._dash)
            {
                _timer -= Time.deltaTime;
            }
        }

        if (_gameUI)
        {
            _gameUI._time = _timer;
        }
    }
}
