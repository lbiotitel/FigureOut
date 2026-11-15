using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSpawnLookingAround : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
   

    public ActionSpawnLookingAround(Enemy enemy)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        if (enemy.mySpawner == null)
        {
            Debug.LogWarning($"{enemy.gameObject.name}: 스포너 참조가 없습니다!");
            return State.Failure;
        }
        Vector2 enemyPosXZ = new Vector2(enemy.transform.position.x, enemy.transform.position.z);
        Vector2 spawnerPosXZ = new Vector2(enemy.mySpawner.transform.position.x, enemy.mySpawner.transform.position.z);
        float distance = Vector2.Distance(enemyPosXZ, spawnerPosXZ);

        // 3. 거리가 일정 이상이면 스포너 위치로 MoveTo
        if (distance > 2)
        {
            enemy.MoveTo(enemy.mySpawner.transform.position);
        }
        else
        {
            if (enemy.agent.hasPath)
            {
                enemy.agent.ResetPath(); // 목적지 도달했으니 경로 초기화
            }
            if (!enemy.isScan)
            {
                enemy.StartScan();
            }
        }
        CurrentState = State.Running;
        return CurrentState;
    }
}