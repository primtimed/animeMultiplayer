using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

[CreateAssetMenu(fileName = "Dash", menuName = "Dash")]
public class Dash : BaseAbillitie
{
    public float _cooldown;

    public float _dashSpeed;

    float _timer;

    Movement _player;

    public void Start()
    {
        
    }

    public override void Open(GameObject player, AbilitieManager manager, GameObject NetworkManager, GameObject keep)
    {
        _player = player.GetComponent<Movement>();
    }

    public override async void Start(InputAction.CallbackContext context)
    {
        Debug.Log(_timer);

        if(_timer <= 0 && !_player._back._dash)
        {
            _player._back._dash = true;

            Vector3 _dashDirection = _player.transform.forward * _player._back._move.y + _player.transform.right * _player._back._move.x;
            _player._back._rb.AddForce(_dashDirection * _dashSpeed);

            await Task.Delay(250);

            _timer = _cooldown;
            _player._back._dash = false;
        }
    }
                                                                                                                                                    
    public override void Stop(InputAction.CallbackContext context)
    {

    }

    public override void Update()
    {
        if (!_player._back._dash)
        {
            _timer -= Time.deltaTime;
        }

        _player.GetComponent<GameUI>()._time = _timer;
    }
}
