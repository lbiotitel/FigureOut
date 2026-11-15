using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.Timeline;

public class PlayerUnit : Unit, IVisionProvider
{
    [SerializeField] private PlayerData playerData;
    [field:SerializeField] public InventoryHolder InventoryHolder { get; private set; }
    public PickUpController PickUpController { get; private set; }
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    public PlayerDashState DashState { get; private set; }
    public VisionTest VisionTest { get; private set; }
    public PlayerMovement PlayerMovement { get; private set; }
    public PlayerLook PlayerLook { get; private set; }
    public PlayerUnitAnimation PlayerAnim { get; private set; }
    public PlayerVisionBroadcaster PlayerVisionBroadcaster {  get; private set; }
    public MouseVisionBroadcaster MouseVisionBroadcaster { get; private set; }

    public Transform VisionTransform => transform;
    public float ViewRadius
    {
        get
        {
            if (PlayerStat == null || PlayerStat.GetStatBase(EStatType.AtkRange) == null)
            {
                return visionRadius;
            }

            float atkRange = PlayerStat.GetStat(EStatType.AtkRange);

            float extraRange = Mathf.Max(0f, atkRange - 5f);
            return Mathf.Clamp(visionRadius + extraRange, visionRadius, maxVisionRadius);
        }  
    }

    public float ViewAngle => visionAngle;
    public float CircleRadius => circleRadius;

    public LayerMask TargetMask => targetMask;
    public LayerMask VisionObstacleMask => visionObstacleMask;
    public LayerMask AttackObstacleMask => attackObstacleMask;
    public bool IsVisionActive => true;

    public Transform currentTarget;
    public UnitStat PlayerStat => unitStat;

    private Vector3 spawnPoint;

    [SerializeField] private float visionAngle = 60f;
    [SerializeField] private float visionRadius = 20f;
    private const float maxVisionRadius = 40f;
    [SerializeField] private float circleRadius = 3f;
    [SerializeField] private float mouseCircleRadius = 3f;
    [SerializeField] private LayerMask targetMask;
    [SerializeField] private LayerMask visionObstacleMask;
    [SerializeField] private LayerMask attackObstacleMask;

    private PlayerWorldUI playerWorldUI;

    private float survivalPenaltyTimer = 0f;
    private float recoveryStaminaDelayTimer = 0f;

    [SerializeField] private float dashCooldownDuration = 0.8f;
    private float dashCooldownTimer = 0f;

    public bool CanDash => dashCooldownTimer <= 0f && !IsExhausted;
    public bool IsExhausted => recoveryStaminaDelayTimer > 0f;
    public bool IsInvincible { get; set; } = false;
    public bool IsDashInvincible { get; set; } = false;
    public bool IsSprinting { get; set; } = false;

    public event Action<float, bool, bool> OnPlayerhit;
    public static event Action OnPlayerDeath;

    public override void Init(UnitData unitData)
    {
        playerData = unitData as PlayerData;

        if (playerData == null)
        {
            Debug.LogError("PlayerData is null or not of type PlayerData.");
            return;
        }

        base.Init(unitData);

        InitStat();
        InitComponent();
        InitStateMachine();

        BindWorldUI();

        if (FogOfWarManager.Instance != null)
        {
            FogOfWarManager.Instance.RegisterVisionProvider(this);
        }
    }

    private void InitStateMachine()
    {
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine);
        MoveState = new PlayerMoveState(this, StateMachine);
        DashState = new PlayerDashState(this, StateMachine);

        StateMachine.Init(IdleState);
    }

    private void InitComponent()
    {
        PickUpController = GetComponent<PickUpController>();
        PlayerAnim = GetComponent<PlayerUnitAnimation>();
        VisionTest = GetComponent<VisionTest>();
        PlayerMovement = GetComponent<PlayerMovement>();
        PlayerLook = GetComponent<PlayerLook>();
        PlayerVisual visual = GetComponent<PlayerVisual>();
        PlayerVisionBroadcaster = GetComponent<PlayerVisionBroadcaster>();
        MouseVisionBroadcaster = GetComponent<MouseVisionBroadcaster>();

        PickUpController.OnItemPickUp += (itemData, amount) =>
        {
            InventoryHolder.InventorySys.AddToInventory(itemData, amount);
        };

        VisionTest.Init(PlayerStat.GetStatBase(EStatType.AtkRange), this);
        PlayerMovement.Init(this, PlayerStat.GetStatBase(EStatType.Speed));
        visual.Init(CharaterSelectData.selectedColor, CharaterSelectData.selectedFaceMat);

        CameraController cameraController = FindAnyObjectByType<CameraController>();
        PlayerLook.Init(cameraController.Cam);
        PlayerVisionBroadcaster.Init("VisionTargetPos", "_HoleRadius", this.circleRadius);
        MouseVisionBroadcaster.MouseInit(mouseCircleRadius, this.PlayerLook);
    }

    private void InitStat()
    {
        Stat maxStamina = new Stat(playerData.maxStamina);
        Stat maxHungry = new Stat(playerData.maxHungry);
        Stat maxThirsty = new Stat(playerData.maxThirsty);

        PlayerStat.AddStat(EStatType.Luck, new Stat(playerData.luck));
        PlayerStat.AddStat(EStatType.MaxStamina, maxStamina);
        PlayerStat.AddStat(EStatType.Stamina, new ResourceStat(maxStamina));
        PlayerStat.AddStat(EStatType.MaxHungry, maxHungry);
        PlayerStat.AddStat(EStatType.Hungry, new ResourceStat(maxHungry));
        PlayerStat.AddStat(EStatType.MaxThirsty, maxThirsty);
        PlayerStat.AddStat(EStatType.Thirsty, new ResourceStat(maxThirsty));
        PlayerStat.AddStat(EStatType.ThirstyDecayRate, new Stat(playerData.thirstyDecayRate));
        PlayerStat.AddStat(EStatType.HungryDecayRate, new Stat(playerData.hungryDecayRate));
        PlayerStat.AddStat(EStatType.StaminaRegenRate, new Stat(playerData.staminaRegenRate));
        PlayerStat.AddStat(EStatType.playerServivalPenaltyDamage, new Stat(playerData.survivalPenaltyDamage));
        PlayerStat.AddStat(EStatType.playerServivalPenaltyDuration, new Stat(playerData.survivalPenaltyDuration));
    }

    private void BindWorldUI()
    {
        playerWorldUI = GetComponentInChildren<PlayerWorldUI>();
            
        if (playerWorldUI != null)
        {
            playerWorldUI.Init(this.transform);

            ResourceStat staminaStat = PlayerStat.GetStatBase(EStatType.Stamina) as ResourceStat;

            if (staminaStat != null)
            {
                playerWorldUI.UpdateStamina(staminaStat.Value, PlayerStat.GetStat(EStatType.MaxStamina));

                staminaStat.OnValueChanged += (prevStamina,currentStamina) =>
                {
                    float currentMaxStamina = PlayerStat.GetStat(EStatType.MaxStamina);
                    playerWorldUI.UpdateStamina(currentStamina, currentMaxStamina);
                };
            }
        }
    }

    private void OnDestroy()
    {
        if (FogOfWarManager.Instance != null)
            FogOfWarManager.Instance.UnregisterVisionProvider(this);
    }

    public void StartDashCooldown()
    {
        dashCooldownTimer = dashCooldownDuration;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) transform.position = spawnPoint;
        if (IsDead) return;

        if (recoveryStaminaDelayTimer > 0f)
        {
            recoveryStaminaDelayTimer -= Time.deltaTime;
        }

        if(dashCooldownTimer > 0f)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (StateMachine != null || StateMachine.CurrentState != null)
        {
            StateMachine.CurrentState.Update();
        }

        if (VisionTest != null)
        {
            currentTarget = VisionTest.Detect();

            if (currentTarget != null && !IsSprinting && StateMachine.CurrentState != DashState && !PlayerLook.IsUIOpen)
            {
                //Debug.DrawLine(
                //    transform.position,
                //    currentTarget.position,
                //    UnityEngine.Color.green
                //);

                EquipmentController.UseWeapon(currentTarget);
            }
        }
        UpdateSurvivalPenalty();
    }

    private void UpdateSurvivalPenalty()
    {
        ResourceStat hungryStat = PlayerStat.GetResourceStat(EStatType.Hungry);
        ResourceStat thirstyStat = PlayerStat.GetResourceStat(EStatType.Thirsty);

        bool isStarving = hungryStat != null && hungryStat.Value <= 0f;
        bool isDehydrated = thirstyStat != null && thirstyStat.Value <= 0f;

        if (isStarving || isDehydrated)
        {
            survivalPenaltyTimer += Time.deltaTime;

            if (survivalPenaltyTimer >= playerData.survivalPenaltyDuration)
            {
                survivalPenaltyTimer = 0f;

                float currentHp = PlayerStat.CurrentHp;

                if (currentHp > 1f)
                {
                    float penaltyDamage = PlayerStat.GetStat(EStatType.playerServivalPenaltyDamage);

                    TakeDotDamage(penaltyDamage, EElementType.None);
                }
            }
        }
        else
        {
            survivalPenaltyTimer = 0f;
        }
    }

    public void SetSpawnPoint(Vector3 spawnPoint)
    {
        this.spawnPoint = spawnPoint;
    }

    public void SetExhausted(float delayTime)
    {
        recoveryStaminaDelayTimer = delayTime;
    }

    public override void OnDamage(float damage, bool crit, bool isDot = false)
    {
        OnPlayerhit?.Invoke(damage, crit, isDot);
    }

    public override void TakeDamage(DmgContext context, bool crit = false)
    {
        if (IsInvincible || IsDashInvincible) return;

        base.TakeDamage(context, crit);
    }

    public override void TakeDotDamage(float damage, EElementType elementType)
    {
        if (IsInvincible) return;
        base.TakeDotDamage(damage, elementType);
    }

    protected override void Die(string killerName)
    {
        transform.DOKill();
        OnPlayerDeath?.Invoke();
        Managers.Event.NotifyEvent<PlayerDeathEvent>(new PlayerDeathEvent { killerName = killerName });
    }

    public void ResetPlayer()
    {
        PlayerMovement.Teleport(spawnPoint);

        IsDead = false;
        PlayerStat.AddModifire(EStatType.Hp, new StatModifier(PlayerStat.GetStat(EStatType.MaxHp), EStatModifireType.Add, this));
        PlayerStat.AddModifire(EStatType.Hungry, new StatModifier(PlayerStat.GetStat(EStatType.MaxHungry), EStatModifireType.Add, this));
        PlayerStat.AddModifire(EStatType.Thirsty, new StatModifier(PlayerStat.GetStat(EStatType.MaxThirsty), EStatModifireType.Add, this));
        PlayerStat.AddModifire(EStatType.Stamina, new StatModifier(PlayerStat.GetStat(EStatType.MaxStamina), EStatModifireType.Add, this));

    }
}
