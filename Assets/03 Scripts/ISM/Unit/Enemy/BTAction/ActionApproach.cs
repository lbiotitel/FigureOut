using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionApproach : Node
{
    private Enemy enemy;
    

    // 생성자에서 컨트롤러 주입
    public ActionApproach(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override State Evaluate()
    {
        Debug.Log("ActionApproach");
        if (enemy.Target == null) return State.Failure;

        float distance = Vector3.Distance(enemy.transform.position, enemy.Target.position);
        float optimal = enemy.enemyData.optimalRange;

        // 1. 이미 최적 거리 근처라면 (오차 범위 2m) 성공으로 반환하고 정지
        if (distance <= optimal + 2f && distance >= optimal - 2f)
        {
            enemy.agent.ResetPath();
            return State.Success;
        }

        enemy.MoveTo(enemy.ghostTarget.position);
        
        return State.Running;
    }
}
