using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPeriodicShare : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private EEnemyType enemyType;
    private float shareRange;
    private float shareInterval;    // 공유 주기 (초단위, 예: 1.0f = 1초)
    private float lastShareTime;    // 마지막으로 공유한 시간


    public ActionPeriodicShare(Enemy enemy, EEnemyType type = EEnemyType.None, float shareRange = 10f, float shareInterval = 1.0f)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
        this.shareRange = shareRange;
        this.enemyType = type;
        this.shareInterval = shareInterval;
        this.lastShareTime = -shareInterval;
        ScoreModifier = () =>
        {
            if (enemy.CanSeePlayer&&Time.time - lastShareTime >= shareInterval)
            {
                return 999f;
            }
            return 0f;
        };
    }

    public override State Evaluate()
    {
        Debug.Log("ActionLifelinePeriodicShare");
        enemy.SharePlayerPosition(enemyType, shareRange);
        lastShareTime = Time.time;
       
        CurrentState = State.Success;
        return CurrentState;
    }
}