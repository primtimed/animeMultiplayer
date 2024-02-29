using JetBrains.Annotations;
using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class BaseGun : NetworkBehaviour
{
    PlayerControlls _input;
    InputAction _shoot, _aim, _reload;

    Camera _cam;
    Movement _move;
    RaycastHit _hit;
    Vector3 _bloom;

    public GunStats _gun;

    int _gunAmmo;
    int _recoilInt;
    bool _shootBool, _aimBool, _reloadding;
    float _timer;
    float fov;

    public LayerMask _player;
    public GameObject _hitEffect;

    public UI _UI = new UI();

    [Serializable]
    public class UI
    {
        [SerializeField] public RawImage _image;
        [SerializeField] public RawImage _scope;
        [SerializeField] public TextMeshProUGUI _ammo;
    }

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

    public void Start()
    {
        _cam = GetComponentInParent<Camera>();
        _move = GetComponentInParent<Movement>();

        _gunAmmo = _gun._ammo;

        Instantiate(_gun._gun, transform);

        _UI._image.texture = _gun._gunPNG;
        _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();

        if (!GetComponentInParent<OwnerCheck>()._isOwner)
        {
            enabled = false;
        }
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

            _recoilInt = 0;
        }
    }
    
    private void Aim(InputAction.CallbackContext context)
    {
        if (_gun._weaponType == WeaponType.Sniper)
        {
            if (context.started)
            {
                fov = _cam.fieldOfView;

                _cam.fieldOfView = _cam.fieldOfView / _gun._zoom;
                _move._gameSens = _move._sensetivitie / _gun._zoom;
                _UI._scope.gameObject.SetActive(true);
                _aimBool = true;
            }

            else
            {
                _cam.fieldOfView = fov;
                _move._gameSens = _move._sensetivitie;
                _UI._scope.gameObject.SetActive(false);
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
        if (!_reloadding && _gunAmmo != _gun._ammo)
        {
            _reloadding = true;

            _recoilInt = 0;

            yield return new WaitForSecondsRealtime(_gun._reloadSpeed);

            _gunAmmo = _gun._ammo;
            _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();
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

                return;
            }

            else if (_gun._fireMode == FireMode.Auto)
            {
                Auto();

                return;
            }

            else if (_gun._fireMode == FireMode.Burst)
            {
                StartCoroutine(Burst());

                return;
            }

            else if (_gun._fireMode == FireMode.Single)
            {
                Single();

                return;
            }
        }
    }

    private void Auto()
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
        Recoil();
        _gunAmmo -= 1;
        _recoilInt += 1;

        _timer = 0;

        _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();
    }

    IEnumerator Burst()
    {
        _timer = 0;
        _shootBool = false;

        if (_gunAmmo > 0)
        {
            if (_aimBool)
            {
                _bloom = _cam.transform.forward;
            }

            else
            {
                Bloom();
            }

            Recoil();
            _recoilInt += 1;
            Shooting();
            _gunAmmo -= 1;
            yield return new WaitForSeconds(_gun._burstDelay);
        }

        if (_gunAmmo > 0)
        {
            if (_aimBool)
            {
                _bloom = _cam.transform.forward;
            }

            else
            {
                Bloom();
            }

            Recoil();
            _recoilInt += 1;
            Shooting();
            _gunAmmo -= 1;
            yield return new WaitForSeconds(_gun._burstDelay);
        }

        if (_gunAmmo > 0)
        {
            if (_aimBool)
            {
                _bloom = _cam.transform.forward;
            }

            else
            {
                Bloom();
            }

            Recoil();
            _recoilInt += 1;
            Shooting();
            _gunAmmo -= 1;
            yield return new WaitForSeconds(_gun._burstDelay);
        }


        _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();

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
        Recoil();

        _gunAmmo -= 1;

        _timer = 0;

        _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();

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
        Recoil();

        _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();

        if (_gun._fireMode != FireMode.Auto)
        {
            _shootBool = false;
        }
    }

    private void Bloom()
    {
        _bloom = _cam.transform.position + _cam.transform.forward * 100;
        _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.up;
        _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.right;
        _bloom -= _cam.transform.position;
        _bloom.Normalize();
    }

    private void Recoil()
    {
        _move._x -= _gun._recoil[_recoilInt].x / 3;
        _move._y -= _gun._recoil[_recoilInt].y / 3;
    }

    private void Shooting()
    {
        Shake();

        if (Physics.Raycast(_cam.transform.position, _bloom, out _hit, Mathf.Infinity))
        {

            if (_hit.transform.GetComponent<PlayerStats>() && !_hit.transform.GetComponent<PlayerStats>()._dead)
            {
                Debug.LogWarning(_hit.transform.gameObject.name);

                _hit.transform.GetComponent<PlayerStats>().TakeDamageServerRpc(_gun._damage);
            }

            Instantiate(_hitEffect, _hit.point, transform.rotation);
        }
    }

    private Quaternion target, rotationX, rotationY;

    public void Shake()
    {
        rotationX = Quaternion.AngleAxis(100, Vector3.right);
        rotationY = Quaternion.AngleAxis(100, Vector3.up);

        target = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, 5 * Time.deltaTime);
    }
}