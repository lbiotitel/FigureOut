using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DamageCalculator 
{
    // 공격자 데이터와 방어자 스탯을 받아서 최종 데미지만 반환
    public static int Calculate(float attackerAtk, UnitStat victimStat, bool isCritical = false, float critDmg=1.5f)
    {
        float finalDamage;
        float incomingDamage = victimStat.GetStat(EStatType.IncomingDamage);
        if (isCritical)
        {
            float defense = victimStat.GetStat(EStatType.CritDef);
            //데미지 계산 공식 (예: 방어율 방식)
            float damageReduction = 100f / (100f + defense);
            finalDamage = attackerAtk*critDmg * damageReduction*incomingDamage;

        }
        else
        {
            float defense = victimStat.GetStat(EStatType.Def);

            //데미지 계산 공식 (예: 방어율 방식)
            float damageReduction = 100f / (100f + defense);
            finalDamage= attackerAtk * damageReduction*incomingDamage;
        }

        // 소수점 처리 후 반환
        return Mathf.RoundToInt(finalDamage);
    }
}
