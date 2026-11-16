using System;
using UnityEngine;

public abstract class EquipmentController : MonoBehaviour
{
    [SerializeField] protected Transform handTransform;
    [SerializeField] protected Transform headTransform;
    [SerializeField] protected Transform bodyTransform;

    protected WeaponBase currentWeapon;
    protected ArmorBase helmet;
    protected ArmorBase chestPlate;
    protected ArmorBase medal;

    protected WeaponFactory weaponFactory;
    protected ArmorFactory armorFactory;

    protected Unit ownUnit;


    public virtual void Init(Unit ownUnit)
    {
        this.ownUnit = ownUnit;
        weaponFactory = new WeaponFactory();
        weaponFactory.Init(ownUnit);
        armorFactory = new ArmorFactory(); 
        armorFactory.Init(ownUnit);
    }


    public abstract void Equip(EquipmentData equipmentData);

    //무기 사용
    public void UseWeapon(Transform targetTransform) => currentWeapon?.Use(targetTransform);

}
