using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class WeaponFactory : IEquipmentFactory
{
    private Unit ownUnit;

    public void Init(Unit ownUnit)
    {
        this.ownUnit = ownUnit;
    }

    public T Create<T>(ItemData itemData)
    {
        WeaponData weaponData = itemData as WeaponData;

        GameObject weapon = new GameObject();
        weapon.name = weaponData.ItemName;
        SetUp(weapon.gameObject, weaponData);
        weapon.AddComponent<RecoilController>();

        if (ownUnit.unitType == UnitType.Player)
        {
            PlayerRangedWeapon pw = weapon.gameObject.AddComponent<PlayerRangedWeapon>();
            pw.Init(ownUnit, weaponData);
        }
        else 
        {
            EnemyRangedWeapon ew = weapon.gameObject.AddComponent<EnemyRangedWeapon>();
            ew.Init(ownUnit, weaponData);
        }

        return weapon.GetComponent<T>();
    }



    private void SetUp(GameObject weapon, WeaponData weaponData)
    {
        GameObject weaponModel = GameObject.Instantiate(weaponData.ModelPrefab, weapon.transform);
        weaponModel.transform.localPosition = new Vector3(-weaponData.GrabPos.x, weaponData.GrabPos.y, -weaponData.GrabPos.z);
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;

        GameObject firePosObject = new GameObject("FirePos");
        firePosObject.transform.SetParent(weaponModel.transform);
        firePosObject.transform.localPosition = weaponData.FirePos;
    }

}
