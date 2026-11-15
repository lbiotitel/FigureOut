using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ActionLifelineEscape : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private bool hasRolledDice = false; // 주사위를 굴렸는가? (최초 1회 보장)
    private bool isFleeing = false;     // 현재 도망치는 중인가?
    public ActionLifelineEscape(Enemy enemy,float lifelinePoint)
    {
        this.enemy = enemy;
        ScoreModifier = () =>
        {
            if (isFleeing) return 9999f;

            // 
            if (hasRolledDice) return 0f;
           
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
        if (!hasRolledDice)
        {
            hasRolledDice = true; 
            float randomVal = Random.Range(0f, 100f);

            if (randomVal <= 70f) // 70% 확률 당첨! (도망)
            {
                isFleeing = true;
               
            }
            else 
            {
               
                CurrentState = State.Success;
                return CurrentState;
            }
        }

        if (isFleeing)
        {
            CurrentState = State.Running;
            enemy.MoveAwayFromPlayer(30f);
            if (enemy.GetDistanceFromPlayer()>30f)
            {
                isFleeing = false; // 도망 상태 해제
                CurrentState = State.Success; // 노드 완전 종료
            }

            return CurrentState;
        }

        return State.Failure; // 혹시 모를 안전장치

    }
}