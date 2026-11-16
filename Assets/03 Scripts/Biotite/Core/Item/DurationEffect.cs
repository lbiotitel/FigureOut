using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//지속 아이템 효과를 정의합니다.
[Serializable]
public class DurationEffect : ItemEffect
{
    [SerializeField] private float duration;
    [SerializeField] private float tickInterval;
    [SerializeField] List<SStatContainer> statContainers;


    public override void ExcuteEffect(Unit targetUnit)
    {
        var unitStat = targetUnit.unitStat;

        ApplyEffect(unitStat);
    }


    //효과 적용
    private async void ApplyEffect(UnitStat unitStat)
    {
        float t = 0;

        while (t < duration)
        {
            foreach (var stat in statContainers)
            {
                unitStat.AddModifire(stat.statType, new StatModifier(stat.value, stat.modifireType, this));
                await UniTask.WaitForSeconds(tickInterval);
                t += tickInterval;
            }
        }
    }

    public override string BuildDescription()
    {

        string res = "\n효과 : ";

        for (int i = 0; i < statContainers.Count; i++)
        {
            string effect = statContainers[i].value > 0 ? "증가" : "감소";
            res += $"{statContainers[i].statType.ToString()}을 {duration}초동안 {tickInterval}주기로 {statContainers[i].value}만큼 {effect}\n";
        }

        return res;
    }
    
}
