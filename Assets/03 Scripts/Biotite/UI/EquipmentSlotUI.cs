using UnityEngine;
using UnityEngine.UI;



//장비 슬롯을 정의 합니다.
public class EquipmentSlotUI : InventorySlotUI
{
    [SerializeField] private Outline hightLightOutLine;
    [SerializeField] private GameObject equipmentHolder;
    [field:SerializeField] public EEquipmentType EquipmentSlotTye { get; private set; }


    //슬롯의 UI를 업데이트 합니다.
    public override void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemImage.sprite = slot.ItemData.ItemIcon;
            itemImage.color = Color.white;
            itemNameBG.SetActive(true);
            itemNameText.text = slot.ItemData.ItemName;
            equipmentHolder.SetActive(false);
        }
        else
        {
            ClearSlot();
        }
    }


    //슬롯의 UI를 초기화 합니다.
    public override void ClearSlot()
    {
        inventorySlot?.ClearSlot();
        itemImage.color = Color.clear;
        itemNameBG.SetActive(false);
        itemNameText.text = string.Empty;
        equipmentHolder.SetActive(true);
    }


    public void ShowHightLight() => hightLightOutLine.enabled = true;
    public void HideHighLight() => hightLightOutLine.enabled = false;
}
