using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ActionCover : Node
{
    private Enemy enemy;
  

    private Vector3 _targetCoverPos;
    private bool _isCovering = false;

    public ActionCover(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override State Evaluate()
    {
        Debug.Log("ActionCover");


        // 1. 이미 엄폐 지점에 도착해 있는 경우
        if (_isCovering)
        {
            

            // 엄폐가 완료되었고 할 일을 다했다면 Success 반환 (다음 판단으로 넘어감)
            return State.Success;
        }

        // 2. 엄폐 지점 찾기
        _targetCoverPos = FindBestCoverPoint();

        if (_targetCoverPos != Vector3.zero)
        {
            enemy.MoveTo(_targetCoverPos);

            // 도착 여부 체크
            float dist = Vector3.Distance(enemy.transform.position, _targetCoverPos);
            if (dist < 0.5f)
            {
                _isCovering = true;
                return State.Running;
            }
            return State.Running;
        }

        return State.Failure; // 엄폐 지점을 못 찾으면 실패
    }

    private Vector3 FindBestCoverPoint()
    {
        // 간단한 전술적 엄폐 지점 계산:
        // 플레이어의 반대 방향으로 일정 거리 떨어진 곳 중 NavMesh 위 지점 탐색
        Vector3 dirFromPlayer = (enemy.transform.position - enemy.Target.position).normalized;
        Vector3 hidePos = enemy.transform.position + (dirFromPlayer * 5f);

        NavMeshHit hit;
        // 5미터 반경 내에서 가장 가까운 NavMesh 지점을 찾음
        if (NavMesh.SamplePosition(hidePos, out hit, 5f, NavMesh.AllAreas))
        {
            return hit.position;
        }

        return Vector3.zero;
    }
}