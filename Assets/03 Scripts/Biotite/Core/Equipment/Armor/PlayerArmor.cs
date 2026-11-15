using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerArmor : ArmorBase
{
    // Start is called before the first frame update

    private PlayerUnit playerUnit;

    public override void Init(Unit ownUnit, EquipmentData equipmentData)
    {
        base.Init(ownUnit, equipmentData);
        playerUnit = ownUnit as PlayerUnit;

        if (equipmentData.CurrnetDurability > 0)
        {
            playerUnit.OnPlayerhit += HandleOnPlayerHit;
        }
    }

    //������ ���� ���
    private void HandleOnPlayerHit(float damage, bool isCri, bool isDot)
    {
        if (equipmentData.EquipmentType == EEquipmentType.Medal || isDot) return;

        float constdurabilityLoss = isCri == true ? 0.1f : 0.05f;
        float durabilityLoss = damage * constdurabilityLoss;

        equipmentData.CurrnetDurability -= durabilityLoss;

        Managers.Event.NotifyEvent<DurabilityChangedEvnet>(new DurabilityChangedEvnet()
        {
            equipmentType = equipmentData.EquipmentType,
            currentDurability = equipmentData.CurrnetDurability,
            maxDurability = equipmentData.MaxDurability
        });

        if (equipmentData.CurrnetDurability <= 0) UnEquip();
    }

    public override void Equip()
    {
        if (equipmentData.CurrnetDurability <= 0) return;
        base.Equip();
    }

    public override void UnEquip()
    {
        base.UnEquip();
        playerUnit.OnPlayerhit -= HandleOnPlayerHit;
    }

}
