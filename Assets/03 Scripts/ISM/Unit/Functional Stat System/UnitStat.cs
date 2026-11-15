using System;
using System.Collections.Generic;
using UnityEngine;

public enum EStatType
{
    Atk,
    CriDamage,
    DamageCorrection,
    IncomingDamage,
    AtkSpeed,
    AtkRange,
    Def,
    CritDef,
    Speed,
    SprintSpeed,
    ReloadSpeed,
    Luck,
    MaxHp,
    Hp,
    //player
    MaxStamina,
    MaxHungry,
    MaxThirsty,
    Stamina,
    Hungry,
    Thirsty,
    HungryDecayRate,
    ThirstyDecayRate,
    StaminaRegenRate,
    playerServivalPenaltyDamage,
    playerServivalPenaltyDuration,
    //enemy
    MaxAggro,
    Aggro,
    //Element
    IgniteAtkCount,//Attribute value added
    MaxIgniteGauge,//The critical point where the effect comes in
    IgniteGauge, //The attribute value applied to me
    IgniteDecayRate, //Amount of ElementGauge decrease

    FreezeAtkCount,//Attribute value added
    MaxFreezeGauge,//The critical point where the effect comes in
    FreezeGauge, //The attribute value applied to me
    FreezeDecayRate, //Amount of ElementGauge decrease

    IrradiationAtkCount,//Attribute value added
    MaxIrradiationGauge,//The critical point where the effect comes in
    IrradiationGauge, //The attribute value applied to me
    IrradiationDecayRate, //Amount of ElementGauge decrease

    CorrosionAtkCount,//Attribute value added
    MaxCorrosionGauge,//The critical point where the effect comes in
    CorrosionGauge, //The attribute value applied to me
    CorrosionDecayRate, //Amount of ElementGauge decrease

    MarkedAtkCount,//Attribute value added
    MaxMarkedGauge,//The critical point where the effect comes in
    MarkedGauge, //The attribute value applied to me
    MarkedDecayRate, //Amount of ElementGauge decrease

    InjuryAtkCount,//Attribute value added
    MaxInjuryGauge,//The critical point where the effect comes in
    InjuryGauge, //The attribute value applied to me
    InjuryDecayRate, //Amount of ElementGauge decrease

}

public class UnitStat//���ֿ� �� ������ ����
{
    protected Dictionary<EStatType, StatBase> statDict = null;

    public event Action<float, float, float> OnHpChanged;

    public float CurrentHp => GetResourceStat(EStatType.Hp)?.Value ?? 0f;

    private UnitType unitType;
    public void Init(UnitData unitData)
    {
        statDict = new Dictionary<EStatType, StatBase>();

        Stat maxHp = new Stat(unitData.maxHp);

        statDict.Add(EStatType.MaxHp, maxHp); //�⺻ �ִ� ü��
        statDict.Add(EStatType.Hp, new ResourceStat(maxHp));
        statDict.Add(EStatType.Atk, new Stat(unitData.atk)); //�⺻ ���ݷ�
        statDict.Add(EStatType.CriDamage, new Stat(unitData.criDamage)); //ũ��Ƽ�� ������ ����
        statDict.Add(EStatType.Def, new Stat(unitData.def)); //�⺻ ����
        statDict.Add(EStatType.CritDef, new Stat(unitData.criDef)); //ũ��Ƽ�� ����
        statDict.Add(EStatType.DamageCorrection, new Stat(unitData.damageCorrection));//������ ������
        statDict.Add(EStatType.IncomingDamage, new Stat(unitData.incomingDamage));//�޴� ���� ������
        statDict.Add(EStatType.AtkRange, new Stat(unitData.atkRange));//���� ��Ÿ�
        statDict.Add(EStatType.AtkSpeed, new Stat(unitData.atkSpeed));//�ʴ� �߻� Ƚ��
        statDict.Add(EStatType.Speed, new Stat(unitData.spd));//�⺻ �̵� �ӵ�
        statDict.Add(EStatType.ReloadSpeed, new Stat(unitData.reloadSpeed));//������ �ð�

        //Element
        statDict.Add(EStatType.IgniteAtkCount, new Stat(unitData.IgniteAtkCount));
        Stat maxIgniteGauge = new Stat(unitData.MaxIgniteGauge);
        statDict.Add(EStatType.MaxIgniteGauge, maxIgniteGauge);
        statDict.Add(EStatType.IgniteGauge, new ResourceStat(maxIgniteGauge,true));
        statDict.Add(EStatType.IgniteDecayRate, new Stat(1));

        statDict.Add(EStatType.FreezeAtkCount, new Stat(unitData.FreezeAtkCount));
        Stat maxFreezeGauge = new Stat(unitData.MaxFreezeGauge);
        statDict.Add(EStatType.MaxFreezeGauge, maxFreezeGauge);
        statDict.Add(EStatType.FreezeGauge, new ResourceStat(maxFreezeGauge,true));
        statDict.Add(EStatType.FreezeDecayRate, new Stat(1));

        statDict.Add(EStatType.IrradiationAtkCount, new Stat(unitData.IrradiationAtkCount));
        Stat maxIrradiationGauge = new Stat(unitData.MaxIrradiationGauge);
        statDict.Add(EStatType.MaxIrradiationGauge, maxIrradiationGauge);
        statDict.Add(EStatType.IrradiationGauge, new ResourceStat(maxIrradiationGauge, true));
        statDict.Add(EStatType.IrradiationDecayRate, new Stat(1));

        statDict.Add(EStatType.CorrosionAtkCount, new Stat(unitData.CorrosionAtkCount));
        Stat maxCorrosionGauge = new Stat(unitData.MaxCorrosionGauge);
        statDict.Add(EStatType.MaxCorrosionGauge, maxCorrosionGauge);
        statDict.Add(EStatType.CorrosionGauge, new ResourceStat(maxCorrosionGauge, true));
        statDict.Add(EStatType.CorrosionDecayRate, new Stat(1));

        statDict.Add(EStatType.InjuryAtkCount, new Stat(unitData.InjuryAtkCount));
        Stat maxInjuryGauge = new Stat(unitData.MaxInjuryGauge);
        statDict.Add(EStatType.MaxInjuryGauge, maxInjuryGauge);
        statDict.Add(EStatType.InjuryGauge, new ResourceStat(maxInjuryGauge, true));
        statDict.Add(EStatType.InjuryDecayRate, new Stat(1));

        statDict.Add(EStatType.MarkedAtkCount, new Stat(unitData.MarkedAtkCount));
        Stat maxMarkedGauge = new Stat(unitData.MaxMarkedGauge);
        statDict.Add(EStatType.MaxMarkedGauge, maxMarkedGauge);
        statDict.Add(EStatType.MarkedGauge, new ResourceStat(maxMarkedGauge, true));
        statDict.Add(EStatType.MarkedDecayRate, new Stat(1));

        //hp를 변경 감지
        ResourceStat hp = statDict[EStatType.Hp] as ResourceStat;
        hp.OnValueChanged += (currentHp, prevHp) => OnHpChanged?.Invoke(currentHp, prevHp, GetStat(EStatType.MaxHp));

        unitType = unitData.UnitType;
    }

    public void AddStat(EStatType type, StatBase stat)
    {
        if (!statDict.ContainsKey(type))
        {
            statDict.Add(type, stat);
        }
    }

    public float GetStat(EStatType statType)
    {
        float result = 0;
        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            result = stat.Value;
        }

        return result;
    }

    public void ConsumeStat(EStatType statType, float value)
    {
        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            stat.ConsumeStat(value);
        }
    }

    public StatBase GetStatBase(EStatType statType)
    {

        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            return stat;
        }

        return null;
    }

    public void AddModifire(EStatType statType, StatModifier statModifire)
    {
        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            stat.AddModifier(statModifire);//���� ���� ����Ʈ�� �߰��Ѵ�
        }
        else
        {

            Debug.Log("스탯 베이스로 전환 불가함"+statType);
        }
    }

    public void AddAllModifire(List<KeyValuePair<EStatType, StatModifier>> list)
    {
        if (list == null) return; // ����Ʈ�� ��������� �׳� ���ư���
        foreach (var MF in list)
        {
            AddModifire(MF.Key, MF.Value);
        }
    }

    public void RemoveModifierFromValue(EStatType statType, StatModifier value)
    {
        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            stat.RemoveModifier(value);
        }
    }

    public void RemoveAllModifiresFromSource(object source)
    {
        
        foreach (var stat in statDict.Values)
        {
            stat.RemoveAllModifier(source); 
        }
    }
    
    public ResourceStat GetResourceStat(EStatType statType)
    {
        if (statDict.TryGetValue(statType, out StatBase stat))
        {
            return stat as ResourceStat;
        }

        return null;
    }


}