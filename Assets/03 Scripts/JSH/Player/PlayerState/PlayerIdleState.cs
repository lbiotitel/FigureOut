using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerStateBase
{
    public PlayerIdleState(PlayerUnit player, PlayerStateMachine playerState) : base(player, playerState) { }
    public override void Enter()
    {
        player.PlayerAnim.PlayAnimation(EAnimType.IDLE);
    }

    public override void Update()
    {
        if(Managers.Input.MoveInput != Vector2.zero)
        {
            stateMachine.ChangeMachine(player.MoveState);
            return;
        }

        if(Managers.Input.RollButton && player.unitStat.GetStat(EStatType.Stamina) > 0f && !player.IsExhausted && player.CanDash)
        {
            stateMachine.ChangeMachine(player.DashState);
            return;
        }

        player.PlayerAnim.PlayAnimation(EAnimType.IDLE);

        RecoverStamina();
    }

    public override void Exit()
    {

    }


}
