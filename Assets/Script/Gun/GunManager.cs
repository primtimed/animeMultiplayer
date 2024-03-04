using UnityEngine;
using UnityEngine.InputSystem;

public class GunManager : MonoBehaviour
{
    public GameObject _gun, _pistol;

    PlayerControlls _input;
    InputAction _gunINPUT, _pistolINPUT;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _gunINPUT = _input.Weapon.weapon1;
        _pistolINPUT = _input.Weapon.weapon2;

        _gunINPUT.started += Gun;
        _pistolINPUT.started += Pistol;
    }

    private void OnDisable()
    {
        _input.Disable();

        _gunINPUT.started -= Gun;
        _pistolINPUT.started -= Pistol;
    }


    private void Gun(InputAction.CallbackContext context)
    {
        _gun.SetActive(true);
        _pistol.SetActive(false);
    }

    private void Pistol(InputAction.CallbackContext context)
    {
        _gun.SetActive(false);
        _pistol.SetActive(true);
    }
}
