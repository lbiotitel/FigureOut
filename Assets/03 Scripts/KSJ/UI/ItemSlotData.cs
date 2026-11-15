using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

/// <summary>
/// 아이템 공통 클래스
/// </summary>

public class ItemSlotData : MonoBehaviour
{
    [Header("아이템 SO")]
    [SerializeField] protected ItemData refItemData;

    [Header("아이템 정보")]
    [SerializeField] protected Sprite itemIcon;
    [SerializeField] protected string itemName = "";

    [SerializeField] protected int grade = 1;
    [SerializeField] protected EItemType itemType;

    [Header("UI 오브젝트")]
    [SerializeField] protected Image itemImage;
    [SerializeField] protected TextMeshProUGUI itemNameTMP;


    public virtual void CopyItemData(ItemData itemData)
    {
        refItemData = itemData;

        itemName = itemData.ItemName;
        //grade = itemData.Rarity;
        itemType = itemData.ItemType;

        if (itemData.ItemIcon != null)
            itemIcon = itemData.ItemIcon;

        itemImage.sprite = itemIcon;
        itemNameTMP.text = itemName;
    }


    public ItemData RefItemData
    {
        get { return refItemData; }
    }

    public string ItemName
    {
        get { return itemName; }
    }


    public int Grade
    {
        get { return grade; }
    }
}
