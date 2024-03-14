using System;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using Unity.Netcode;
using Unity.VisualScripting;
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
    //public NetworkVariable<GameObject> _gunP = new NetworkVariable<GameObject>();

    int _gunAmmo;
    int _recoilInt;
    bool _shootBool, _aimBool, _reloadding;
    float _timer;
    float fov;
    WeaponType _weaponType;

    public UI _UI = new UI();
    public UX _UX = new UX();

    [Serializable]
    public class UI
    {
        [SerializeField] public RawImage _image;
        [SerializeField] public RawImage _scope;
        [SerializeField] public TextMeshProUGUI _ammo;

        [SerializeField] public GameObject _reload;
        [SerializeField] public GameObject _teamMate;

        [SerializeField] public GameObject _hit;
    }

    [Serializable]
    public class UX
    {
        [SerializeField] public GameObject _hit;
        [SerializeField] public GameObject _sound;
    }

    private void Awake()
    {
        _input = new PlayerControlls();
    }

    private void OnEnable()
    {
        OnEnables();
    }

    private void OnEnables()
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

        if (_gun)
        {
            _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();
        }

        _UI._reload.SetActive(false);
        _UI._teamMate.SetActive(false);
        _reloadding = false;
    }

    private void OnDisable()
    {
        _shoot.started -= Shoot;
        _shoot.canceled -= Shoot;

        _aim.started -= Aim;
        _aim.canceled -= Aim;

        _reload.started -= Reload;
    }

    public void SetGun(GunStats gun)
    {
        _gun = gun;
    }

    GameObject _mainGun;
    private void Start()
    {
        _cam = GetComponentInParent<Camera>();
        fov = _cam.fieldOfView;
        _move = GetComponentInParent<Movement>();

        if (_gun != null )
        {
            _gunAmmo = _gun._ammo;
            _UI._image.texture = _gun._gunPNG;
            _weaponType = _gun._weaponType;

            _mainGun = Instantiate(_gun._gun, transform);
        }
    }

    public void StartX()
    {
        OnEnables();

        _gunAmmo = _gun._ammo;
        _UI._image.texture = _gun._gunPNG;
        _weaponType = _gun._weaponType;

        _mainGun = Instantiate(_gun._gun, transform);

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

        else
        {
            if (context.started)
            {
                _cam.fieldOfView = _cam.fieldOfView / 1.1f;
                _move._gameSens = _move._sensetivitie / 1.1f;
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
        if (!IsLocalPlayer) return;

        StartCoroutine(ReloadingIENUM());
    }

    IEnumerator ReloadingIENUM()
    {
        if (!_reloadding && _gunAmmo != _gun._ammo)
        {
            Debug.LogWarning(_gun._name);

            _reloadding = true;
            _UI._reload.SetActive(true);

            _recoilInt = 0;

            yield return new WaitForSecondsRealtime(_gun._reloadSpeed);

            _gunAmmo = _gun._ammo;
            _UI._ammo.text = _gunAmmo.ToString() + " / " + _gun._ammo.ToString();
            _UI._reload.SetActive(false);
            _reloadding = false;
        }
    }

    private void Update()
    {
        _timer += Time.unscaledDeltaTime;

        if (!_gun) return;

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
        float Weaponspret = (_move._back._rb.velocity.magnitude / 12) * -_gun._weaponSpretAmount;

        if(_gun._weaponType == WeaponType.Shotgun)
        {
            _bloom = _cam.transform.position + _cam.transform.forward * 100;
            _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.up;
            _bloom += Random.Range(-_gun._weaponSpretAmount, _gun._weaponSpretAmount) * _cam.transform.right;
            _bloom -= _cam.transform.position;
            _bloom.Normalize();

            return;
        }

        if (!_aimBool)
        {
            _bloom = _cam.transform.position + _cam.transform.forward * 100;
            _bloom += Random.Range(-Weaponspret, Weaponspret) * _cam.transform.up;
            _bloom += Random.Range(-Weaponspret, Weaponspret) * _cam.transform.right;
            _bloom -= _cam.transform.position;
            _bloom.Normalize();
        }

        else
        {
            _bloom = _cam.transform.position + _cam.transform.forward * 100;
            _bloom += Random.Range(-Weaponspret, Weaponspret / 1.4f) * _cam.transform.up;
            _bloom += Random.Range(-Weaponspret, Weaponspret / 1.4f) * _cam.transform.right;
            _bloom -= _cam.transform.position;
            _bloom.Normalize();
        }
    }

    private void Recoil()
    {
        //float recoilX = Mathf.Lerp(_move._x, _move._x - _gun._recoil[_recoilInt].x / 3, 1);
        //float recoilY = Mathf.Lerp(_move._y, _move._y - _gun._recoil[_recoilInt].x / 3, 1);

        //_move._x -= recoilX;
        //_move._y -= recoilX;

        _move._x -= _gun._recoil[_recoilInt].x / 3;
        _move._y -= _gun._recoil[_recoilInt].y / 3;
    }


    GameObject _bullet;
    private void Shooting()
    {
        if (!IsLocalPlayer) return;

        Shake();
        ShotServerRpc();

        if (Physics.Raycast(_cam.transform.position, _bloom, out _hit, Mathf.Infinity))
        {

            if (_hit.transform.GetComponent<PlayerStats>())
            {
                //if (GetComponentInParent<PlayerStats>()._team.Value != _hit.transform.GetComponent<PlayerStats>()._team.Value || (GetComponentInParent<PlayerStats>()._team.Value == Team.FreeForAll))
                //{
                //    _hit.transform.GetComponent<PlayerStats>().TakeDamageServerRpc(_gun._damage);
                //    StartCoroutine(Hit());
                //}

                //else
                //{
                //    StartCoroutine(HitTeam());
                //}

                _hit.transform.GetComponent<PlayerStats>().TakeDamageServerRpc(_gun._damage);

                StartCoroutine(Hit());
            }

            _bullet = Instantiate(_gun._hitEffect, _hit.point, transform.rotation);
            _bullet.GetComponent<Bullet>().SetTrail(_mainGun.GetComponentInChildren<BarrelLoc>().transform);
        }
    }

    IEnumerator Hit()
    {
        _UI._hit.SetActive(true);

        yield return new WaitForSeconds(.1f);

        _UI._hit.SetActive(false);
    }

    [ServerRpc]
    void ShotServerRpc()
    {
        var instance = Instantiate(_UX._sound, transform);
        var instanceNetworkObject = instance.GetComponent<NetworkObject>();
        instanceNetworkObject.Spawn();
    }

    public IEnumerator HitTeam()
    {
        _UI._teamMate.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        _UI._teamMate.SetActive(false);
    }

    Quaternion target, rotationX, rotationY;

    public void Shake()
    {
        rotationX = Quaternion.AngleAxis(100, Vector3.right);
        rotationY = Quaternion.AngleAxis(100, Vector3.up);

        target = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, target, _gun._weaponFlick * Time.deltaTime);
    }
}