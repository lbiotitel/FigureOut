using System;
using UnityEngine;


//장비 진화 UI의 슬롯을 정의합니다
public class EvolutionSlotUI : InventorySlotUI
{

    public override void UpdateUISlot(InventorySlot slot)
    {
        EquipmentData equipmentData = slot.ItemData as EquipmentData;
        if (equipmentData == null) return;

        if (slot.ItemData != null)
        {
            itemImage.sprite = slot.ItemData.ItemIcon;
            itemImage.color = Color.white;
            itemNameBG.SetActive(true);
            itemNameText.text = slot.ItemData.ItemName;
        }
        else
        {
            ClearSlot();
        }
    }


    public override void ClearSlot()
    {
        inventorySlot?.ClearSlot();
        itemImage.color = Color.clear;
        itemNameBG.SetActive(false);
        itemNameText.text = string.Empty;
    }

}
