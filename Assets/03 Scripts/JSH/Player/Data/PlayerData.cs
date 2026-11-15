using UnityEngine;

[CreateAssetMenu(fileName = "NewUnitData", menuName = "Stats/PlayerData")]
public class PlayerData : UnitData
{

    [Header("─── 플레이어 전용 능력치 ───────────────────────────")]

    [Tooltip("운: 높을수록 좋은 아이템 등급 확률 증가")]
    public float luck = 0f;

    [Header("─── 리소스 초기 최대치 ────────────────────────────")]

    [Tooltip("스테미너 최대치 (달리기/구르기 소모)")]
    public float maxStamina = 100f;

    [Tooltip("배고픔 최대치 ")]
    public float maxHungry = 100f;

    [Tooltip("목마름 최대치 ")]
    public float maxThirsty = 100f;

    [Header("─── 리소스 소모 속도 ────────────────────────────")]

    [Tooltip("스테미너 초당 회복 속도")]
    public float staminaRegenRate = 25f;

    [Tooltip("배고픔 감소 속도")]
    public float hungryDecayRate = 0.5f;

    [Tooltip("목마름 감소 속도")]
    public float thirstyDecayRate = 0.8f;

    [Header("─── 페널티 ────────────────────────────────────────")]

    [Tooltip("배고픔/목마름이 0일 때 초당 HP 감소량")]
    public float survivalPenaltyDamage = 1f;

    public float survivalPenaltyDuration = 1.5f;
}
