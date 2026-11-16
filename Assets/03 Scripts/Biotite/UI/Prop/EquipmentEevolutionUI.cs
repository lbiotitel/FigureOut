using System;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//무기 진화에 대한 UI 입니다.
public class EquipmentEevolutionUI : StaticInventoryUI
{
    [Header("EquipmentEvolutionUI 속성")]
    [SerializeField] private EvolutionSlotUI equipmentSlotUI; // 진화 전 장비 슬롯
    [SerializeField] private EvolutionSlotUI evolutionSlotUI; //진화 후 장비 슬롯
    [SerializeField] private Button evolutionButton;
    [SerializeField] private EvolutionAnimUI evolutionAnimUI;
    private bool isEvolution = false;

    //재료창
    [SerializeField] private int poolingCount = 10;
    [SerializeField] private Transform resourceTransform; //진화 재료를 보여줄 트랜스폼
    [SerializeField] private ResourceNodeUI resourceNodeUIPrefab; //진화 재료 노드 프리팹
    private List<ResourceNodeUI> resourceNodeUIs = new List<ResourceNodeUI>();

    //달러
    [SerializeField] private TMP_Text dollarText;

    private EvolutionRecipeData evolutionRecipeData;
    private bool eventRegistered = false;


    public void Init()
    {
        evolutionAnimUI.HideUI();
        evolutionAnimUI.OnBlackScreen += Evolution;
        evolutionAnimUI.OnAnimFinished += EvolutionAnimFinished;

        evolutionButton.onClick.AddListener(OnEvolutionButtonClicked);

        for (int i = 0; i < poolingCount; i++)
        {
            ResourceNodeUI resourceNodeUI = Instantiate(resourceNodeUIPrefab, resourceTransform);
            resourceNodeUI.Init();
            resourceNodeUIs.Add(resourceNodeUI);
        }

        HideAllResourceNodeUIs();
    }


    //슬롯 클릭 시
    public override void SlotClicked(InventorySlotUI clickedSlotUI)
    {
        HideAllResourceNodeUIs();
        evolutionSlotUI.ClearSlot();
        evolutionSlotUI.UpdateUISlot();
        base.SlotClicked(clickedSlotUI);
        EquipmentData equipmentData = clickedSlotUI.InventorySlot.ItemData as EquipmentData;
        if (equipmentData == null) return;
        ShowRevolutionEquipment(equipmentData);
    }

    protected override void SwapSlots(InventorySlotUI clickedSlotUI)
    {
        EquipmentData mouseEquipmentData = mouseObject.InventorySlot.ItemData as EquipmentData;
        if (mouseEquipmentData == null) return;
        base.SwapSlots(clickedSlotUI);
    }

    protected override void SetEmptySlot(InventorySlotUI clickedSlotUI)
    {
        if (!mouseObject.InventorySlot.IsEmpty() && mouseObject.InventorySlot.ItemData.ItemType != EItemType.Equipment) return;
        base.SetEmptySlot(clickedSlotUI);
    }


    //진화 레시피를 가지고와서 재료 슬롯에 보여줍니다.
    private void ShowRevolutionEquipment(EquipmentData equipmentData)
    {
        string evolutionRecipeID = equipmentData.RevolutionRecipeID;
        if (evolutionRecipeID == string.Empty) return;

        evolutionRecipeData = null;
        evolutionRecipeData = Managers.Data.GetData<EvolutionRecipeData>(evolutionRecipeID);
        if (evolutionRecipeData == null) return;

        EquipmentData evolutionEquipment = Managers.Data.GetData<EquipmentData>(evolutionRecipeData.RevolutionEquipmentID);
        if (evolutionEquipment == null)
        {
            Logger.Log("진화 장비의 id 가 잘못되었습니다");
            return;
        }

        ShowEvolutionEquipmentResourceUI(evolutionRecipeData);

        InventorySlot newItemSlot = new InventorySlot(evolutionEquipment, evolutionEquipment.MaxStackCount);

        evolutionSlotUI.InventorySlot.AssignItem(newItemSlot);
        evolutionSlotUI.UpdateUISlot();
    }

    private void ShowEvolutionEquipmentResourceUI(EvolutionRecipeData evolutionRecipeData)
    {
        List<EevolutionRecipeRequiredData> requiredDatas = evolutionRecipeData.RecipeRequiredDatas;
        //재료 슬롯 초기화

        PlayerUnit player = GameObject.FindAnyObjectByType<PlayerUnit>();
        InventorySystem InventorySys = player.InventoryHolder.InventorySys;

        for (int i = 0; i < requiredDatas.Count; i++)
        {
            ItemData itemData = Managers.Data.GetData<ItemData>(requiredDatas[i].itemID);
            int requiredCount = requiredDatas[i].amount;
            if (itemData == null)
            {
                Logger.Log("진화 재료의 id 가 잘못되었습니다");
                continue;
            }

            resourceNodeUIs[i].SetData(itemData.ItemIcon, InventorySys.CountItem(itemData.ItemID), requiredCount);
            resourceNodeUIs[i].ShowUI();
        }

        dollarText.text = $"{evolutionRecipeData.RequiredDollar}$";
    }


    //장비 진화
    private void OnEvolutionButtonClicked()
    {
        if (CanEvolution() && !isEvolution)
        {
            isEvolution = true;
            evolutionAnimUI.ShowUI();
            evolutionAnimUI.PlayAnimation(evolutionSlotUI.InventorySlot.ItemData.ItemIcon);
            Managers.Audio.PlaySfx(KeyDefines.SoundKey.EvolutionSfx);
        }
    }


    public void Evolution()
    {
        if (evolutionSlotUI.InventorySlot.IsEmpty()) return;
        EquipmentData equipmentData = evolutionSlotUI.InventorySlot.ItemData as EquipmentData;
        if (equipmentData == null) return;

        var requiredDatas = evolutionRecipeData.RecipeRequiredDatas;

        PlayerUnit player = Managers.Game.PlayerUnit;

        foreach (var data in requiredDatas)
        {
            player.InventoryHolder.InventorySys.RemoveItem(data.itemID, data.amount);
        }
        player.InventoryHolder.InventorySys.AddToInventory(equipmentData, 1);
        Managers.Game.SpendDollar(evolutionRecipeData.RequiredDollar);

        equipmentSlotUI.ClearSlot();
        evolutionSlotUI.ClearSlot();
        HideAllResourceNodeUIs();
    }


    //진화에 필요한 모든 재료가 다 있는지 확인합니다.
    private bool CanEvolution()
    {
        if (evolutionSlotUI.InventorySlot.IsEmpty()) return false;

        int currentDollar = Managers.Game.Dollar;
        if (currentDollar < evolutionRecipeData.RequiredDollar) return false;

        PlayerUnit player = Managers.Game.PlayerUnit;
        InventorySystem InventorySys = player.InventoryHolder.InventorySys;

        var requiredDatas = evolutionRecipeData.RecipeRequiredDatas;

        foreach (var data in requiredDatas)
        {
            int currentAmount = InventorySys.CountItem(data.itemID);
            if (currentAmount < data.amount)
            {
                return false;
            }
        }

        return true;
    }


    //모든 재료 UI를 감춥니다.
    private void HideAllResourceNodeUIs()
    {
        foreach (var materialNodeUI in resourceNodeUIs)
        {
            materialNodeUI.HideUI();
        }
    }


    private void EvolutionAnimFinished()
    {
        isEvolution = false;
        evolutionAnimUI.HideUI();
    }


    //인벤토리 슬롯이 변경되었을 때 진화 장비와 재료 UI를 초기화합니다.
    private void HandleInventorySlotChnagedEvent(InventoryDataChangedEvent inventoryDataChanged)
    {
        EquipmentData equipmentData = equipmentSlotUI.InventorySlot.ItemData as EquipmentData;
        if (equipmentData == null) return;
        ShowRevolutionEquipment(equipmentData);
    }


    public override void ShowUI()
    {
        gameObject.SetActive(true);
        if (eventRegistered) return;
        eventRegistered = true;
        Managers.Event.AddEvent<InventoryDataChangedEvent>(HandleInventorySlotChnagedEvent);
    }

    public override void HideUI()
    {
        gameObject.SetActive(false);
        if (!eventRegistered) return;
        eventRegistered = false;
        Managers.Event.RemoveEvent<InventoryDataChangedEvent>(HandleInventorySlotChnagedEvent);
    }
}
