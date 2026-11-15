using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionEncounter : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private bool hasRolledDice = false; // 주사위를 굴렸는가? (최초 1회 보장)
    private bool isFollowing = false;

    private float trackStartTime = 0f;
    private const float MaxTrackingTime = 5.0f;
    public ActionEncounter(Enemy enemy)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
        ScoreModifier = () =>
        {
            if (!enemy.CanSeePlayer&&!hasRolledDice) return 0f;
            if (isFollowing&&enemy.unitStat.GetStat(EStatType.Hp)>=enemy.unitStat.GetStat(EStatType.MaxHp)) return 9999f;

            if (hasRolledDice) return 0f;
            else return 9999f;

        };
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        if (!hasRolledDice)
        {
            hasRolledDice = true;
            float randomVal = Random.Range(0f, 100f);

            if (randomVal <= 50f) // 50% 확률 당첨! (미행)
            {
                isFollowing=true;
                trackStartTime = Time.time;
                //추적 당첨시 그 시간 기록
            }
            else
            {

                CurrentState = State.Success;
                return CurrentState;
            }
        }

        if (isFollowing)
        {
            // [실제 기능] 호출 주기(0.1초)마다 플레이어 위치 갱신
            if (enemy.Target != null)
            {
                enemy.UpdateGhostPosition(enemy.Target);
                enemy.FaceTarget();
                enemy.MoveTo(enemy.Target.position);
            }

            // 현재 시간에서 시작 시간을 빼서 '정확히 5초'가 지났는지 체크!
            if (Time.time - trackStartTime >= MaxTrackingTime)
            {
                isFollowing = false;
                CurrentState = State.Success;
                return CurrentState;
            }

            CurrentState = State.Running;
            return CurrentState;
        }

        return State.Running; //상태가 실행 중인지, 실패했는지, 성공했는지 반환
    }
}