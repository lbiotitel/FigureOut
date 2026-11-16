using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerInventoryUI : DynamicInventoryUI
{

    [SerializeField] private TMP_Text dollarText;
    
    private Dictionary<string, int> itemCoolTimeDict = new Dictionary<string, int>();
    private bool isOpen = false;
    private bool isOpenTab = false;
    private PlayerUnit playerUnit;


    protected override void Start()
    {
        base.Start();
        playerUnit = GetComponentInParent<PlayerUnit>();
        Managers.Event.AddEvent<InteractEvent>(HandleIteract);
        Managers.Event.AddEvent<InventorySlotDataChanged>(HandleReloadEvent);
        Managers.Event.AddEvent<PlayerTriggerExit>(HandleTriggerExit);

        HideUI();

        Managers.Game.OnDollarValueChanged += HandleDollarValueChanged;
    }


    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<InteractEvent>(HandleIteract);
        Managers.Event.RemoveEvent<InventorySlotDataChanged>(HandleReloadEvent);
        Managers.Event.RemoveEvent<PlayerTriggerExit>(HandleTriggerExit);

        Managers.Game.OnDollarValueChanged -= HandleDollarValueChanged;
    }


    private void Update()
    {
        ToggleInvenUI();

        if(isOpen && isOpenTab && (Managers.Input.MoveInput != Vector2.zero))
        {
            isOpen = false;
            isOpenTab = false;
            HideUI();
        }
    }

    private void HandleIteract(InteractEvent interact)
    {
        isOpen = true;
        ShowUI();
    }


    private void ToggleInvenUI()
    {
        if (Managers.Input.InventoryButton)
        {
            if (!isOpen)
            {
                isOpen = true;
                isOpenTab = true;
                ShowUI();
            }
            else
            {
                if (isOpenTab)
                {
                    isOpen = false;
                    isOpenTab = false;
                    HideUI();
                }
            }
        }
    }

    public override void SlotClicked(InventorySlotUI clickedSlotUI)
    {
        base.SlotClicked(clickedSlotUI);
        Managers.Event.NotifyEvent<InventoryDataChangedEvent>(new InventoryDataChangedEvent { });
    }

    protected override void HandleMouseClicked(BaseEventData eventData, InventorySlotUI targetSlot)
    {
        PointerEventData pointerEventData = eventData as PointerEventData;
        if (pointerEventData == null) return;
        if (targetSlot.InventorySlot.IsEmpty()) return;

        if(pointerEventData.button == PointerEventData.InputButton.Right)
        {
            UseItem(targetSlot);
        }
    }

    //아이템 쿨타임
    private async void ItemCoolTime(string code, float time)
    {
        itemCoolTimeDict.Add(code,1);
        await UniTask.WaitForSeconds(time);
        itemCoolTimeDict.Remove(code);
    }


    //아이템 사용 로직
    private void UseItem(InventorySlotUI targetSlot)
    {
        //소모품 아이템만 사용 가능
        ConsumableData consumableData = targetSlot.InventorySlot.ItemData as ConsumableData;

        if (consumableData != null)
        {
            Logger.Log($"{consumableData.ItemName} 사용함");
            List<ItemEffect> itemEffects = consumableData.Effects;

            if (itemEffects == null || itemEffects.Count == 0) return;

            if (consumableData.CoolTime > 0)
            {
                if (itemCoolTimeDict.TryGetValue(consumableData.ItemID, out int value)) return;
                ItemCoolTime(consumableData.ItemID, consumableData.CoolTime);
            }


            foreach (var effect in itemEffects)
            {
                effect.ExcuteEffect(playerUnit);
            }

            //개수를 감소시키고 UI 갱신
            targetSlot.InventorySlot.RemoveFromStack(1);
            targetSlot.UpdateUISlot();
        }
    }


    // 총기 발사 시 인벤토리 UI 갱신
    private void HandleReloadEvent(InventorySlotDataChanged data)
    {
        List<InventorySlot> inventorySlots = data.inventorySlots;
        if (inventorySlots == null) return;

        foreach (var slot in inventorySlots)
        { 
            UpdateSlot(slot);
        }
    }

    
    //달러 UI 업데이트
    private void HandleDollarValueChanged(int obj)
        => dollarText.text = $"{obj} $";

    private void HandleTriggerExit(PlayerTriggerExit data)
    {
        if(isOpen)
        {
            isOpen = false;
            HideUI();
        }
    }
}
