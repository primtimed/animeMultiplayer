using System;
using UnityEngine;

public enum FireMode
{
    Single,
    Auto,
    Burst
}

public enum WeaponType
{
    Standaard,
    Sniper,
    Shotgun,
    Pistol
}

[CreateAssetMenu(fileName = "GunStats", menuName = "GunStats")]
public class GunStats : ScriptableObject
{
    public string _name;

    public int _ammo;
    public float _fireRate;
    public float _reloadSpeed;
    public float _weaponSpretAmount;
    public float _damage;

    [Header("")]
    public Vector2[] _recoil;
    public float _weaponFlick;

    [Header("")]
    public WeaponType _weaponType;
    public FireMode _fireMode;

    [Header("")]
    public GameObject _gun;
    public Texture _gunPNG;
    public GameObject _hitEffect;
    public GameObject _flash;

    [Header("")]

    [ConditionalHide("_fireMode", 2)] public float _burstDelay;
    [ConditionalHide("_weaponType", 1)] public float _zoom;
    [ConditionalHide("_weaponType", 1)] public Texture _scoop;

    public UX _UX = new UX();

    [Serializable]
    public class UX
    {
        public AudioSource _shoot;
        public AudioSource _reload;
    }
}