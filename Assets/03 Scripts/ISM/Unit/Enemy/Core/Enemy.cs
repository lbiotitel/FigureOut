using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public enum EEnemyBodyState
{
    Idle,
    Moving,
    Action,
    Scanning
}

public class Enemy : Unit
{
    [Header("Enemy Data")]
    public EnemyData enemyData;
    public EnemySpawner mySpawner;

    [Header("AI State & Blackboard")]
    public Transform Target;
    public Transform ghostTarget;
    private Tween ghostTween;
    public Vector3 patrolPoint;
    public float patrolRMax;//max Radius when patrolling
    public float currentPatrolR;//Radius when patrolling
    //private bool isPatrolling = false;
    public EEnemyBodyState BodyState { get; private set; } = EEnemyBodyState.Idle;
    public bool isScan = false;
    [SerializeField] private LayerMask enemyLayerMask = 1 << 10;
    private Vector3 lastDestination = Vector3.positiveInfinity;
    private const float DestinationThreshold = 0.3f;


    [Header("Combat & Equipment")]
    public Transform shootPoint;
    public Quaternion _stopRotation;
    public StatModifier patrolSpeedModifier;


    [Header("Layer Masks")]
    private LayerMask targetMask = 1 << 8;
    private LayerMask obstructionMask = 1 << 9;

    [Header("Components")]
    public NavMeshAgent agent;
    private Node rootNode;


    private Renderer[] cachedRenderers;
    private Dropper dropper;

    [Header("UI & Minimap")]
    private SpriteRenderer minimapMaker;

    private bool IsTargetSynchronized
    {
        get
        {
            // 타겟이 없으면 쏠 수 없음
            if (Target == null || ghostTarget == null) return false;

            Vector3 dirToGhost = (ghostTarget.position - transform.position).normalized;
            Vector3 dirToRealPlayer = (Target.position - transform.position).normalized;

            dirToGhost.y = 0f;
            dirToRealPlayer.y = 0f;

            float syncAngle = Vector3.Angle(dirToGhost, dirToRealPlayer);

            // 고스트 타겟과 실제 타겟의 각도 차이가 15도 이내면 조준 완료(true)
            return syncAngle <= 30f;
        }
    }


    public void SetSpawner(EnemySpawner spawner)
    {
        mySpawner = spawner;
    }

    public override void Init(UnitData data)
    {
        base.Init(data);
        agent = GetComponent<NavMeshAgent>();

        enemyData = data as EnemyData;
        if (enemyData == null || EquipmentController == null || agent == null) return;

        Stat maxAgro = new Stat(enemyData.maxAggro);
        unitStat.AddStat(EStatType.MaxAggro, maxAgro);
        unitStat.AddStat(EStatType.Aggro, new ResourceStat(maxAgro));
        unitStat.AddModifire(EStatType.Aggro, new StatModifier(-1000f, EStatModifireType.Add, this));

        patrolSpeedModifier = new StatModifier(-0.5f, EStatModifireType.Percent, this);
        unitStat.AddModifire(EStatType.Speed, patrolSpeedModifier);
        SetPatrollSpeed(false);
        agent.stoppingDistance = 0.1f;
        patrolRMax = enemyData.patrolRMax;
        patrolPoint = transform.position;

        ghostTarget = new GameObject("NewEmptyObject").transform;
        ghostTarget.position = transform.position;
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            //Debug.Log("Player Found");
            Target = playerObj.transform;
        }

        agent.acceleration = 1000f;

        dropper = new Dropper();
        dropper.Init(enemyData.UnitDropTableID);

        EquipmentController.Equip(Managers.Data.GetData<WeaponData>(enemyData.WeaponID));

        FindViewRenderer();
        CheckAggroState();
    }

    private void FindViewRenderer()
    {
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();
        List<Renderer> vaildRenderers = new List<Renderer>();

        for (int i = 0; i < allRenderers.Length; i++)
        {
            Renderer r = allRenderers[i];

            if (r.CompareTag("MinimapMarker") && r is SpriteRenderer)
            {
                minimapMaker = (SpriteRenderer)r;
                continue;
            }
            if (r is MeshRenderer || r is SkinnedMeshRenderer)
            {
                vaildRenderers.Add(r);
            }
        }

        cachedRenderers = vaildRenderers.ToArray();
        SetVisible(false);
    }


    public void CheckAggroState()
    {
        bool hasAggro = unitStat.GetStat(EStatType.Aggro) > 0;

        if (hasAggro)
        {
            SetPatrollSpeed(false);
            currentPatrolR = patrolRMax * 0.5f;
            patrolPoint = ghostTarget.position;

        }
        else if (!hasAggro)
        {
            SetPatrollSpeed(true);
            currentPatrolR = patrolRMax;
            patrolPoint = mySpawner.transform.position;
        }
    }

    private void Update()
    {
        if (IsDead) return;

        if (isScan)
        {
            Scanning();
        }

        if (unitStat.GetStat(EStatType.Aggro) > 0)
        {
            patrolPoint = ghostTarget.position;
            float decayAmount = -1 * enemyData.aggroDecayRate * Time.deltaTime;
            ModifyAggro(decayAmount); // 합쳐둔 함수 사용
        }
    }
    public void StartScan()
    {
        if (IsDead) return;

        if (BodyState != EEnemyBodyState.Scanning)
        {
            Stop();
            SetBodyState(EEnemyBodyState.Scanning);

            _stopRotation = transform.rotation;
            isScan = true;
        }
    }
    public void Scanning()
    {
        if (BodyState == EEnemyBodyState.Scanning)
        {
            float angle = Mathf.Sin(Time.time * 3) * 90;
            transform.rotation = _stopRotation * Quaternion.Euler(0, angle, 0);
        }
    }

    public void StopScan()
    {
        isScan = false;
        _stopRotation = Quaternion.identity;
        if (BodyState == EEnemyBodyState.Scanning)
        {
            SetBodyState(EEnemyBodyState.Idle);
        }
    }

    public void SetBodyState(EEnemyBodyState newState)
    {
        BodyState = newState;

        if (newState == EEnemyBodyState.Moving)
            agent.updateRotation = true;
        else
            agent.updateRotation = false;
    }

    public void SetPatrollSpeed(bool isPatrolling)
    {
        if (isPatrolling)
        {
            unitStat.AddModifire(EStatType.Speed, patrolSpeedModifier);
        }
        else
        {
            unitStat.RemoveModifierFromValue(EStatType.Speed, patrolSpeedModifier);
        }
        // 최종 속도를 내비게이션에 갱신
        SetMoveSpeed(unitStat.GetStat(EStatType.Speed));
    }

    public override void ApplySpeed()
    {
        agent.speed = unitStat.GetStat(EStatType.Speed);
    }
    public void SetMoveSpeed(float speed)
    {
        agent.speed = speed;

    }

    public void SetVisible(bool isVisible)
    {
        if (cachedRenderers == null) return;

        for (int i = 0; i < cachedRenderers.Length; i++)
        {
            if (cachedRenderers[i] != null)
            {
                cachedRenderers[i].enabled = isVisible;
            }
        }

        if (worldHpUI != null)
        {
            worldHpUI.gameObject.SetActive(isVisible);
        }

        if (minimapMaker != null)
        {
            minimapMaker.enabled = isVisible;
        }
    }

    public void MoveTo(Vector3 destination)
    {
        //Debug.Log("이동 실행");
        if (agent == null) return;
        if (Vector3.Distance(lastDestination, destination) < DestinationThreshold)
        {
            return;
        }
        if (BodyState != EEnemyBodyState.Moving)
        {
            StopScan();   // 스캔 중이었다면 멈춤
            SetBodyState(EEnemyBodyState.Moving);
        }
        if (agent.isStopped)
        {
            Debug.Log(" 무브투 함수 이동에이전트 멈춤 상태였음");
            agent.isStopped = false;
        }
        agent.SetDestination(destination);
        lastDestination = destination;
    }

    public void Stop()
    {
        if (agent == null) return;

        agent.isStopped = true;
        agent.ResetPath();
    }

    public void Attack()
    {
        if (IsDead) return;
        if (BodyState != EEnemyBodyState.Action)
        {
            Stop();       // 1. 이동 중이었다면 급브레이크 (agent.isStopped = true)
            StopScan();   // 2. 스캔 중이었다면 스캔 회전 중지
            SetBodyState(EEnemyBodyState.Action); // 3. 내 몸은 이제 '공격 상태'
        }

        FaceTarget();
        EquipmentController.UseWeapon(ghostTarget);
    }

    public void FaceTarget()
    {
        if (IsDead) return;
        if (IsTargetSynchronized)
        {
            ModifyAggro(unitStat.GetStat(EStatType.MaxAggro));
            //플레이어를 바라볼 경우 어그로 수치 최대
            Vector3 targetDirection = (ghostTarget.position - transform.position).normalized;
            targetDirection.y = 0f;

            if (targetDirection != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(targetDirection);
            }
        }
    }
    public override void TakeDamage(DmgContext context, bool crit = false)
    {
        base.TakeDamage(context, crit);

        UpdateGhostPosition(Target);

        ModifyAggro(unitStat.GetStat(EStatType.MaxAggro));
    }
    public void ModifyAggro(float amount)
    {
        unitStat.ConsumeStat(EStatType.Aggro, amount);

        if (amount > 0)//어그로 수치가 증가하는 경우
        {
            UpdateGhostPosition(Target); // 어그로가 오르면 고스트 타겟도 갱신
            CheckAggroState();//어그로가 오를 경우 속도 값 조정 들어감

        }
        else//감소하는 경우
        {
            if (unitStat.GetStat(EStatType.Aggro) <= 0)
            {
                CheckAggroState();//어그로 수치가 0보다 작거나 같아지면 속도 값 보정 실시

            }
        }


    }

    public void UpdateGhostPosition(Transform targetTransform)
    {
        if (targetTransform == null) return;

        if (ghostTween != null && ghostTween.IsActive()) ghostTween.Kill();

        ghostTween = ghostTarget.DOMove(targetTransform.position, 1.0f).SetEase(Ease.OutCubic);
    }

    public void SharePlayerPosition(EEnemyType targetType, float shareRange)
    {
        Collider[] nearbyColliders = Physics.OverlapSphere(transform.position, shareRange, enemyLayerMask);

        foreach (var col in nearbyColliders)
        {
            // 2. 에너미 컴포넌트 추출
            if (col.TryGetComponent<Enemy>(out Enemy ally))
            {
                // 자기 자신이나 이미 죽은 동료는 제외
                if (col.gameObject == this.gameObject || ally.IsDead) continue;

                // 3. 사용자가 지정한 에너미 종류와 일치하는지 필터링
                if (targetType == EEnemyType.None || ally.enemyData.EnemyType == targetType)
                {
                    // 4. 동료에게 플레이어 위치를 주입. 주입받은 동료는 당시 플레이어 위치로 이동하게됨
                    ally.OnReceiveTargetAlert(Target);
                }
            }
        }

    }


    public void OnReceiveTargetAlert(Transform sharedTarget)
    {
        if (IsDead || sharedTarget == null) return;

        if (BodyState == EEnemyBodyState.Action)
            return;


        if (ghostTarget != null)
        {
            UpdateGhostPosition(sharedTarget);
        }

        float alertAggro = unitStat.GetStat(EStatType.MaxAggro); // 최대치의 50% 정도 충전
        ModifyAggro(alertAggro);
        CheckAggroState();
        MoveTo(sharedTarget.position);


    }
    protected override void Die(string killerName)
    {
        Debug.Log($"{enemyData.name} 사망 처리 중...");

        Managers.Object.CreatePaticleEffect(KeyDefines.ParticleKey.DEAD_PARTICLE_PREFAB,
            new Vector3(transform.position.x, 2f, transform.position.z), Quaternion.identity);

        Managers.Audio.PlaySfx(KeyDefines.SoundKey.DeadSfx);

        if (mySpawner != null)
        {
            mySpawner.ReportDeath();
        }

        IsDead = true;
        dropper.Drop(transform.position);
        if (ghostTween != null && ghostTween.IsActive()) ghostTween.Kill();
        gameObject.SetActive(false);
        Managers.Event.NotifyEvent<EnemyDiedEvent>(new EnemyDiedEvent { enemy = this });
    }

    public void ResetEnemyState(Vector3 spawnPosition)
    {
        // 1. 위치 리셋 
        if (agent != null && agent.isActiveAndEnabled)
        {
            agent.Warp(spawnPosition);
            agent.ResetPath();
        }
        else
        {
            transform.position = spawnPosition;
        }

        // 2. 스탯 리셋
        float maxHp = unitStat.GetStat(EStatType.MaxHp);
        unitStat.ConsumeStat(EStatType.Hp, maxHp);
        unitStat.ConsumeStat(EStatType.Aggro, -1000);
        UpdateGhostPosition(mySpawner.transform);
        patrolPoint = mySpawner.transform.position;

        // 3. 몸 상태 리셋
        SetBodyState(EEnemyBodyState.Idle);
    }
    private async void DisableEnemy(float t, Action action)
    {
        await UniTask.WaitForSeconds(t);
        action?.Invoke();
    }

    public bool IsCoverNearby(float radius, out Vector3 coverPos)
    {
        coverPos = Vector3.zero;
        if (Target == null) return false;

        Collider[] colliders = Physics.OverlapSphere(transform.position, radius, LayerMask.GetMask("Wall"));

        float bestDistance = float.MaxValue;
        bool found = false;

        foreach (var col in colliders)
        {
            Vector3 dirFromPlayer = (col.transform.position - Target.position).normalized;
            Vector3 candidatePos = col.transform.position + dirFromPlayer * 1.5f;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(candidatePos, out hit, 2.0f, NavMesh.AllAreas))
            {
                Vector3 testDir = (Target.position - hit.position).normalized;
                float distToPlayer = Vector3.Distance(hit.position, Target.position);

                if (Physics.Raycast(hit.position + Vector3.up, testDir, out RaycastHit rayHit, distToPlayer))
                {
                    if (rayHit.collider.gameObject.layer == LayerMask.NameToLayer("Cover"))
                    {
                        float distToCover = Vector3.Distance(transform.position, hit.position);
                        if (distToCover < bestDistance)
                        {
                            bestDistance = distToCover;
                            coverPos = hit.position;
                            found = true;
                        }
                    }
                }
            }
        }
        return found;
    }

    public bool CanSeePlayer
    {
        get
        {
            if (Target == null) return false;

            float dist = Vector3.Distance(transform.position, Target.position);
            if (dist > enemyData.viewDistance) return false;

            Vector3 dirToTarget = (Target.position - transform.position).normalized;
            if (Vector3.Angle(transform.forward, dirToTarget) < enemyData.viewAngle / 2)
            {
                if (!Physics.Raycast(transform.position, dirToTarget, dist, obstructionMask))
                {

                    return true;
                }
            }

            return false;
        }
    }



    //Select a random point from among points 10 units away from currentPatrolR
    public Vector3 GetNavigableRandomPoint(Vector3 center, float radius)
    {


        Vector3 randomDir = UnityEngine.Random.insideUnitSphere * radius;//반지름 currentPatrolR인 원 안의 랜덤한 위치를 반환
        randomDir.y = 0f;
        randomDir += center;//그걸 에너미의 현 추적 위치에 더함

        NavMeshHit hit;//정보 담을 일회용 바구니
        if (NavMesh.SamplePosition(randomDir, out hit, 10f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return transform.position;
    }

    public float GetDistanceFromPlayer()
    {
        if (Target == null) return float.MaxValue; // 플레이어가 없으면 아주 먼 거리 반환

        return Vector3.Distance(transform.position, Target.position);
    }
    public void MoveAwayFromPlayer(float fleeDistance)
    {
        if (Target == null) return;

        Vector3 fleeDirection = (transform.position - Target.position).normalized;

        fleeDirection.y = 0f;

        Vector3 idealDestination = transform.position + (fleeDirection * fleeDistance);

        Vector3 safeDestination = GetNavigableRandomPoint(idealDestination, 5f);
        SetBodyState(EEnemyBodyState.Moving);
        MoveTo(safeDestination);
    }


#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        if (enemyData == null) return;

        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, enemyData.viewDistance);

        Vector3 leftDir = Quaternion.Euler(0, -enemyData.viewAngle / 2f, 0) * transform.forward;
        Vector3 rightDir = Quaternion.Euler(0, enemyData.viewAngle / 2f, 0) * transform.forward;

        Gizmos.color = CanSeePlayer ? Color.red : Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + leftDir * enemyData.viewDistance);
        Gizmos.DrawLine(transform.position, transform.position + rightDir * enemyData.viewDistance);

        if (Target != null)
        {
            Gizmos.color = CanSeePlayer ? Color.red : Color.gray;
            Gizmos.DrawLine(transform.position + Vector3.up, Target.position + Vector3.up);
        }

        if (ghostTarget != null && ghostTarget.position != Vector3.zero)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(ghostTarget.position, 0.3f);
        }
    }
#endif
}
