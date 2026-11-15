using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionAcceleratedApproach : Node
{
    private readonly Enemy enemy;
    private readonly float dashSpeed;
    private readonly float cooldownDuration;

    private Vector3 targetDashPosition;
    private float lastDashTime;
    private bool isDashing = false;

    public ActionAcceleratedApproach(Enemy enemy, float dashSpeed = 15f, float cooldownDuration = 1f)
    {
        this.enemy = enemy;
        this.dashSpeed = dashSpeed;
        this.cooldownDuration = cooldownDuration;

        this.lastDashTime = -cooldownDuration;

        this.ScoreModifier = () =>
        {
            if (isDashing)
            {
                return 60f;
            }

            if (enemy.CanSeePlayer &&
                enemy.GetDistanceFromPlayer() >= enemy.enemyData.optimalRange &&
                Time.time - lastDashTime >= cooldownDuration)
            {
                return 60f;
            }

            return 0f;
        };
    }

    public override State Evaluate()
    {
        Debug.Log("대쉬!");
        if (enemy.Target == null)
        {
            ResetDashState();
            CurrentState = State.Failure;
            return CurrentState;
        }

        // 대시 시작 순간에만 최초 1회 실행되는 로직
        if (!isDashing)
        {
            StartDash();
        }

        // 대시 중에는 실시간으로 플레이어를 조준 (몸 회전)
        LookAtTargetImmediately();

        // 목적지(처음 발견한 위치)에 거의 도달했는지 검사
        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            ResetDashState();
            CurrentState = State.Success;
            return CurrentState;
        }

        CurrentState = State.Running;
        return CurrentState;
    }

    private void StartDash()
    {
        isDashing = true;
        lastDashTime = Time.time; // 쿨타임 타이머 가동 시작
        targetDashPosition = enemy.Target.position; // 처음 발견한 위치 락온

        if (enemy.agent != null)
        {
            enemy.agent.acceleration = 9999f; // 브레이크 해제
            enemy.SetMoveSpeed(dashSpeed);
            enemy.agent.SetDestination(targetDashPosition);
        }

        if (enemy.BodyState != EEnemyBodyState.Moving)
        {
            enemy.StopScan();
            enemy.SetBodyState(EEnemyBodyState.Moving);
        }
    }

    private void LookAtTargetImmediately()
    {
        Vector3 direction = (enemy.Target.position - enemy.transform.position).normalized;
        direction.y = 0f;

        if (direction != Vector3.zero)
        {
            enemy.transform.rotation = Quaternion.LookRotation(direction);
        }
        enemy.UpdateGhostPosition(enemy.Target);
    }

    private void ResetDashState()
    {
        isDashing = false;
        if (enemy.agent != null)
        {
            enemy.CheckAggroState();
            float baseSpeed = enemy.unitStat.GetStat(EStatType.Speed);
            enemy.SetMoveSpeed(baseSpeed > 0 ? baseSpeed : enemy.enemyData.spd);
        }
    }
}