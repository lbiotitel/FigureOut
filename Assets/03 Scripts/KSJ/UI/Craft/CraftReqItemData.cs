using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftReqItemData : ItemSlotData // , IPointerUpHandler
{
    public string craftItemID;
    public int currAmountValue;
    public int reqAmountValue;
    [SerializeField] private TextMeshProUGUI reqAmountText;

    public override void CopyItemData(ItemData itemData)
    {
        base.CopyItemData(itemData);
        craftItemID = itemData.ItemID;
    }

    public void GetIngredientAmount(int curr, int target)
    {
        currAmountValue = curr;
        reqAmountValue = target;

        reqAmountText.text = ($"{currAmountValue} / {reqAmountValue}");
    }

    public void InitializeInventory()
    {


    }

    public TextMeshProUGUI ReqAmountText
    {
        get { return reqAmountText; }
        set { reqAmountText = value; }
    }
}
