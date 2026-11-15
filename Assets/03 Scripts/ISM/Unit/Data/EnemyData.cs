using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EEnemyType
{
    None,
    samurai
}
[CreateAssetMenu(fileName = "NewUnitData", menuName = "Stats/EnemyData")]
public class EnemyData : UnitData
{
    [Header("에너미 BT 타입")]
    [Tooltip("BT: 행동 패턴이 다음과 같습니다.")]
    public EnemyAIStrategy aiStrategy;


    [Header("-----에너미 전용 수치-------------------------")]
    [Tooltip("시야거리: 이만큼 멀리까지 봅니다.")]
    public float viewDistance = 20f; // 시야 거리
    [Tooltip("시야각: 자신을 중심으로 이만큼 각도를 봅니다.")]
    public float viewAngle = 90f;    // 시야각
    [Header("Utility AI Weights (가중치 계수)")]
    [Tooltip("공격성: 높을수록 사격 점수가 가중됩니다.")]
    public float aggressionWeight = 1.0f;
    [Tooltip("방어성: 높을수록 엄폐 및 도망 점수가 가중됩니다.")]
    public float defenseWeight = 1.0f;
    [Tooltip("사거리 선호도: 이 거리 근처에서 사격 점수가 가장 높습니다.")]
    public float optimalRange = 15f;
    [Tooltip("순찰 범위: 스폰한 위치에서 돌아다니는 범위 입니다. 전투 모드시 더 좁은 범위를 순찰합니다")]
    public float patrolRMax = 10f;
    [Tooltip("몬스터가 보유할 무기 번호입니다.")]
    public string WeaponID;
    [Tooltip("몬스터의 종류 지정입니다.플레이어 공유에 사용됩니다")]
    public EEnemyType EnemyType=EEnemyType.None;

    [Header("─── 리소스 최대치 ────────────────────────────")]
    [Tooltip("어그로 수치 최대치에 대한 설정 입니다. 해당 리소스 스텟은 시작시 0 입니다.")]
    public float maxAggro = 100f;

    [Header("─── 리소스 소모 속도 ────────────────────────────")]
    [Tooltip("초당 어드로 수치 감소량입니다.")]
    public float aggroDecayRate = 5f;

    public string UnitDropTableID;
}