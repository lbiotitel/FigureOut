using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmorBase : MonoBehaviour, IEquipment
{
    protected Unit ownUnit;
    protected EquipmentData equipmentData;

    public virtual void Init(Unit ownUnit,EquipmentData equipmentData)
    { 
        this.equipmentData = equipmentData;
        this.ownUnit = ownUnit;
    }



    //장착 하고 스탯 추가
    public virtual void Equip()
    {
        var statList = equipmentData.EquipmentStats;

        foreach (var stat in statList)
        {
            ownUnit.unitStat.AddModifire(stat.statType, new StatModifier(stat.value,stat.modifireType,this));
        }
    }


    //해제 하고 스탯 감소
    public virtual void UnEquip()
    {
        ownUnit.unitStat.RemoveAllModifiresFromSource(this);
    }
}
