using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemToolTip : MonoBehaviour
{
    [SerializeField] private GameObject uiContent;

    [Header("아이템 공통 속성")]
    [SerializeField] private Image itemImage;
    [SerializeField] private TMP_Text itemNameText;
    [SerializeField] private TMP_Text itemDescText;

    [Header("컨테이너")]
    [SerializeField] private GameObject equipmentStatContainer;
    [SerializeField] private GameObject weaponStatContainer;
    [SerializeField] private GameObject durabilityContainer;

    [Header("장비 속성")]
    [SerializeField] private TMP_Text durabilityText;
    [SerializeField] private TMP_Text atkText;
    [SerializeField] private TMP_Text defText;
    [SerializeField] private TMP_Text cirDefText;
    [SerializeField] private TMP_Text damageCorrectionText;
    [SerializeField] private TMP_Text incomingDamageText;
    [SerializeField] private TMP_Text atkSpeedText;
    [SerializeField] private TMP_Text atkRangeText;
    [SerializeField] private TMP_Text criDamageText;
    [SerializeField] private TMP_Text moveSpeedText;
    [SerializeField] private TMP_Text maxHpText;

    [Header("무기 속성")]
    [SerializeField] private TMP_Text ammoTypeText;
    [SerializeField] private TMP_Text magazineSizeText;
    [SerializeField] private TMP_Text projectileSpeedText;
    [SerializeField] private TMP_Text reloadSpeedText;
    [SerializeField] private TMP_Text soundSizeText;
    [SerializeField] private TMP_Text projectileCountText;
    [SerializeField] private TMP_Text spreadAngleText;


    private RectTransform rectTransform;
    private RectTransform uiContentRect;
    private Canvas canvas;


    private void Awake()
    {
        itemImage.preserveAspect = true;
        rectTransform = GetComponent<RectTransform>();
        uiContentRect = uiContent.GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        HideUI();
    }

    private void Start()
    {
        Managers.Event.AddEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<UIChangeEvent>(OnUIChanged);
    }

    private void OnUIChanged(UIChangeEvent data)
    {
        if (!data.isOpen) HideUI();
    }

    public void ShowUI(InventorySlotUI slotUI)
    {
        ItemData itemData = slotUI.InventorySlot.ItemData;
        DrawCommonItemData(itemData);
        uiContent.SetActive(true);
        LayoutRebuilder.ForceRebuildLayoutImmediate(uiContentRect);
        Canvas.ForceUpdateCanvases();
        SetTooltipPosition(slotUI.transform.position);
    }


    //화면 밖으로 나가지 않게 위치 조정
    private void SetTooltipPosition(Vector2 targetPosition)
    {
        if (canvas == null) return;

        float tooltipWidth = uiContentRect.rect.width;
        float tooltipHeight = uiContentRect.rect.height;

        Vector2 screenPos = targetPosition;

        screenPos.x = Mathf.Clamp(
            screenPos.x,
            0,
            Screen.width - tooltipWidth
        );

        screenPos.y = Mathf.Clamp(
            screenPos.y,
            0,
            Screen.height - tooltipHeight
        );

        uiContent.transform.position = screenPos;
    }


    //공통 데이터 그림
    private void DrawCommonItemData(ItemData itemData)
    {
        HideAllContainer();
        if (itemData != null)
        {
            itemImage.sprite = itemData.ItemIcon;
            itemNameText.text = itemData.ItemName;
            itemDescText.text = itemData.ItemDescription;

            ConsumableData consumable = itemData as ConsumableData;
            if (consumable != null) itemDescText.text += consumable.BuildDescription();

            EquipmentData equipmentData = itemData as EquipmentData;
            if (equipmentData == null) return;

            DrawEquipmentItemData(equipmentData);
        }
    }



    //공통 장비 속성
    private void DrawEquipmentItemData(EquipmentData equipmentData)
    {
        var statList = equipmentData.EquipmentStats;
        durabilityContainer.SetActive(true);

        durabilityText.text =
            $"{Mathf.RoundToInt(equipmentData.CurrnetDurability)} / {equipmentData.MaxDurability}";

        foreach (var stat in statList)
        {
            EStatType statType = stat.statType;
            string valueText = stat.value != 0 ? stat.value.ToString() : "0";

            if (statType == EStatType.Atk) atkText.text = valueText;
            else if (statType == EStatType.DamageCorrection) damageCorrectionText.text = valueText;
            else if (statType == EStatType.IncomingDamage) incomingDamageText.text = valueText;
            else if (statType == EStatType.AtkSpeed) atkSpeedText.text = valueText;
            else if (statType == EStatType.AtkRange) atkRangeText.text = valueText;
            else if (statType == EStatType.CriDamage) criDamageText.text = valueText;
            else if (statType == EStatType.Speed) moveSpeedText.text = valueText;
            else if (statType == EStatType.MaxHp) maxHpText.text = valueText;
            else if (statType == EStatType.Def) defText.text = valueText;
            else if (statType == EStatType.CritDef) cirDefText.text = valueText;
        }

        equipmentStatContainer.SetActive(true);

        WeaponData weaponData = equipmentData as WeaponData;
        if (weaponData == null) return;

        DrawWeaponItemData(weaponData);
    }


    //무기 데이터 속성
    private void DrawWeaponItemData(WeaponData weaponData)
    {
        if (weaponData is RangedWeaponData) DrawRangedWeaponItemData(weaponData as RangedWeaponData);
        //else if(weaponData is MeleeWeaponData) DrawMeleeWeaponItemData(weaponData as MeleeWeaponData);
    }

    private void DrawRangedWeaponItemData(RangedWeaponData rangedWeaponData)
    {
        ammoTypeText.text = rangedWeaponData.AmmoType.ToString();
        magazineSizeText.text = rangedWeaponData.MagazineSize.ToString();
        projectileSpeedText.text = rangedWeaponData.ProjectileData.ProjectileSpeed.ToString() + "m/s";
        reloadSpeedText.text = rangedWeaponData.ReloadTime.ToString() + "s";
        projectileCountText.text = rangedWeaponData.ProjectileData.ProjectileCount.ToString();
        spreadAngleText.text = rangedWeaponData.ProjectileData.ProjectileSpreadAngle.ToString();

        weaponStatContainer.SetActive(true);
    }


    private void HideAllContainer()
    {
        equipmentStatContainer.SetActive(false);
        weaponStatContainer.SetActive(false);
        durabilityContainer.SetActive(false);
    }



    public void HideUI() => uiContent.SetActive(false);
}