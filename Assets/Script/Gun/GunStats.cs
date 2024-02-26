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
    Shotgun
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

    public WeaponType _weaponType;
    public FireMode _fireMode;
}