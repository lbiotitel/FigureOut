using DG.Tweening;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseObject : MonoBehaviour
{
    [SerializeField] private UnityEngine.UI.Image itemImage;
    [SerializeField] private GameObject itemNameBG;
    [SerializeField] private TMP_Text itemAmountText;
    [SerializeField] private TMP_Text itemNameText;

    public InventorySlot InventorySlot { get; private set; }

    private void Awake()
    {
        itemImage.preserveAspect = true;
        InventorySlot = new InventorySlot();
        ClearSlot();
    }

    internal void UpdateSlot(InventorySlot inventorySlot)
    {
        InventorySlot.AssignItem(inventorySlot);
        itemImage.sprite = inventorySlot.ItemData.ItemIcon;
        itemImage.color = Color.white;
        itemNameBG.SetActive(true);
        itemNameText.text = inventorySlot.ItemData.ItemName;
        if(inventorySlot.StackSize > 1)itemAmountText.text = inventorySlot.StackSize.ToString();
        Managers.Event.NotifyEvent<MouseObjectUpdatedEvent>(new MouseObjectUpdatedEvent { itemData = inventorySlot.ItemData });
    }

    private void Update()
    {
        FollowMouse();
    }

    private void FollowMouse()
    {
        if (InventorySlot.ItemData != null)
        {
            transform.position = Input.mousePosition;
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
            {
                DropItem();
                ClearSlot();
            }
        }
    }

    private void DropItem()
    { 
        ItemData itemData = InventorySlot.ItemData;
        int amount = InventorySlot.StackSize;

        //나중에 플레이어 싱글톤으로 교체
        PlayerUnit player = GameObject.FindAnyObjectByType<PlayerUnit>();
        Transform targetTr = player.transform;
        Vector3 targetPos = targetTr.position + 
            new Vector3(UnityEngine.Random.Range(-2,2),0,UnityEngine.Random.Range(-2,2));
        targetPos.y = 0f;

        DropItem dropItem = Managers.Object.CreateDropObject(itemData,amount,
            targetTr.position,Quaternion.identity);

        dropItem.transform.DOJump(targetPos,1,1,0.3f);

        ClearSlot();

        Managers.Event.NotifyEvent<SlotClickedEvent>(new SlotClickedEvent { itemData = null }); //버릴 때 하이라이트 된 부분을 없애기 위해 null로 itemdata 보냄
        Managers.Event.NotifyEvent<InventorySlotDataChanged>(new InventorySlotDataChanged { inventorySlots = null });
    }


    public void ClearSlot()
    {
        InventorySlot.ClearSlot();
        itemImage.color = Color.clear;
        itemAmountText.text = "";
        itemNameText.text = "";
        itemNameBG.SetActive(false);
        Managers.Event.NotifyEvent<MouseObjectUpdatedEvent>(new MouseObjectUpdatedEvent { itemData = null });
    }


    //마우스와 닿는 UI가 있는지 체크
    public bool IsPointerOverUIObject()
    {
        PointerEventData evenDataPos = new PointerEventData(EventSystem.current);
        evenDataPos.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(evenDataPos, results);
        return results.Count > 0;
    }
}
