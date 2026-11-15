using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Xml;
using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;


public enum EElementType
{
    None = 0,
    Ignite,        // 화상 (최대 체력 비례 도트뎀)
    Freeze,         // 빙결 (이동 불능)
    Irradiation, // 쇠약 (공격력 감소, 데미지 보정값 감소)
    Corrosion,   // 부식 (모든 방어력 감소, 받는 피해량 증가)
    Marked,      // 표식 (치명타 확정)
    Injury       // 부상 (둔화 + 약한 도트뎀)
}

public class ElementStat
{
    private Unit owner;       // 주인 유닛 (Transform이나 위치 정보가 필요할 때 사용)
    private UnitStat unitStat; // 주인 유닛의 스탯 장부
    private CancellationToken cancelToken; // 유닛 파괴 시 비동기 작업을 멈출 토큰

    private Dictionary<EElementType, CancellationTokenSource> decayTokens = new Dictionary<EElementType, CancellationTokenSource>();
    // 디버프 지속시간 타이머 보관함
    private Dictionary<EElementType, CancellationTokenSource> activeDebuffTokens = new Dictionary<EElementType, CancellationTokenSource>();

    private readonly Dictionary<EElementType, List<(EStatType statType, StatModifier modifier)>> debuffModifiers = new();

    // 재적용 대기시간(면역) 상태인 속성 목록
    private HashSet<EElementType> immuneElements = new HashSet<EElementType>();


    private float decayDelay = 3f;

    public ElementStat(Unit ownerUnit)
    {
        this.owner = ownerUnit;
        this.unitStat = ownerUnit.unitStat;
        this.cancelToken = ownerUnit.destroyCancellationToken;
        //Debug.Log("속성 시스템 활성화" + ownerUnit.name);
        debuffModifiers[EElementType.Freeze] = new List<(EStatType, StatModifier)>
        {
            (EStatType.Speed, new StatModifier(-10000f, EStatModifireType.Add, this))
        };

        debuffModifiers[EElementType.Irradiation] = new List<(EStatType, StatModifier)>
        {
            (EStatType.Atk, new StatModifier(-0.5f, EStatModifireType.Percent, this)),
            (EStatType.DamageCorrection, new StatModifier(-0.2f, EStatModifireType.Percent, this))
        };

        debuffModifiers[EElementType.Corrosion] = new List<(EStatType, StatModifier)>
        {
            (EStatType.Def, new StatModifier(-0.5f, EStatModifireType.Percent, this)),
            (EStatType.CritDef, new StatModifier(-0.5f, EStatModifireType.Percent, this)),
            (EStatType.IncomingDamage, new StatModifier(0.2f, EStatModifireType.Percent, this))
        };

        debuffModifiers[EElementType.Injury] = new List<(EStatType, StatModifier)>
        {
            (EStatType.Speed, new StatModifier(-0.5f, EStatModifireType.Percent, this))
        };
    }

    public void ApplyElementGauge(EElementType elementType, float amount)
    {
        Debug.Log("속성 적용 준비됨");
        if (immuneElements.Contains(elementType))
        {
            Debug.Log("면역 상태");
            return;
        }
        if (elementType == EElementType.Freeze && activeDebuffTokens.ContainsKey(elementType))
        {
            Debug.Log("1");
            return;
        }

        var (gaugeType, maxType, decayType) = GetElementStatTypes(elementType);
        ResourceStat gaugeStat = unitStat.GetResourceStat(gaugeType);
        float maxGauge = unitStat.GetStat(maxType);

        if (gaugeStat == null)
        {
            Debug.Log("스탯 없음");

            return;
        }
        gaugeStat.ConsumeStat(amount);

        if (gaugeStat.Value >= maxGauge)
        {
            gaugeStat.ConsumeStat(-gaugeStat.Value);
            TriggerElementEffect(elementType);
            CancelDecayTask(elementType);
            return;
        }

        CancelDecayTask(elementType);

        // Unit으로부터 받아온 파괴 토큰을 부모로 삼아서 새로운 토큰을 생성
        var cts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
        //딕셔너리는 아래와 같이 해서 값을 넣는다 하면 없다면 만들고 있다면 그냥 벨류값을 바꾼다.
        decayTokens[elementType] = cts;

        DecayGaugeTask(elementType, gaugeType, decayType, cts.Token).Forget();
    }

    private async UniTaskVoid DecayGaugeTask(EElementType elementType, EStatType gaugeType, EStatType decayType, CancellationToken token)
    {
        try
        {
            await UniTask.WaitForSeconds(decayDelay, cancellationToken: token);

            ResourceStat gaugeStat = unitStat.GetResourceStat(gaugeType);
            float decayPerSecond = unitStat.GetStat(decayType);

            while (gaugeStat.Value > 0f)
            {
                gaugeStat.ConsumeStat(-decayPerSecond * Time.deltaTime);

                if (gaugeStat.Value <= 0f)
                {
                    Debug.Log("속성치 감소중");
                    gaugeStat.ConsumeStat(-gaugeStat.Value);
                    break;
                }
                await UniTask.Yield(PlayerLoopTiming.Update, token);
            }
        }
        catch (OperationCanceledException) { }
    }

    private void CancelDecayTask(EElementType elementType)
    {
        if (decayTokens.TryGetValue(elementType, out var cts))
        {
            cts?.Cancel();
            cts?.Dispose();
            decayTokens.Remove(elementType);
        }
    }

    private void TriggerElementEffect(EElementType elementType)
    {
        //Transform 위치가 필요하다면 owner.transform.position을 사용 가능!
        Debug.Log($"[{owner.gameObject.name}] 유닛에 {elementType} 상태이상 발동!");


        if (elementType != EElementType.Freeze)
        {
            if (activeDebuffTokens.TryGetValue(elementType, out var existingCts))
            {
                existingCts?.Cancel();
                existingCts?.Dispose();
            }
        }

        // 2. 새로운 디버프 타이머 발급
        var debuffCts = CancellationTokenSource.CreateLinkedTokenSource(cancelToken);
        activeDebuffTokens[elementType] = debuffCts;
        GameObject paticle=null;
        // 3. 속성별 디버프 로직 실행
        switch (elementType)
        {
            case EElementType.Freeze:
                paticle=Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Freeze_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessFreezeDebuff(debuffCts.Token).Forget();
                break;
            case EElementType.Ignite:
                paticle = Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Ignite_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessIgniteDebuff(debuffCts.Token).Forget();
                break;
            case EElementType.Irradiation:
                paticle = Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Irradiation_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessIrradiationDebuff(debuffCts.Token).Forget();
                break;
            case EElementType.Corrosion:
                paticle = Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Corrosion_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessCorrosionDebuff(debuffCts.Token).Forget();
                break;
            case EElementType.Marked:
                paticle = Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Marked_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessMarkedDebuff(debuffCts.Token).Forget();
                break;
            case EElementType.Injury:
                paticle = Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.Injury_PARTICLE_PREFAB,
                new Vector3(owner.transform.position.x, owner.transform.position.y, owner.transform.position.z), Quaternion.identity);
                ProcessInjuryDebuff(debuffCts.Token).Forget();
                break;
                // 다른 속성은 나중에 여기에 추가
        }

        paticle.GetComponent<PoolingParticle>().Init(owner.transform);
    }
    // 빙결: 2초간 멈춤 -> 3초간 면역
    private async UniTaskVoid ProcessFreezeDebuff(CancellationToken token)
    {
        var mods = debuffModifiers[EElementType.Freeze];
        try
        {
            foreach (var mod in mods) unitStat.AddModifire(mod.statType, mod.modifier);
            if (owner != null && owner.gameObject.activeInHierarchy) owner.ApplySpeed();
            await UniTask.WaitForSeconds(2f, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            // 무조건 해제되도록 finally로 이동 & 전용 객체만 제거
            foreach (var mod in mods) unitStat.RemoveModifierFromValue(mod.statType, mod.modifier);
            activeDebuffTokens.Remove(EElementType.Freeze);
            StartImmunityCooldown(EElementType.Freeze, 3f).Forget();
            if (owner != null && owner.gameObject.activeInHierarchy)
            {
                owner.ApplySpeed();
            }
        }
    }

    //  화상: 5초간 1초마다 최대 체력의 1% 도트 데미지 (무한 갱신)
    private async UniTaskVoid ProcessIgniteDebuff(CancellationToken token)
    {
        float duration = 5f;
        float tickInterval = 1f;
        float timeSinceLastTick = 0f;
        float totalElapsed = 0f;

        try
        {
            while (totalElapsed < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                float deltaTime = Time.deltaTime;
                timeSinceLastTick += deltaTime;
                totalElapsed += deltaTime;

                if (timeSinceLastTick >= tickInterval)
                {
                    float maxHp = unitStat.GetStat(EStatType.MaxHp);
                    float dotDmg = maxHp * 0.01f;
                    owner.TakeDotDamage(dotDmg, EElementType.Ignite);

                    timeSinceLastTick = 0f;
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            activeDebuffTokens.Remove(EElementType.Ignite); //finally로 이동하여 안전망 구축
        }
    }

    private async UniTaskVoid ProcessIrradiationDebuff(CancellationToken token)
    {
        var mods = debuffModifiers[EElementType.Irradiation];

        try
        {
            foreach (var mod in mods) unitStat.AddModifire(mod.statType, mod.modifier);
            await UniTask.WaitForSeconds(5f, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            foreach (var mod in mods) unitStat.RemoveModifierFromValue(mod.statType, mod.modifier);
            activeDebuffTokens.Remove(EElementType.Irradiation);
        }
    }
    private async UniTaskVoid ProcessCorrosionDebuff(CancellationToken token)
    {
        var mods = debuffModifiers[EElementType.Corrosion];

        try
        {
            foreach (var mod in mods) unitStat.AddModifire(mod.statType, mod.modifier);
            await UniTask.WaitForSeconds(5f, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            foreach (var mod in mods) unitStat.RemoveModifierFromValue(mod.statType, mod.modifier);
            activeDebuffTokens.Remove(EElementType.Corrosion);
        }
    }
    private async UniTaskVoid ProcessInjuryDebuff(CancellationToken token)
    {
        float duration = 5f;
        float tickInterval = 1f;
        float timeSinceLastTick = 0f;
        float totalElapsed = 0f;

        var mods = debuffModifiers[EElementType.Injury];

        try
        {
            // 1. 디버프 스탯 적용 (이속 감소)
            foreach (var mod in mods) unitStat.AddModifire(mod.statType, mod.modifier);
            if (owner != null && owner.gameObject.activeInHierarchy)
            {
                owner.ApplySpeed();
            }
            // 2. 도트 데미지 루프 시작
            while (totalElapsed < duration)
            {
                await UniTask.Yield(PlayerLoopTiming.Update, token);

                float deltaTime = Time.deltaTime;
                timeSinceLastTick += deltaTime;
                totalElapsed += deltaTime;

                if (timeSinceLastTick >= tickInterval)
                {
                    float maxHp = unitStat.GetStat(EStatType.MaxHp);
                    float dotDmg = maxHp * 0.005f;
                    owner.TakeDotDamage(dotDmg, EElementType.Injury);

                    timeSinceLastTick = 0f;
                }
            }
        }
        catch (OperationCanceledException) { }
        finally
        {
            // 💡 3. 도중에 죽든 취소되든 완벽하게 해제
            foreach (var mod in mods) unitStat.RemoveModifierFromValue(mod.statType, mod.modifier);
            activeDebuffTokens.Remove(EElementType.Injury);
            if (owner != null && owner.gameObject.activeInHierarchy)
            {
                owner.ApplySpeed();
            }
        }
    }
    private async UniTaskVoid ProcessMarkedDebuff(CancellationToken token)
    {
        try
        {
            owner.SetMarked(true);
            await UniTask.WaitForSeconds(5f, cancellationToken: token);
        }
        catch (OperationCanceledException) { }
        finally
        {
            // 💡 표식 해제 처리 추가 & 오타 수정 (Freeze -> Marked)
            owner.SetMarked(false);
            activeDebuffTokens.Remove(EElementType.Marked);
        }
    }
    //면역 대기시간 관리
    private async UniTaskVoid StartImmunityCooldown(EElementType elementType, float duration)
    {
        try
        {
            // 1. 면역 부여
            immuneElements.Add(elementType);

            // 2. 대기 (도중에 토큰이 취소되면 여기서 Exception 발생)
            await UniTask.WaitForSeconds(duration, cancellationToken: cancelToken);
        }
        catch (OperationCanceledException) { }
        finally
        {
            // 3. 도중에 죽든, 풀에 들어가든, 시간이 다 되든 무조건 면역 리스트에서 제거!
            immuneElements.Remove(elementType);
        }
    }
    private (EStatType gauge, EStatType max, EStatType decay) GetElementStatTypes(EElementType type)
    {

        return type switch
        {
            EElementType.Ignite => (EStatType.IgniteGauge, EStatType.MaxIgniteGauge, EStatType.IgniteDecayRate),
            EElementType.Freeze => (EStatType.FreezeGauge, EStatType.MaxFreezeGauge, EStatType.FreezeDecayRate),
            EElementType.Corrosion => (EStatType.CorrosionGauge, EStatType.MaxCorrosionGauge, EStatType.CorrosionDecayRate),
            EElementType.Irradiation => (EStatType.IrradiationGauge, EStatType.MaxIrradiationGauge, EStatType.IrradiationDecayRate),
            EElementType.Injury => (EStatType.InjuryGauge, EStatType.MaxInjuryGauge, EStatType.InjuryDecayRate),
            EElementType.Marked => (EStatType.MarkedGauge, EStatType.MaxMarkedGauge, EStatType.MarkedDecayRate),
            _ => throw new Exception($"매핑되지 않은 속성: {type}")
        };
    }
}
