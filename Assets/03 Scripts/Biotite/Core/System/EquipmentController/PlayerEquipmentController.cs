using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEquipmentController : EquipmentController
{

    [field: SerializeField] public ReloadUI ReloadUI { get; private set; }

    public EquipmentData CurrentHelmetData { get; private set; }
    public EquipmentData CurrentChestData { get; private set; }
    public EquipmentData CurrentMedalData { get; private set; }

    private PlayerUnit Player => ownUnit as PlayerUnit;

    public override void Init(Unit ownUnit)
    {
        base.Init(ownUnit);

        Managers.Event.AddEvent<EquippedEquipmentEvent>(HandleEquipped);
        Managers.Event.AddEvent<UnEquippedEquipmentEvent>(HandleUnEquipped);
    }

    private void OnDisable()
    {
        if (ownUnit.unitType == UnitType.Player)
        {
            Managers.Event.RemoveEvent<EquippedEquipmentEvent>(HandleEquipped);
            Managers.Event.RemoveEvent<UnEquippedEquipmentEvent>(HandleUnEquipped);
        }
    }


    protected void HandleEquipped(EquippedEquipmentEvent data)
    {
        EquipmentData equipmentData = data.itemData as EquipmentData;
        if (equipmentData == null) return;

        Logger.Log(equipmentData.ItemName + "착용");
        Equip(equipmentData);
    }




    protected void HandleUnEquipped(UnEquippedEquipmentEvent data)
    {
        EquipmentData equipmentData = data.itemData as EquipmentData;
        if (equipmentData == null) return;

        Logger.Log($"{equipmentData.ItemName} 장비 해제");

        if (equipmentData.EquipmentType == EEquipmentType.Weapon)
        {
            RemoveClassWeaponBuff(equipmentData as WeaponData);
            currentWeapon.UnEquip();
            Destroy(currentWeapon.gameObject);
        }
        else if (equipmentData.EquipmentType == EEquipmentType.Helmet)
        {
            helmet.UnEquip();
            Destroy(helmet.gameObject);
        }
        else if (equipmentData.EquipmentType == EEquipmentType.Chestplate)
        {
            chestPlate.UnEquip();
            Destroy(chestPlate.gameObject);
        }
        else if (equipmentData.EquipmentType == EEquipmentType.Medal)
        {
            medal.UnEquip();
            Destroy(medal.gameObject);
        }
    }


    public override void Equip(EquipmentData equipmentData)
    {
        if (equipmentData == null) return;

        if (equipmentData is WeaponData)
        {
            WeaponData weaponData = equipmentData as WeaponData;
            WeaponBase weapon = weaponFactory.Create<WeaponBase>(weaponData);
            currentWeapon = weapon;
            weapon.transform.SetParent(handTransform);
            weapon.transform.position = handTransform.position;
            weapon.transform.rotation = handTransform.rotation;
            currentWeapon.Equip();
            ApplyClassWeaponBuff(weaponData);
        }
        else if (equipmentData is ArmorData)
        {
            ArmorData armorData = equipmentData as ArmorData;
            ArmorBase armor = armorFactory.Create<ArmorBase>(armorData);
            if (equipmentData.EquipmentType == EEquipmentType.Helmet)
            {
                helmet = armor;
                armor.transform.SetParent(headTransform);
                armor.transform.localPosition = Vector3.zero;
                helmet.Equip();
                CurrentHelmetData = equipmentData;
            }
            else if (equipmentData.EquipmentType == EEquipmentType.Chestplate)
            {
                chestPlate = armor;
                armor.transform.SetParent(bodyTransform);
                armor.transform.localPosition = Vector3.zero;
                chestPlate.Equip();
                CurrentChestData = equipmentData;
            }
            else if (equipmentData.EquipmentType == EEquipmentType.Medal)
            {
                medal = armor;
                armor.transform.SetParent(bodyTransform);
                armor.transform.localPosition = Vector3.zero;
                medal.Equip();
                CurrentMedalData = equipmentData;
            }
        }
    }

    private void ApplyClassWeaponBuff(WeaponData weaponData)
    {
        if(Player == null) return;

        EClassType playerClass = CharaterSelectData.selectedClassType;

        EWeaponType weaponType = weaponData.WeaponType;

        if (playerClass == EClassType.Farmer && weaponType == EWeaponType.SG)
        {
            Player.PlayerStat.AddModifire(EStatType.MaxStamina, new StatModifier(30f, EStatModifireType.Add, this));
            Player.PlayerStat.AddModifire(EStatType.Def, new StatModifier(10f, EStatModifireType.Add, weaponData));
        }
        else if (playerClass == EClassType.Security && weaponType == EWeaponType.PG)
        {
            Player.PlayerStat.AddModifire(EStatType.DamageCorrection, new StatModifier(0.3f, EStatModifireType.Add, weaponData));
        }
        else if (playerClass == EClassType.Observer && weaponType == EWeaponType.SR)
        {
            Player.PlayerStat.AddModifire(EStatType.AtkRange, new StatModifier(1.1f, EStatModifireType.Percent, weaponData));
            Player.PlayerStat.AddModifire(EStatType.MaxStamina, new StatModifier(-10f, EStatModifireType.Add, weaponData));
        }
        else if (playerClass == EClassType.Hunter && weaponType == EWeaponType.AR)
        {
            Player.PlayerStat.AddModifire(EStatType.AtkSpeed, new StatModifier(0.2f, EStatModifireType.Add, weaponData));
        }
        else if (playerClass == EClassType.Fighter)
        {

        }
    }

    private void RemoveClassWeaponBuff(WeaponData weaponData)
    {
        if (Player == null || weaponData == null) return;

        Player.PlayerStat.RemoveAllModifiresFromSource(weaponData);
    }
}
