using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ActionLifelineBuff : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private bool hasBuff = false; // 
    private StatModifier statModifier;
    public ActionLifelineBuff(Enemy enemy,float lifelinePoint)
    {
        statModifier = new StatModifier(0.3f, EStatModifireType.Percent, enemy);
        this.enemy = enemy;
        ScoreModifier = () =>
        {
            if (hasBuff) return 0f;
            float hpPercent = enemy.unitStat.GetStat(EStatType.Hp) / enemy.unitStat.GetStat(EStatType.MaxHp);

            if (hpPercent <= lifelinePoint)
            {
                return 9999f;
            }
            // 조건 미달 시 0점
            return 0f;
        };
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        hasBuff = true;
        enemy.unitStat.AddModifire(EStatType.Atk,statModifier);
        return State.Failure; // 혹시 모를 안전장치

    }
}