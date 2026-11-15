using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//즉시 발동되는 아이템의 효과를 정의합니다.
[Serializable]
public class ImpactEffect : ItemEffect
{
    [SerializeField] private List<SStatContainer> statContainers;

    public override void ExcuteEffect(Unit targetUnit)
    {
        var unitStat = targetUnit.unitStat;

        foreach (var stat in statContainers)
        {
            unitStat.AddModifire(stat.statType, new StatModifier(stat.value, stat.modifireType, this));
        }
    }

    public override string BuildDescription()
    {

        string res = "\n효과 : ";

        for (int i = 0; i < statContainers.Count; i++)
        {
            string effect = statContainers[i].value > 0 ? "증가" : "감소";
            res += $"{statContainers[i].statType.ToString()}을 {statContainers[i].value}만큼 {effect}\n";
        }

        return res;
    }
}
