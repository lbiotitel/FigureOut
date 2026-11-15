using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Threading;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour,IWeapon, IEquipment
{
    protected WeaponData weaponData;
    protected Transform firePos;
    protected bool isAttack = false;
    protected Unit ownUnit;
    protected float reloadTime;

    public virtual void Init(Unit ownUnit,WeaponData weaponData)
    {
        this.ownUnit = ownUnit;
        this.weaponData = weaponData;   

        firePos = Utils.Find("FirePos",transform);

    }


    public void Equip()
    {
        var statList = weaponData.EquipmentStats;

        foreach (var stat in statList)
        {
            ownUnit.unitStat.AddModifire(stat.statType, new StatModifier(stat.value, stat.modifireType, this));
        }

        reloadTime = ownUnit.unitStat.GetStat(EStatType.ReloadSpeed);
    }

    public void UnEquip()
    {
        ownUnit.unitStat.RemoveAllModifiresFromSource(this);
    }


    public virtual void Use(Transform targetTransform)
    {
        if (isAttack) return;
        OnUse(targetTransform);
        isAttack = true;
        CoAttackDelay();
    }

    protected abstract void OnUse(Transform targetUnit);


    protected async void CoAttackDelay()
    {
        if (this == null) return;

        var cancelToken = this.GetCancellationTokenOnDestroy();

        try
        {
            float atkSpeed = ownUnit.unitStat.GetStat(EStatType.AtkSpeed);
            await UniTask.WaitForSeconds(1f/atkSpeed, cancellationToken: cancelToken);
            isAttack = false;
        }
        catch (OperationCanceledException)
        {
            Logger.Log("유니티 태스크가 안전하게 해제됬습니다.");
        }
    }

}
