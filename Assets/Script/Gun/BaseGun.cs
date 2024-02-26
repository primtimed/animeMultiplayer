using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGun : MonoBehaviour
{
    PlayerControlls _input;
    InputAction _shoot, _aim, _reload;

    Camera _cam;
    RaycastHit _hit;
    Vector3 _bloom;

    public GunStats _gun;

    int _gunAmmo;
    bool _shootBool, _aimBool, _reloadding;
    float _timer;

    public LayerMask _players;
    public GameObject _hitEffect;

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        _input.Enable();

        _shoot = _input.Weapon.Shoot;
        _aim = _input.Weapon.Aim;
        _reload = _input.Weapon.Reload;

        _shoot.started += Shoot;
        _shoot.canceled += Shoot;

        _aim.started += Aim;
        _aim.canceled += Aim;

        _reload.started += Reload;
    }

    private void OnDisable()
    {
        _shoot.started -= Shoot;
        _shoot.canceled -= Shoot;

        _aim.started -= Aim;
        _aim.canceled -= Aim;

        _reload.started -= Reload;
    }

    private void Start()
    {
        _cam = GetComponentInParent<Camera>();

        _gunAmmo = _gun._ammo;
    }

    private void Shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            _shootBool = true;
        }

        else
        {
            _shootBool = false;
        }
    }

    private void Aim(InputAction.CallbackContext context)
    {
        if (_gun._weaponType == WeaponType.Sniper)
        {
            if (context.started)
            {
                Debug.Log("AimIn");

                _aimBool = true;
            }

            else
            {
                Debug.Log("AimOut");

                _aimBool = false;
            }
        }
    }

    private void Reload(InputAction.CallbackContext context)
    {
        StartCoroutine(ReloadingIENUM());
    }

    IEnumerator ReloadingIENUM()
    {
        if (!_reloadding)
        {
            _reloadding = true;
            yield return new WaitForSecondsRealtime(_gun._reloadSpeed);

            _gunAmmo = _gun._ammo;
            _reloadding = false;
        }
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (_shootBool && _timer > _gun._fireRate && !_reloadding && _gunAmmo > 0)
        {
            if (_gun._weaponType == WeaponType.Shotgun)
            {
                Shotgun();
            }

            else if (_gun._fireMode == FireMode.Auto)
            {
                Auto();
            }

            else if (_gun._fireMode == FireMode.Burst)
            {
                Burst();
            }

            else if (_gun._fireMode == FireMode.Single)
            {
                Single();
            }
        }
    }

    private void Auto()
    {
        Debug.Log("Shoot");

        Bloom();
        Shooting();
        _gunAmmo -= 1;

        _timer = 0;
    }

    private void Burst()
    {
        Bloom();
        Shooting();
        _gunAmmo -= 1;
        Bloom();
        Shooting();
        _gunAmmo -= 1;
        Bloom();
        Shooting();
        _gunAmmo -= 1;

        _timer = 0;
        _shootBool = false;
    }

    private void Single()
    {
        if (_aimBool)
        {
            _bloom = _cam.transform.forward;
        }

        else
        {
            Bloom();
        }

        Shooting();
        _gunAmmo -= 1;

        _timer = 0;
        _shootBool = false;
    }

    private void Shotgun()
    {
        Bloom();
        Shooting();

        Bloom();
        Shooting();

        Bloom();
        Shooting();

        Bloom();
        Shooting();

        Bloom();
        Shooting();
        _gunAmmo -= 1;

        _timer = 0;
        _shootBool = false;
    }

    private void Bloom()
    {
        _bloom = _cam.transform.position + _cam.transform.forward * 100;
        _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.up;
        _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.right;
        _bloom -= _cam.transform.position;
        _bloom.Normalize();
    }

    private void Shooting()
    {
        if (Physics.Raycast(_cam.transform.position, _bloom, out _hit, Mathf.Infinity))
        {
            Instantiate(_hitEffect, _hit.point, transform.rotation);

            if (_hit.transform.gameObject.layer == _players)
            {
                _hit.transform.GetComponent<PlayerStats>().TakeDamageServerRpc(_gun._damage);
            }
        }
    }
}