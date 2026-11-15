using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDashState : PlayerStateBase
{
    private float dashDuration = 0.3f;
    private float currentDashTime = 0f;

    private float dashSpeedMultiplier = 3f;
    private float finalDashSpeed;

    private Vector3 dashDirection;

    public PlayerDashState(PlayerUnit player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        currentDashTime = 0f;

        float currentMoveSpeed = player.unitStat.GetStat(EStatType.Speed);
        finalDashSpeed = currentMoveSpeed * dashSpeedMultiplier;

        player.IsDashInvincible = true;
        //if (!player.PlayerLook.IsUIOpen)    player.PlayerLook.enabled = false;

        player.StartDashCooldown();

        player.PlayerAnim.PlayAnimation(EAnimType.DASH);

        player.unitStat.AddModifire(EStatType.Stamina, new StatModifier(-20f, EStatModifireType.Add, this));
        if(player.unitStat.GetStat(EStatType.Stamina) <= 0f)
        {
            player.SetExhausted(2f);
        }

        Vector2 moveInput = Managers.Input.MoveInput;

        if(moveInput != Vector2.zero)
        {
            dashDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;
        }
        else
        {
            Vector3 targetPos = player.PlayerLook.MouseWorldPos;
            Vector3 dir = targetPos - player.transform.position;
            dir.y = 0f;

            dashDirection = dir.normalized;
        }

        if(dashDirection != Vector3.zero)
        {
            player.transform.rotation = Quaternion.LookRotation(dashDirection);
        }
    }

    public override void Update()
    {
        currentDashTime += Time.deltaTime;
            
        player.PlayerMovement.DashMove(dashDirection, finalDashSpeed);

        if(currentDashTime >= dashDuration)
        {
            if(Managers.Input.MoveInput != Vector2.zero)
            {
                stateMachine.ChangeMachine(player.MoveState);
                return;
            }
            else
            {
                stateMachine.ChangeMachine(player.IdleState);
            }

        }
    }

    public override void Exit()
    {
        player.IsDashInvincible = false;
        //if(!player.PlayerLook.IsUIOpen) player.PlayerLook.enabled = true;
    }
}
