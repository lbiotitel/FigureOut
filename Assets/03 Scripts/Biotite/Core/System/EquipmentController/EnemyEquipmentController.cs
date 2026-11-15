using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEquipmentController : EquipmentController
{


    public override void Equip(EquipmentData equipmentData)
    {
        if (equipmentData == null) return;

        equipmentData = Instantiate(equipmentData);

        if (equipmentData is WeaponData)
        {
            WeaponData weaponData = equipmentData as WeaponData;
            WeaponBase weapon = weaponFactory.Create<WeaponBase>(weaponData);
            currentWeapon = weapon;
            weapon.transform.SetParent(handTransform);
            weapon.transform.position = handTransform.position;
            weapon.transform.rotation = handTransform.rotation;
        }
    }
}
