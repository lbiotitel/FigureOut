using System;
using UnityEngine;


[System.Serializable]
public class InventorySlot : IComparable<InventorySlot> 
{

    [SerializeField] private ItemData itemData;
    [SerializeField] private int stackSize;

    public ItemData ItemData => itemData;
    public int StackSize => stackSize;


    public InventorySlot(ItemData itemData, int amount)
    { 
        this.itemData = itemData;
        stackSize = amount;
    }

    public InventorySlot()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        itemData = null;
        stackSize = -1;
    }


    public bool IsEmpty()
    {
        return itemData == null;
    }   

    public bool SlotLeftInStack(int amountToAdd, out int amountRemaining)
    {
        amountRemaining = itemData.MaxStackCount - amountToAdd;
        return IsEnoughSlotStack(amountToAdd);
    }

    public bool IsEnoughSlotStack(int amountToAdd)
    {
        if (ItemData == null || (itemData != null && stackSize + amountToAdd <= itemData.MaxStackCount)) return true;
        return false;
    }

    public void AddToStack(int amount)
    {
        stackSize += amount;
    }

    public void RemoveFromStack(int amount)
    { 
        stackSize -= amount;
        if (stackSize <= 0) ClearSlot();
    }

    internal void AssignItem(InventorySlot inventorySlot)
    {
        //같은 아이템일 시 수량만 넘겨 줌
        if (itemData != null && inventorySlot.itemData != null && itemData.ItemID == inventorySlot.itemData.ItemID)
            AddToStack(inventorySlot.stackSize);
        else
        {
            //다른 아이템일시 새로운 아이템 할당
            itemData = inventorySlot.itemData;
            stackSize = 0;
            AddToStack(inventorySlot.stackSize);
        }
    }

    //데이터 수정
    public void UpdateSlot(ItemData itemData, int amount)
    {
        this.itemData = itemData;
        stackSize = amount;
    }


    //아이템 분할

    public bool SplitStack(out InventorySlot slot)
    {
        if (stackSize <= 1)
        {
            //수량이 1개 이하이면 분할 하지 않음
            slot = null;
            return false;        
        }

        //수량이 2개 이상일시 2개로 나누어 out에 할당
        int halfStack = Mathf.RoundToInt(stackSize / 2);
        RemoveFromStack(halfStack);

        slot = new InventorySlot(itemData, halfStack);

        return true;
    }

    
    //정렬 우선순위
    public int CompareTo(InventorySlot other)
    {
        if (itemData == null && other.itemData == null) return 0;
        if (itemData == null) return 1;
        if (other.itemData == null) return -1;

        if (itemData.ItemType != other.itemData.ItemType)
        {
            return itemData.ItemType.CompareTo(other.itemData.ItemType);
        }

        if (itemData.ItemID != other.itemData.ItemID)
        {
            return itemData.ItemID.CompareTo(other.itemData.ItemID);
        }

        return other.stackSize.CompareTo(this.stackSize);
    }
}