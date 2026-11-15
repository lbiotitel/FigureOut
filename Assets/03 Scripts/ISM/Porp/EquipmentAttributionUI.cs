using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipmentAttributionUI : StaticInventoryUI
{
    [Header("EquipmentAttributionUI 加己")]
    [SerializeField] private AttributionSlotUI equipmentSlotUI; 
    [SerializeField] private AttributionMaterialSlotUI attributionMaterialSlotUI;

    [SerializeField] private Button EquipmentAttributionButton;
    [SerializeField] private EquipmentAttributionAnimUI equipmentAttributionAnimUI;


    private bool isAttribution;
    public void Init()
    {
        equipmentAttributionAnimUI.HideUI();
        equipmentAttributionAnimUI.OnAnimFinished += AttributionAnimFinished;

        EquipmentAttributionButton.onClick.AddListener(OnAttributionButtonClicked);


    }


    protected override void SetEmptySlot(InventorySlotUI clickedSlotUI)
    {
        Debug.Log("SetEmptySlot");

        if (!mouseObject.InventorySlot.IsEmpty()
            && ((mouseObject.InventorySlot.ItemData.ItemType != EItemType.Equipment)
            && mouseObject.InventorySlot.ItemData.ItemType != EItemType.AttributeItem))
        {
            return;
        }
        var a = clickedSlotUI as AttributionMaterialSlotUI;

        if (a == null)
        {
            Debug.Log("a==null");
            if (mouseObject.InventorySlot.ItemData.ItemType != EItemType.Equipment) return;

        }
        else
        {
            Debug.Log("a!=null");

            if (mouseObject.InventorySlot.ItemData.ItemType != EItemType.AttributeItem) return;

        }

        base.SetEmptySlot(clickedSlotUI);
    }
    private void OnAttributionButtonClicked()
    {
        Debug.Log("加己 何咯 矫档!");
        if (isAttribution)
        {
            Debug.Log("isAttribution false");

            return;
        }
        if (!CanAttribution())
        {
            Debug.Log("CanAttribution false");
            return;
        }



        isAttribution = true;

        equipmentAttributionAnimUI.ShowUI();
        Debug.Log("局聪皋捞记 角青!");
        Managers.Audio.PlaySfx(KeyDefines.SoundKey.AttributionSfx);

        equipmentAttributionAnimUI.PlayAnimation(equipmentSlotUI.InventorySlot.ItemData.ItemIcon);
    }
    public void Attribution()
    {
        if (equipmentSlotUI.InventorySlot.IsEmpty() || attributionMaterialSlotUI.InventorySlot.IsEmpty()) return;
        EquipmentData equipmentData = equipmentSlotUI.InventorySlot.ItemData as EquipmentData;
        AttributeitemData attributeitemData = attributionMaterialSlotUI.InventorySlot.ItemData as AttributeitemData;
        if (equipmentData == null || attributeitemData == null) return;

        SStatContainer sStat = new SStatContainer { statType = attributeitemData.StatType, modifireType = EStatModifireType.Add, value = attributeitemData.Number };
        equipmentData.EquipmentStats.Add(sStat);

        PlayerUnit player = Managers.Game.PlayerUnit;
        if (player == null) return;

        bool isAdded = player.InventoryHolder.InventorySys.AddToInventory(equipmentData, 1);

        if (isAdded)
        {
            attributionMaterialSlotUI.ClearSlot();
            equipmentSlotUI.ClearSlot();
        }
        else
        {
            Transform targetTr = player.transform;
            Vector3 targetPos = targetTr.position
                + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
            targetPos.y = 0f;

            DropItem dropItem = Managers.Object.CreateDropObject(
                equipmentData, 1, targetTr.position, Quaternion.identity);

            dropItem.transform.DOJump(targetPos, 1f, 1, 0.3f);

            attributionMaterialSlotUI.ClearSlot();
            equipmentSlotUI.ClearSlot();
        }

    }
    private bool CanAttribution()
    {
        if (equipmentSlotUI.InventorySlot.IsEmpty() || attributionMaterialSlotUI.InventorySlot.IsEmpty()) return false;

        return true;
    }
    private void AttributionAnimFinished()
    {
        Attribution();
        isAttribution = false;
        equipmentAttributionAnimUI.HideUI();
    }



    public override void ShowUI() => gameObject.SetActive(true);
    public override void HideUI() => gameObject.SetActive(false);

}
