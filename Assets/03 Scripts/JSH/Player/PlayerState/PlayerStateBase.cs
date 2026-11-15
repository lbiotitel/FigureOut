using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase 
{
    protected PlayerUnit player;
    protected PlayerStateMachine stateMachine;

    public PlayerStateBase(PlayerUnit player, PlayerStateMachine stateMachine)
    {
        this.player = player;
        this.stateMachine = stateMachine;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    protected void RecoverStamina()
    {
        if (!player.IsExhausted)
        {
            // 현재 스태미나와 최대 스태미나를 가져와서 비교합니다!
            float currentStamina = player.unitStat.GetStat(EStatType.Stamina);
            float maxStamina = player.unitStat.GetStat(EStatType.MaxStamina);
            float regenRate = player.unitStat.GetStat(EStatType.StaminaRegenRate);

            // 현재 스태미나가 최대치보다 작을 때만 회복시킵니다.
            if (currentStamina < maxStamina)
            {
                player.unitStat.AddModifire(EStatType.Stamina, new StatModifier(regenRate * Time.deltaTime, EStatModifireType.Add, this));
            }
        }
    }
}
