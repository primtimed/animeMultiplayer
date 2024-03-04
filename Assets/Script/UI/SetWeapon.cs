using UnityEngine;

public class SetWeapon : MonoBehaviour
{
    public GameObject _weapon;

    BaseGun _gun;

    private void Start()
    {
        _gun = _weapon.GetComponentInChildren<BaseGun>();
    }

    public void Setweapon(GunStats gun)
    {
        _gun._gun = gun;
    }
}
