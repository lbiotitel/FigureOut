using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributionMaterialSlotUI : InventorySlotUI
{
    public override void UpdateUISlot(InventorySlot slot)
    {
        AttributeitemData attributeitemData = slot.ItemData as AttributeitemData;
        if (attributeitemData == null)
        {
            //Debug.Log("속성 부여 아이템아님");
            return;
        }
        if (slot.ItemData != null)
        {
            Debug.Log("아이템 데이터 있음");

            itemImage.sprite = slot.ItemData.ItemIcon;
            itemImage.color = Color.white;
            itemNameBG.SetActive(true);
            itemNameText.text = slot.ItemData.ItemName;
        }
        else
        {
            Debug.Log("아이템 데이터 없음");
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
