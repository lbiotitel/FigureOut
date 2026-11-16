using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using static UnityEditor.Progress;


[System.Serializable]
public class InventorySystem
{
    [SerializeField] private List<InventorySlot> inventorySlots;

    public List<InventorySlot> InventorySlots => inventorySlots;
    public int InventorySize => inventorySlots.Count;
    public event UnityAction<InventorySlot> OnSlotChanged;
    public event UnityAction<List<InventorySlot>> OnSortedSlots;

    public InventorySystem(int size)
    {
        inventorySlots = new List<InventorySlot>(size);

        for (int i = 0; i < size; i++)
        {
            inventorySlots.Add(new InventorySlot());
        }

    }

    public void SortInvetorySlot()
    {
        inventorySlots.Sort();
        OnSortedSlots?.Invoke(inventorySlots);
    }

    //아이템 추가
    public virtual bool AddToInventory(ItemData itemData, int amountToAdd)
    {
        //이미 아이템이 존재하면 스택에 쌓기
        if (ContainsItem(itemData, out List<InventorySlot> invSlots))
        {
            foreach (var slot in invSlots)
            {
                if (slot.IsEnoughSlotStack(amountToAdd))
                {
                    slot.AddToStack(amountToAdd);
                    OnSlotChanged?.Invoke(slot);
                    Managers.Event.NotifyEvent<InventoryDataChangedEvent>(new InventoryDataChangedEvent { });
                    return true;
                }
            }
        }

        //아이템이 새로운 아이템일 때 
        if (HasFreeSlot(out InventorySlot freeSlot))
        {
            if (freeSlot.IsEnoughSlotStack(amountToAdd))
            {
                freeSlot.UpdateSlot(itemData, amountToAdd);
                OnSlotChanged?.Invoke(freeSlot);
                Managers.Event.NotifyEvent<InventoryDataChangedEvent>(new InventoryDataChangedEvent { });
                return true;
            }
        }

        return false;
    }




    //특정 아이템을 amount 만큼 삭제 시킵니다.
    public void RemoveItem(string itemID, int amount = 1)
    {
        int removeAmount = amount;
        if (ContainsItem(itemID, out List<InventorySlot> invSlots))
        {
            for (int i = 0; i < invSlots.Count; i++)
            {
                int currentAmount = invSlots[i].StackSize;

                if (removeAmount < currentAmount)
                {
                    invSlots[i].RemoveFromStack(removeAmount);
                    OnSlotChanged?.Invoke(invSlots[i]);
                    break;
                }
                else
                {
                    invSlots[i].RemoveFromStack(currentAmount);
                    removeAmount -= currentAmount;
                    OnSlotChanged?.Invoke(invSlots[i]);
                }
            }
            Managers.Event.NotifyEvent<InventoryDataChangedEvent>(new InventoryDataChangedEvent { });
        }
    }


    //현재 아이디의 아이템 전체 갯수를 가져옵니다.
    public int CountItem(string itemID)
    {
        int count = 0;
        if (ContainsItem(itemID, out List<InventorySlot> invSlots))
        {
            foreach (var slot in invSlots)
            {
                count += slot.StackSize;
            }
        }
        return count;
    }

    //특정한 아이템 아이디와 일치하는 아이템 슬롯들을 가져옵니다
    public bool ContainsItem(string itemID, out List<InventorySlot> invSlots)
    {
        invSlots = inventorySlots.Where(i => i.ItemData != null && i.ItemData.ItemID == itemID).ToList();
        if (invSlots == null) return false;
        return invSlots.Count <= 0 ? false : true;
    }

    public bool ContainsItem(string itemID)
        => ContainsItem(itemID, out List<InventorySlot> invSlots);


    //아이템 id 와 일치하는 슬롯들을 다 가져옴
    private bool ContainsItem(ItemData itemData, out List<InventorySlot> invSlots)
    {
        return ContainsItem(itemData.ItemID, out invSlots);
    }


    //빈 슬롯이 있는 지 확인
    private bool HasFreeSlot(out InventorySlot freeSlot)
    {
        freeSlot = inventorySlots.FirstOrDefault(i => i.ItemData == null);
        return freeSlot != null;
    }


    public bool HasFreeSlot()
    {
        InventorySlot inventorySlot = inventorySlots.FirstOrDefault(i => i.ItemData == null);
        return inventorySlot != null;
    }


    //모든 아이템 데이터를 삭제시킵니다.
    public void ClearItem()
    {
        foreach (var slot in inventorySlots)
        { 
            slot.ClearSlot();
            OnSlotChanged?.Invoke(slot);
        }
        Managers.Event.NotifyEvent<InventoryDataChangedEvent>(new InventoryDataChangedEvent { });
    }
}
