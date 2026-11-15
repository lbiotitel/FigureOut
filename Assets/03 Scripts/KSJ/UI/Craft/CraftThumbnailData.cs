using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class CraftThumbnailData : ItemSlotData, IPointerUpHandler
{
    [Header("표시 아이템")]
    [SerializeField] private ItemData itemData;

    [Header("제작 UI 매니저")]
    public CraftManager craftManager;

    public override void CopyItemData(ItemData itemData)
    {
        base.CopyItemData(itemData);
        this.itemData = itemData;
    }

    public void OnPointerUp(PointerEventData eventdata)
    {
        craftManager.targetItem = itemData;
        craftManager.SetUIInfo(itemData);
    }
}
