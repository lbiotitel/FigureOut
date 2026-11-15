using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;

public class ActionMoveToLastKnown : Node//클래스 이름 바꾸고
{
    private Enemy enemy;
    private bool isPathSet = false;
    private NavMeshAgent agent;
    private bool isSearching = false;

    public ActionMoveToLastKnown(Enemy enemy)//생성자 이름도 맘에 드는걸로 바꿔라!
    {
        this.enemy = enemy;
        this.agent=enemy.agent;
    }

    //기능 해당 노드가 가중치 충족되어 사용될때 기동할 기능을 적으면 된다!
    public override State Evaluate()
    {
        if (enemy.ghostTarget == null)
        {
            CurrentState= State.Failure;
            return CurrentState;
        }

        // 2. 최초 진입 시 목적지 설정 (매 프레임 호출 방지)
        if (!isPathSet)
        {
            enemy.SetBodyState(EEnemyBodyState.Moving);
            enemy.MoveTo(enemy.ghostTarget.position);
            isPathSet = true;
        }

        // 3. 경로 계산 중이거나 아직 이동 중인지 체크
        if (agent.pathPending)
        {
            CurrentState = State.Running;
            return CurrentState;
        }

        // 4. 목적지에 도착했는지 확인 (남은 거리가 정지 거리 이하인가?)
        // 혹은 에이전트가 더 이상 갈 수 없는 고립된 경로인지 체크
        if (agent.remainingDistance <= agent.stoppingDistance + 0.2f)
        {
            // 목적지 도달 성공! 
            enemy.Stop();
            isPathSet = false; // 플래그 초기화
            if (!isSearching)
            {
                // 도착했으니 제자리에 서서 주변을 두리번거리는(Scanning) 상태로 바꿉니다.
                enemy.StartScan(); // 현재 바라보는 방향을 회전 기준점으로 저장
                enemy.SetBodyState(EEnemyBodyState.Scanning);
                isSearching = true;
            }
            else 
            {
                enemy.SetBodyState(EEnemyBodyState.Scanning);
            }
            Debug.Log($"마지막 목격 장소({enemy.ghostTarget.position}) 도착! 수색 모드 전환.");
            CurrentState = State.Success;
            return CurrentState;
        }

        // 5. 아직 가고 있는 중이라면 계속 Running
        CurrentState = State.Running;
        return CurrentState;
    }


   
}