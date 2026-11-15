using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class ActionPatrol : Node
{
    private Enemy enemy;
    private Vector3 _currentDestination = Vector3.zero;

    private float _waitUntil;
    private float _maxWaitTime = 5f;
    private bool _isWaiting = false;

    public ActionPatrol(Enemy enemy)
    {
        this.enemy = enemy;
    }

    public override State Evaluate()
    {
       // Debug.Log("ActionPatrol");
        if (_isWaiting && enemy.BodyState != EEnemyBodyState.Scanning)
        {
            ResetPatrol();
        }
        else if (!_isWaiting && enemy.BodyState != EEnemyBodyState.Moving)
        {
            ResetPatrol();
        }

        if (_currentDestination == Vector3.zero)
        {
            StartNewPatrol();
            return State.Running;
        }

        if (_isWaiting)
        {
            // 목표 시간이 지나면 다시 이동 시작
            if (Time.time >= _waitUntil)
            {
                StartNewPatrol();
            }
            return State.Running;
        }




        if (!enemy.agent.pathPending)
        {
            if (enemy.agent.hasPath && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
            {
                if (enemy.agent.velocity.sqrMagnitude < 0.01f)
                {
                    EnterWaitState();
                }
            }
            else if (!enemy.agent.hasPath && enemy.BodyState == EEnemyBodyState.Moving)
            {
                //Debug.LogWarning("[순찰] 길을 찾을 수 없는 목적지입니다. 제자리 대기(스캔)로 전환합니다.");
                EnterWaitState();
            }
        }

        return State.Running;
    }


    private void StartNewPatrol()
    {
        _isWaiting = false;
        float radius = enemy.currentPatrolR > 0.1f ? enemy.currentPatrolR : enemy.enemyData.patrolRMax;
        // 새로운 목적지 탐색
        _currentDestination = enemy.GetNavigableRandomPoint(enemy.patrolPoint, enemy.currentPatrolR);

        enemy.MoveTo(_currentDestination);
    }

    private void EnterWaitState()
    {
        _isWaiting = true;
        _waitUntil = Time.time + _maxWaitTime;

        enemy.StartScan();
    }

    private void ResetPatrol()
    {
        _currentDestination = Vector3.zero;
    }
}