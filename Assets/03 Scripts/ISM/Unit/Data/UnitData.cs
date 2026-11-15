using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;




[CreateAssetMenu(fileName = "NewUnitData", menuName = "Stats/UnitData")]
public class UnitData : ScriptableObject
{


    [Header("설정")]
    [Tooltip("플레이어인지 아닌지에 대한 항목입니다.")]
    public UnitType UnitType;
    [Tooltip("해당 유닛의 식별 코드 입니다.")]
    public string unitID;

    [Header("기본 능력치")]
    [Tooltip("기본 최대체력의 설정입니다.")]
    public float maxHp = 100;
    [Tooltip("기본 공격력에 대한 설정입니다")]
    public float atk = 10f;
    [Tooltip("치명타 공격력에 대한 설정입니다. 치명타시 몇 배율로 공격이 들어 갈 것이냐 입니다.")]
    public float criDamage = 0f;
    [Tooltip("기본 방어력에 대한 설정입니다")]
    public float def = 10f;
    [Tooltip("크리티컬 방어력에 대한 설정입니다")]
    public float criDef = 10f;
    [Tooltip("기본 이동속도에 대한 설정입니다")]
    public float spd = 4f;
    [Tooltip("데미지 보정값에 대한 설정입니다. 낮을 수록 데미지가 낮게 나옵니다")]
    public float damageCorrection = 0.8f;
    [Tooltip("받는 데미지 증가량에 대한 설정입니다. 높을수록 받는 데미지가 커집니다.")]
    public float incomingDamage = 1f;
    [Tooltip("공격 속도에 대한 설정 입니다. 초당 발사 횟수입니다.")]
    public float atkSpeed = 0f;
    [Tooltip("공격 범위에 대한 설정입니다.총알이 날아가는 최대 사거리입니다.")]
    public float atkRange = 0f;
    [Tooltip("재장전 속도에 대한 설정입니다. 재장전에 몇초가 걸리는지에 대한 값입니다. ")]
    public float reloadSpeed = 0f;

    [Header("속성 스텟")]
    [Tooltip("화상의 임계점에 대한 설정입니다. 해당 수치에 IgniteGauge가 도달하면 화상을 입습니다.")]
    public float MaxIgniteGauge = 5;
    [Tooltip("빙결의 임계점에 대한 설정입니다. 해당 수치에 FreezeGauge가 도달하면 빙결에 빠집니다.")]
    public float MaxFreezeGauge = 5;
    [Tooltip("쇠약의 임계점에 대한 설정입니다. 해당 수치에 IrradiationGauge가 도달하면 쇠약에 빠집니다.")]
    public float MaxIrradiationGauge = 5;
    [Tooltip("부식의 임계점에 대한 설정입니다. 해당 수치에 CorrosionGauge가 도달하면 부식됩니다.")]
    public float MaxCorrosionGauge = 5;
    [Tooltip("표식의 임계점에 대한 설정입니다. 해당 수치에 MarkedGauge가 도달하면 표식이 찍힙니다.")]
    public float MaxMarkedGauge = 5;
    [Tooltip("부상의 임계점에 대한 설정입니다. 해당 수치에 InjuryGauge가 도달하면 부상을 입습니다.")]
    public float MaxInjuryGauge = 5;


    [Tooltip("공격에 담기는 화상의 누적치에 대한 설정입니다.")]
    public float IgniteAtkCount = 0;
    [Tooltip("공격에 담기는 빙결의 누적치에 대한 설정입니다.")]
    public float FreezeAtkCount = 0;
    [Tooltip("공격에 담기는 쇠약의 누적치에 대한 설정입니다.")]
    public float IrradiationAtkCount = 0;
    [Tooltip("공격에 담기는 부식의 누적치에 대한 설정입니다.")]
    public float CorrosionAtkCount = 0;
    [Tooltip("공격에 담기는 표식의 누적치에 대한 설정입니다.")]
    public float MarkedAtkCount = 0;
    [Tooltip("공격에 담기는 부상의 누적치에 대한 설정입니다.")]
    public float InjuryAtkCount = 0;

    [Tooltip("화상의 면역 유무에 대한 설정입니다.")]
    public bool IgniteImmunity = false;
    [Tooltip("빙결의 면역 유무에 대한 설정입니다.")]
    public bool FreezeImmunity = false;
    [Tooltip("쇠약의 면역 유무에 대한 설정입니다.")]
    public bool IrradiationImmunity = false;
    [Tooltip("부식의 면역 유무에 대한 설정입니다.")]
    public bool CorrosionImmunity = false;
    [Tooltip("쇠약의 면역 유무에 대한 설정입니다.")]
    public bool MarkedImmunity = false;
    [Tooltip("부식의 면역 유무에 대한 설정입니다.")]
    public bool InjuryImmunity = false;


    [Header("시각적 정보")]
    [Tooltip("해당 유닛의 이름입니다")]
    public string unitName;
    [Tooltip("해당 유닛의 외형이 담긴 프리펩 입니다.")]
    public GameObject modelPrefab;//이 안에 애니메이션, 콜라이더 존재
}