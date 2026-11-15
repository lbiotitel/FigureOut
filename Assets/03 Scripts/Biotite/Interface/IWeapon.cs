using UnityEngine;

public interface IWeapon
{
    public void Init(Unit ownUnit, WeaponData weaponData);
    public void Use(Transform targetTransform);
}