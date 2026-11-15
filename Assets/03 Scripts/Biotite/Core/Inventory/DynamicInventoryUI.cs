using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


//동적 인벤토리 - 슬롯이 프리팹으로 생성되어야 하는 경우 사용
public class DynamicInventoryUI : InventoryUIBase
{

    [SerializeField] private InventoryHolder inventoryHolder;
    [SerializeField] private Transform slotParent;
    private InventoryUIBase inventoryUI;

    protected override void Start()
    {
        base.Start();

        if (inventoryHolder != null)
        {
            inventorySys = inventoryHolder.InventorySys;
            inventorySys.OnSlotChanged += UpdateSlot;
            inventorySys.OnSortedSlots += UpdateSortedSlot;
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
        GameObject slotPrefab = Managers.Data.GetData<GameObject>(KeyDefines.ResourceKey.INVENTORY_SLOT_PREFAB);

        for (int i = 0; i < inventorySys.InventorySize; i++)
        {
            InventorySlotUI slotUI = Instantiate(slotPrefab, slotParent).GetComponent<InventorySlotUI>();
            slotDict.Add(slotUI, inventorySys.InventorySlots[i]);
            slotUI.Init(inventorySys.InventorySlots[i]);
            slotUI.SetParentUI(inventoryUI);

            BindEvents(slotUI);
        }
    }




}
