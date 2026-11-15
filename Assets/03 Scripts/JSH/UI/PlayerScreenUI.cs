using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Cysharp.Threading.Tasks;
using System.Threading;
using System;

public class PlayerScreenUI : MonoBehaviour
{
    [SerializeField] private Image hpFill;
    [SerializeField] private TMP_Text hpText;
    [SerializeField] private Image hungryFillImage;
    [SerializeField] private Image thirstyFillImage;

    [SerializeField] private GameObject ammoUI;
    [SerializeField] private TMP_Text currentAmmoText;
    [SerializeField] private TMP_Text hasAmmoText;

    [SerializeField] private Image weaponDurabilityFillImage;
    [SerializeField] private Image chestDurabilityFillImage;
    [SerializeField] private Image helmetDurabilityFillImage;

    [SerializeField] private Image weaponImage;
    [SerializeField] private Sprite normalWeaponSprite;
    [SerializeField] private Sprite brokenWeaponSprite;
    [SerializeField] private Image helmetImage;
    [SerializeField] private Sprite normolHelmetSprite;
    [SerializeField] private Sprite brokenHelmetSprite;
    [SerializeField] private Image chestImage;
    [SerializeField] private Sprite normalChestSprite;
    [SerializeField] private Sprite brokenChestSprite;

    private PlayerUnit currentPlayer;
    private RangedWeaponData currentWeaponData;

    private void Start()
    {
        SetVisible(false);
        Init();
    }
    public void Init()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(OnSpawnPlayer);
        Managers.Event.AddEvent<EquippedEquipmentEvent>(OnEquipped);
        Managers.Event.AddEvent<UnEquippedEquipmentEvent>(OnUnEquipped);
        Managers.Event.AddEvent<DurabilityChangedEvnet>(OnDurabilityChanged);
        Managers.Event.AddEvent<InventorySlotDataChanged>(OnInventorySlotDataChanged);

        SetAmmoUIVisible(false);
        SetDurabilityUIVisible(EEquipmentType.Weapon, false);
        SetDurabilityUIVisible(EEquipmentType.Helmet, false);
        SetDurabilityUIVisible(EEquipmentType.Chestplate, false);
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(OnSpawnPlayer);
        Managers.Event.RemoveEvent<EquippedEquipmentEvent>(OnEquipped);
        Managers.Event.RemoveEvent<UnEquippedEquipmentEvent>(OnUnEquipped);
        Managers.Event.RemoveEvent<DurabilityChangedEvnet>(OnDurabilityChanged);
        Managers.Event.RemoveEvent<InventorySlotDataChanged>(OnInventorySlotDataChanged);

        if(currentPlayer != null && currentPlayer.InventoryHolder != null)
        {
            currentPlayer.InventoryHolder.InventorySys.OnSlotChanged -= OnInventorySlotChanged;
        }

        UnsubscribeWeaponData();
    }

    private void OnSpawnPlayer(PlayerSpawnEvent playerSpawnEvent)
    {
        currentPlayer = playerSpawnEvent.player;

        currentPlayer.InventoryHolder.InventorySys.OnSlotChanged += OnInventorySlotChanged;

        InitStat(currentPlayer);
        SetVisible(true);
    }

    private void InitStat(PlayerUnit playerUnit)
    {
        UnitStat unitStat = playerUnit.unitStat;

        UpdateDrawHpUI(unitStat.CurrentHp, unitStat.GetStat(EStatType.MaxHp));

        unitStat.OnHpChanged += (prevHp, currentHp, maxHp) => UpdateDrawHpUI(currentHp, maxHp);

        ResourceStat thirsty  = unitStat.GetResourceStat(EStatType.Thirsty);
        UpdateDrawThurstyUI(thirsty.Value, unitStat.GetStat(EStatType.MaxThirsty));

        thirsty.OnValueChanged += (prevThirsty, currentThirsty) =>
        {
            float maxThirsty = unitStat.GetStat(EStatType.MaxThirsty);
            UpdateDrawThurstyUI(currentThirsty, maxThirsty);
        };

        ResourceStat hungry = unitStat.GetResourceStat(EStatType.Hungry);
        UpdateDrawHungryUI(hungry.Value, unitStat.GetStat(EStatType.MaxHungry));

        hungry.OnValueChanged += (prevtHungry, currentHungry) =>
        {
            float maxHungry = unitStat.GetStat(EStatType.MaxHungry);
            UpdateDrawHungryUI(currentHungry, maxHungry);
        };
    }

    private async void OnEquipped(EquippedEquipmentEvent data)
    {
        if (data.itemData is RangedWeaponData weaponData)
        {
            currentWeaponData = weaponData;
            currentWeaponData.OnAmmoChanged += UpdateAmmoUI;
            currentWeaponData.OnDurabilityChanged += UpdateWeaponDurabilityUI;

            int hasAmmo = currentPlayer.InventoryHolder.InventorySys.CountItem(Utils.AmmoTypeToItemId.GetItemId(currentWeaponData.AmmoType));
            UpdateAmmoUI(currentWeaponData.CurrentMagazineSize, hasAmmo);
            UpdateWeaponDurabilityUI(currentWeaponData.CurrnetDurability, currentWeaponData.MaxDurability);

            SetAmmoUIVisible(true);
            SetDurabilityUIVisible(EEquipmentType.Weapon, true);
        }
        else if (data.itemData is ArmorData armorData)
        {
            if (armorData.EquipmentType == EEquipmentType.Helmet)
            {
                UpdateHelmetDurabilityUI(armorData.CurrnetDurability, armorData.MaxDurability);
                SetDurabilityUIVisible(EEquipmentType.Helmet, true);
            }
            else if (armorData.EquipmentType == EEquipmentType.Chestplate)
            {
                UpdateChestDurabilityUI(armorData.CurrnetDurability, armorData.MaxDurability);
                SetDurabilityUIVisible(EEquipmentType.Chestplate, true);
            }      
        }

        await UniTask.Yield();
        RefreshStatUI();
    }

    private async void OnUnEquipped(UnEquippedEquipmentEvent data)
    {
        if (data.itemData is RangedWeaponData waponData)
        {
            UnsubscribeWeaponData();
            SetAmmoUIVisible(false);
            SetDurabilityUIVisible(EEquipmentType.Weapon, false);
        }
        else if (data.itemData is ArmorData armorData)
        {
            if(armorData.EquipmentType == EEquipmentType.Helmet)
            {
                SetDurabilityUIVisible(EEquipmentType.Helmet, false); 
            }
            else if(armorData.EquipmentType == EEquipmentType.Chestplate)
            {
                SetDurabilityUIVisible(EEquipmentType.Chestplate, false);
            }
        }

        await UniTask.Yield();
        RefreshStatUI();
    }

    private void OnDurabilityChanged(DurabilityChangedEvnet data)
    {
        switch (data.equipmentType)
        {
            case EEquipmentType.Helmet:
                UpdateHelmetDurabilityUI(data.currentDurability, data.maxDurability);
                break;
            case EEquipmentType.Chestplate:
                UpdateChestDurabilityUI(data.currentDurability, data.maxDurability);
                break;
        }
    }

    private void OnInventorySlotChanged(InventorySlot slot)
    {
        RefreshUI();
    }

    private void OnInventorySlotDataChanged(InventorySlotDataChanged slot)
    {
        RefreshUI();
    }

    private void RefreshUI()
    {
        if (currentWeaponData == null || currentPlayer == null) return;

        string currentAmmoId = Utils.AmmoTypeToItemId.GetItemId(currentWeaponData.AmmoType);
        int hasAmmo = currentPlayer.InventoryHolder.InventorySys.CountItem(currentAmmoId);

        UpdateAmmoUI(currentWeaponData.CurrentMagazineSize, hasAmmo);
    }

    private void RefreshStatUI()
    {
        if(currentPlayer == null || currentPlayer.PlayerStat == null) return;

        UnitStat unitStat = currentPlayer.PlayerStat;

        UpdateDrawHpUI(unitStat.CurrentHp, unitStat.GetStat(EStatType.MaxHp));  
    }

    private void UnsubscribeWeaponData()
    {
        if (currentWeaponData == null) return;
        currentWeaponData.OnAmmoChanged -= UpdateAmmoUI;
        currentWeaponData.OnDurabilityChanged -= UpdateWeaponDurabilityUI;
        currentWeaponData = null;
    }

    public void SetAmmoUIVisible(bool isVisible)
    {
        ammoUI.SetActive(isVisible);
    }

    public void SetDurabilityUIVisible(EEquipmentType eqType, bool isVisible)
    {
        switch (eqType)
        {
            case EEquipmentType.Weapon:
                weaponDurabilityFillImage.gameObject.SetActive(isVisible);
                break;
            case EEquipmentType.Helmet:
                helmetDurabilityFillImage.gameObject.SetActive(isVisible);
                break;
            case EEquipmentType.Chestplate:
                chestDurabilityFillImage.gameObject.SetActive(isVisible);
                break;
        }
    }

    public void UpdateDrawHpUI(float curHp, float maxHp)
    {
        float hpRatio = Mathf.Clamp01(curHp / maxHp);
        hpFill.fillAmount = hpRatio;
        
        hpText.text = $"{curHp} / {maxHp}";
    }

    public void UpdateDrawThurstyUI(float curThursty, float maxThursty)
    {
        thirstyFillImage.fillAmount = Mathf.Clamp01(curThursty / maxThursty);
    }

    public void UpdateDrawHungryUI(float curHungry, float maxHungry)
    {
        hungryFillImage.fillAmount = Mathf.Clamp01(curHungry / maxHungry);
    }

    public void UpdateAmmoUI(int currentAmmo, int hasAmmo)
    {
        currentAmmoText.text = currentAmmo.ToString();
        hasAmmoText.text = hasAmmo.ToString();
    }

    public void UpdateWeaponDurabilityUI(float current, float max)
    {
        weaponDurabilityFillImage.fillAmount = max > 0 ? (current / max) : 0f;

        if(current <= 0)
        {
            weaponImage.sprite = brokenWeaponSprite;
            weaponImage.color = Color.red;
        }
        else
        {
            weaponImage.sprite = normalWeaponSprite;
            weaponImage.color = new Color32(255, 255, 255, 50);
        }
    }

    public void UpdateHelmetDurabilityUI(float current, float max)
    {
        helmetDurabilityFillImage.fillAmount = max > 0 ? (Mathf.Ceil(current / max)) : 0f;

        if(current <= 0)
        {             
            helmetImage.sprite = brokenHelmetSprite;
            helmetImage.color = Color.red;
        }
        else 
        { 
            helmetImage.sprite = normolHelmetSprite;
            helmetImage.color = new Color32(255, 255, 255, 50);
        }
    }

    public void UpdateChestDurabilityUI(float current, float max)
    {
        chestDurabilityFillImage.fillAmount = max > 0 ? (Mathf.Ceil(current / max)) : 0f;

        if(current <= 0)
        {
            chestImage.sprite = brokenChestSprite;
            chestImage.color = Color.red;
        }
        else
        {
            chestImage.sprite = normalChestSprite;
            chestImage.color = new Color32(255, 255, 255, 50);
        }
    }

    private void SetVisible(bool visible) => gameObject.SetActive(visible);
}
