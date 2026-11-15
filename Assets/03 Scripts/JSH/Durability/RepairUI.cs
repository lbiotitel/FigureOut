using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class RepairUI : DynamicInventoryUI
{
    [SerializeField] private Transform repairTr;
    [SerializeField] private Transform animTargetTr;
    [SerializeField] private float showAnimDuration = 0.5f;
    [SerializeField] private Button repairButton;
    [SerializeField] private TMP_Text requiredDollarText;

    private Vector3 originPos;
    private int totalRepairCost = 0;

    protected override void Start()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    private void HandlePlayerSpawn(PlayerSpawnEvent data)
    {
        base.Start();
        Init();
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    public void Init()
    {
        originPos = repairTr.transform.position;

        if (repairButton != null)
        {
            repairButton.onClick.RemoveAllListeners();
            repairButton.onClick.AddListener(OnRepairButtonClicked);
        }

        if (inventorySys != null)
        {
            inventorySys.OnSlotChanged += (slot) => UpdateTotalRepairCost();
        }

        HideUI();
    }

    private List<InventorySlot> GetRepairbleSlots()
    {
        List<InventorySlot> vaildSlots = new List<InventorySlot>();

        for (int i = 0; i < inventorySys.InventorySlots.Count; i++)
        {
            InventorySlot slot = inventorySys.InventorySlots[i];
            if (!slot.IsEmpty() && slot.ItemData is EquipmentData)
            {
                vaildSlots.Add(slot);
            }
        }
        return vaildSlots;
    }

    private void UpdateTotalRepairCost()
    {
        totalRepairCost = 0;

        foreach (InventorySlot slot in GetRepairbleSlots())
        {
            EquipmentData equipmentData = slot.ItemData as EquipmentData;

            int basePrice = equipmentData.Price;
            float maxDurability = equipmentData.MaxDurability;
            float currentDurability = equipmentData.CurrnetDurability;

            float missingDurability = maxDurability - currentDurability;
            float rarityMultiplier = equipmentData.Rarity switch
            {
                ERarity.Normal => 1f,
                ERarity.Rare => 1.2f,
                ERarity.Epic => 1.4f,
                ERarity.Legend => 1.6f,
                _ => 1f
            };

            if (maxDurability > 0)
            {
                float cost = basePrice * (missingDurability / maxDurability) * rarityMultiplier;
                Logger.Log($"{cost}");
                totalRepairCost += Mathf.CeilToInt(cost);
            }
        }

        if (requiredDollarText != null)
        {
            requiredDollarText.text = $"${totalRepairCost}";

            if (Managers.Game.Dollar < totalRepairCost && totalRepairCost != 0)
            {
                requiredDollarText.color = Color.red;
            }
            else
            {
                requiredDollarText.color = Color.white;
            }
        }
    }

    private void OnRepairButtonClicked()
    {
        if(Managers.Game.Dollar < totalRepairCost)
        {
            return;
        }

        PlayerUnit player = Managers.Game.PlayerUnit;
        if (player == null) return;

        List<InventorySlot> targetSlots = GetRepairbleSlots();
        if(targetSlots.Count == 0)
        {
            return;
        }

        Managers.Game.SpendDollar(totalRepairCost);

        foreach (InventorySlot slot in targetSlots)
        {
            Managers.Audio.PlaySfx(KeyDefines.SoundKey.RepairSfx);
            EquipmentData equipmentData = slot.ItemData as EquipmentData;
            int stackSize = slot.StackSize;

            equipmentData.CurrnetDurability = equipmentData.MaxDurability;

            bool isAdded = player.InventoryHolder.InventorySys.AddToInventory(equipmentData, stackSize);

            if (isAdded)
            {
                slot.ClearSlot();
            }
            else
            {
                Transform targetTr = player.transform;
                Vector3 targetPos = targetTr.position
                    + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                targetPos.y = 0f;

                DropItem dropItem = Managers.Object.CreateDropObject(
                    equipmentData, stackSize, targetTr.position, Quaternion.identity);

                dropItem.transform.DOJump(targetPos, 1f, 1, 0.3f);

                slot.ClearSlot();
            }
        }

        foreach (var slotUI in slotDict.Keys)
        {
            slotUI.UpdateUISlot();
        }

        UpdateTotalRepairCost();
    }

    public override void SlotClicked(InventorySlotUI clickedSlotUI)
    {
        if(!mouseObject.InventorySlot.IsEmpty() && mouseObject.InventorySlot.ItemData.ItemType != EItemType.Equipment)
        {
            return;
        }
        base.SlotClicked(clickedSlotUI);

        UpdateTotalRepairCost();
    }

    public override void ShowUI()
    {
        if(repairTr.gameObject.activeSelf) return;

        repairTr.transform.position = originPos;
        repairTr.gameObject.SetActive(true);
        repairTr.DOMove(animTargetTr.position, showAnimDuration);
    }

    public override void HideUI()
    {
        repairTr.gameObject.SetActive(false);
    }
}
