using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 상점 UI 클래스
/// </summary>

public class ShopSlotData : ItemSlotData, IPointerUpHandler
{
    [Header("아이템 정보")]
    [TextArea(order = 60)]  // 상세 설명
    [SerializeField] private string itemDescription = "";
    
    [SerializeField] private int itemPrice = 0; // 상점 가격

    [Header("UI")]
    [SerializeField] protected TextMeshProUGUI itemDescTMP;
    [SerializeField] protected TextMeshProUGUI itemPriceTMP;

    [Header("경고 UI")]
    [SerializeField] private GameObject warningPrefab;

    [Header("인벤토리")]
    public PlayerUnit player;
    [SerializeField] protected InventorySystem inventorySystem;


    public override void CopyItemData(ItemData itemData)
    {
        base.CopyItemData(itemData);

        itemDescription = itemData.ItemDescription;
        itemPrice = itemData.Price;
        itemDescTMP.text = itemDescription;
        itemPriceTMP.text = itemPrice.ToString();
    }

    public void InitializeInventory()
    {
        inventorySystem = player.InventoryHolder.InventorySys;
    }

    public void OnPointerUp(PointerEventData eventdata)
    {
        BuyItem();
    }

    private void BuyItem()
    {
        // 재화가 충분한지 검사
        if(Managers.Game.Dollar >= ItemPrice)
        {
            //(Managers.Game.Dollar)
            // 달러 차감
            int spendAmount = ItemPrice;
            Managers.Game.SpendDollar(spendAmount);

            // 아이템 추가
            inventorySystem.AddToInventory(refItemData, 1);
        }

        else
        {
            Animator anim = warningPrefab.GetComponent<Animator>();
            anim.enabled = true;
            anim.Play("ui_shop_warning", -1, 0f);            
        }
    }


    public string ItemDescription
    {
        get { return itemDescription; }
    }

    public int ItemPrice
    {
        get { return itemPrice; }
    }

}
