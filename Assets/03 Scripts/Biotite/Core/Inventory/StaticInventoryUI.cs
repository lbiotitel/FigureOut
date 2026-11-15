

using System.Collections.Generic;
using UnityEngine;


//정적 인벤토리 - 슬롯이 이미 존재하는 경우 사용
public class StaticInventoryUI : InventoryUIBase
{
    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private InventorySlotUI[] slots;
    private InventoryUIBase inventoryUI;

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySys = inventoryHolder.InventorySys;
            inventorySys.OnSlotChanged += UpdateSlot;
            inventoryUI = GetComponent<InventoryUIBase>();
        }
        else
        {
            Logger.LogWarning($"인벤토리 할당 안됨 {this.gameObject}");
        }

        InitSlot(inventorySys);
    }

    public override void InitSlot(InventorySystem inventorySys)
    {
        slotDict = new Dictionary<InventorySlotUI, InventorySlot>();

        if (slots.Length != inventorySys.InventorySize) Logger.LogError("인벤토리 슬롯과 인벤토리 실제 사이즈가 다릅니다.");

        for (int i = 0; i < inventorySys.InventorySize; i++)
        {
            slotDict.Add(slots[i], inventorySys.InventorySlots[i]);
            slots[i].Init(inventorySys.InventorySlots[i]);
            slots[i].SetParentUI(inventoryUI);

            BindEvents(slots[i]);
        }
    }
}
