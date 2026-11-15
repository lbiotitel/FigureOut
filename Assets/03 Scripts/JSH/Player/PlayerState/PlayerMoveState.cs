using UnityEngine;

public class PlayerMoveState : PlayerStateBase
{
    private float stepTime = 0;
    private float stepSfxTime = 0.3f;
    public PlayerMoveState(PlayerUnit player, PlayerStateMachine stateMachine) : base(player, stateMachine) { }

    public override void Enter()
    {
        stepTime = 0f;
        player.PlayerAnim.PlayAnimation(EAnimType.WALK);
    }

    public override void Update()
    {
        if (player.IsDead) return;

        Vector2 input = Managers.Input.MoveInput;

        if (input == Vector2.zero)
        {
            stateMachine.ChangeMachine(player.IdleState);
            return;
        }

        if (Managers.Input.RollButton && player.unitStat.GetStat(EStatType.Stamina) > 0f && !player.IsExhausted && player.CanDash)
        {
            stateMachine.ChangeMachine(player.DashState);
            return;
        }

        bool hasStamina = player.unitStat.GetStat(EStatType.Stamina) > 0f;
        bool isSprinting = Managers.Input.SprintButton && hasStamina && !player.IsExhausted;

        player.IsSprinting = isSprinting;

        player.PlayerMovement.Move(input, isSprinting);
        stepTime += Time.deltaTime;
        PlayStepSfx();

        player.unitStat.AddModifire(EStatType.Hungry, new StatModifier(-player.unitStat.GetStat(EStatType.HungryDecayRate) * Time.deltaTime, EStatModifireType.Add, this));
        player.unitStat.AddModifire(EStatType.Thirsty, new StatModifier(-player.unitStat.GetStat(EStatType.ThirstyDecayRate) * Time.deltaTime, EStatModifireType.Add, this));

        if (isSprinting)
        {
            player.unitStat.AddModifire(EStatType.Stamina, new StatModifier(-20f * Time.deltaTime, EStatModifireType.Add, this));
            player.PlayerAnim.PlayAnimation(EAnimType.SPRINT);

            if (player.unitStat.GetStat(EStatType.Stamina) <= 0f)
            {
                player.SetExhausted(2f);
            }
        }
        else
        {
            player.PlayerAnim.PlayAnimation(EAnimType.WALK);

            RecoverStamina();
        }
    }


    private void PlayStepSfx()
    {
        if (stepTime >= stepSfxTime)
        {
            stepTime = 0f;
            int randomSfx = Random.Range(0, 2);

            if (randomSfx == 0) Managers.Audio.PlaySfx(KeyDefines.SoundKey.StepSfx1);
            else Managers.Audio.PlaySfx(KeyDefines.SoundKey.StepSfx2);
        }
    }

    public override void Exit()
    {
        player.IsSprinting = false;
    }


}
