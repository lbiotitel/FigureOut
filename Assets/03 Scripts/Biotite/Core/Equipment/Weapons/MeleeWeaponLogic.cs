using UnityEngine;

public class MeleeWeaponLogic : WeaponBase
{

    public override void Init(Unit ownUnit, WeaponData weaponData)
    {
        base.Init(ownUnit, weaponData);

    }


    protected override void OnUse(Transform targetTransform)
    {
        throw new System.NotImplementedException();
    }
}
