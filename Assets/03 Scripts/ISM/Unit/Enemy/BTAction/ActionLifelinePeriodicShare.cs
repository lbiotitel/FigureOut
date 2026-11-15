using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionLifelinePeriodicShare : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private EEnemyType enemyType;
    private float shareRange;
    private float shareInterval;    // 공유 주기 (초단위, 예: 1.0f = 1초)
    private float lastShareTime;    // 마지막으로 공유한 시간


    public ActionLifelinePeriodicShare(Enemy enemy, EEnemyType type = EEnemyType.None, float lifelinePoint = 0.5f, float shareRange = 10f,float shareInterval = 1.0f)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
        this.shareRange = shareRange;
        this.enemyType = type;
        this.shareInterval = shareInterval;
        this.lastShareTime = -shareInterval;
        ScoreModifier = () =>
        {

            float hpPercent = enemy.unitStat.GetStat(EStatType.Hp) / enemy.unitStat.GetStat(EStatType.MaxHp);
            if (hpPercent <= lifelinePoint)
            {
                // 2. 주기 검사: 마지막으로 무전을 친 지 1초가 지났는가?
                if (Time.time - lastShareTime >= shareInterval)
                {
                    // 1초가 지난 바로 '그 순간'에만 최우선 순위(95점)를 할당!
                    return 95f;
                }
            }


            return 0f;
        };
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        Debug.Log("ActionLifelinePeriodicShare");
        enemy.SharePlayerPosition(enemyType, shareRange);
        lastShareTime = Time.time;

        // 3. 액션 제어 (무전 치는 순간 제자리에 정지)
        if (enemy.BodyState != EEnemyBodyState.Action)
        {
            enemy.Stop();
            enemy.StopScan();
            enemy.SetBodyState(EEnemyBodyState.Action);
        }
        enemy.FaceTarget();

        // 이 노드는 단 한 프레임만 95점을 찍고 바로 0점으로 내려갈 것이므로 Success를 리턴해 마무리합니다.
        CurrentState = State.Success;
        return CurrentState;
    }
}