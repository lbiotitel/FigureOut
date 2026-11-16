using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


//인벤토리 기능을 사용할 클래스는 모두 상속 받아야 합니다.
public abstract class InventoryUIBase : MonoBehaviour
{
    [SerializeField] private GameObject uiContent;

    protected MouseObject mouseObject;

    protected InventorySystem inventorySys;
    protected Dictionary<InventorySlotUI, InventorySlot> slotDict;

    public InventorySystem InventorySys => inventorySys;
    public Dictionary<InventorySlotUI, InventorySlot> SlotDict => slotDict;


    protected virtual void Start()
    {
        mouseObject = Managers.UI.MouseObject;
    }


    public abstract void InitSlot(InventorySystem inventorySys);

    //특정 슬롯 수정 시 그 슬롯만 업데이트
    protected virtual void UpdateSlot(InventorySlot updatedSlot)
    {
        foreach (var slot in slotDict)
        {
            if (slot.Value == updatedSlot)
            {
                slot.Key.UpdateUISlot(updatedSlot);
                return;
            }
        }
    }


    //인벤토리 정렬
    public void SortInventory()
        => inventorySys.SortInvetorySlot();


    //졍렬시 슬롯 전체 다시 업데이트
    protected virtual void UpdateSortedSlot(List<InventorySlot> sortedSlots)
    {
        var uiKeys = new List<InventorySlotUI>(slotDict.Keys);

        for (int i = 0; i < uiKeys.Count; i++)
        {
            InventorySlotUI ui = uiKeys[i];
            InventorySlot newData = sortedSlots[i];

            ui.Init(newData);
            slotDict[ui] = newData;
            ui.UpdateUISlot(newData);
        }
    }


    //슬롯 클릭시
    public virtual void SlotClicked(InventorySlotUI clickedSlotUI)
    {
        //마우스에는 데이터가가 없고 슬롯에는 데이터가 있을 시
        if (clickedSlotUI.InventorySlot.ItemData != null && mouseObject.InventorySlot.ItemData == null)
        {
            //분할키 누름
            if (Managers.Input.InventorySplitButton && clickedSlotUI.InventorySlot.SplitStack(out InventorySlot slot))
            {
                //아이템 분할
                mouseObject.UpdateSlot(slot);
                clickedSlotUI.UpdateUISlot();
                return;
            }
            else
            {

                mouseObject.UpdateSlot(clickedSlotUI.InventorySlot);
                clickedSlotUI.ClearSlot();
                return;
            }
        }

        SetEmptySlot(clickedSlotUI);

        //마우스 슬롯 둘다 데이터가 있을 시
        if (clickedSlotUI.InventorySlot.ItemData != null && mouseObject.InventorySlot.ItemData != null)
        {
            //두개의 아이템 데이터 ID가 다를시
            if (clickedSlotUI.InventorySlot.ItemData.ItemID != mouseObject.InventorySlot.ItemData.ItemID)
            {
                SwapSlots(clickedSlotUI);
                return;
            }
            else
            {
                //2개의 아이템 데이터가 같을 시

                //현재수 + 더한 아이템 수 가 최대 스택을 못넘을시
                if (clickedSlotUI.InventorySlot.IsEnoughSlotStack(mouseObject.InventorySlot.StackSize))
                {
                    clickedSlotUI.InventorySlot.AssignItem(mouseObject.InventorySlot);
                    clickedSlotUI.UpdateUISlot();

                    mouseObject.ClearSlot();
                    return;
                }
                else if (clickedSlotUI.InventorySlot.SlotLeftInStack(mouseObject.InventorySlot.StackSize, out int remainingAmount))
                {
                    if (remainingAmount < 1) SwapSlots(clickedSlotUI); //스택이 가득 찼을 때
                    else
                    {
                        int remainMouse = mouseObject.InventorySlot.StackSize - remainingAmount;
                        clickedSlotUI.InventorySlot.AddToStack(remainingAmount);
                        clickedSlotUI.UpdateUISlot();

                        var newItemSlot = new InventorySlot(mouseObject.InventorySlot.ItemData, remainMouse);
                        mouseObject.ClearSlot();
                        mouseObject.UpdateSlot(newItemSlot);
                        return;
                    }
                }
            }
        }
    }

    protected virtual void SetEmptySlot(InventorySlotUI clickedSlotUI)
    {
        //마우스에는 데이터가가 있고 슬롯에는 데이터가 없을 시
        if (clickedSlotUI.InventorySlot.ItemData == null && mouseObject.InventorySlot.ItemData != null)
        {
            clickedSlotUI.InventorySlot.AssignItem(mouseObject.InventorySlot);
            clickedSlotUI.UpdateUISlot();

            mouseObject.ClearSlot();
        }
    }


    //마우스와 슬롯에 있는 데이터를 교환합니다.
    protected virtual void SwapSlots(InventorySlotUI clickedSlotUI)
    {
        var clonedSlot = new InventorySlot(mouseObject.InventorySlot.ItemData, mouseObject.InventorySlot.StackSize);
        mouseObject.ClearSlot();

        mouseObject.UpdateSlot(clickedSlotUI.InventorySlot);

        clickedSlotUI.ClearSlot();
        clickedSlotUI.InventorySlot.AssignItem(clonedSlot);
        clickedSlotUI.UpdateUISlot();
    }

    protected void BindEvents(InventorySlotUI slotUI)
    {
        EventTrigger eventTrigger = slotUI.AddComponent<EventTrigger>();
        BindEvent(slotUI, eventTrigger, EventTriggerType.PointerEnter, HandleEnter);
        BindEvent(slotUI, eventTrigger, EventTriggerType.PointerExit, HandleExit);
        BindEvent(slotUI, eventTrigger, EventTriggerType.PointerClick, HandleMouseClicked);
    }


    //마우스 이벤트 바인딩
    protected void BindEvent(InventorySlotUI slotUI, EventTrigger eventTrigger, EventTriggerType triggerType, Action<BaseEventData, InventorySlotUI> action)
    {
        EventTrigger.Entry entry = new EventTrigger.Entry();

        entry.eventID = triggerType;

        entry.callback.AddListener((data) => { action?.Invoke(data, slotUI); });

        eventTrigger.triggers.Add(entry);
    }


    protected virtual void HandleEnter(BaseEventData eventData, InventorySlotUI targetSlot)
    {
        if (targetSlot.InventorySlot.IsEmpty()) return;
        Managers.UI.ItemToolTip.ShowUI(targetSlot);
    }

    protected virtual void HandleExit(BaseEventData eventData, InventorySlotUI targetSlot)
    {
       Managers.UI.ItemToolTip.HideUI();
    }

    protected virtual void HandleMouseClicked(BaseEventData eventData, InventorySlotUI targetSlot) { }

    public virtual void ShowUI() => uiContent.SetActive(true);
    public virtual void HideUI() => uiContent.SetActive(false);
}
