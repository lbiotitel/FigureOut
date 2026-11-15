using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributionSlotUI : InventorySlotUI
{
    public override void UpdateUISlot(InventorySlot slot)
    {
        EquipmentData equipmentData = slot.ItemData as EquipmentData;
        if (equipmentData==null ||equipmentData.EquipmentType !=EEquipmentType.Weapon ) return;

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
