
//장비 장착을 담당하는 UI 입니다
public class EquipmentUI : StaticInventoryUI
{

    protected override void Start()
    {
        base.Start();
        Managers.Event.AddEvent<MouseObjectUpdatedEvent>(HandleSlotClicked);
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<MouseObjectUpdatedEvent>(HandleSlotClicked);
    }


    //장비를 클릭해서 마우스가 들고있을 때 장비 부분을 하이라이트
    private void HandleSlotClicked(MouseObjectUpdatedEvent data)
    {
        HideAllHightLight();
        EquipmentData equipmentData = data.itemData as EquipmentData;
        if (equipmentData == null) return;

        ShowHightLight(equipmentData);
    }


    private void ShowHightLight(EquipmentData equipmentData)
    {
        foreach (var slot in slotDict)
        {
            EquipmentSlotUI equipmentSlotUI = slot.Key as EquipmentSlotUI;
            if (equipmentSlotUI.EquipmentSlotTye == equipmentData.EquipmentType)
            {
                equipmentSlotUI.ShowHightLight();
            }
        }
    }

    //모든 하이라이트 off
    private void HideAllHightLight()
    {
        foreach (var slot in slotDict)
        {
            EquipmentSlotUI equipmentSlotUI = slot.Key as EquipmentSlotUI;
            equipmentSlotUI.HideHighLight();
        }
    }



    public override void SlotClicked(InventorySlotUI clickedSlotUI)
    {
        //마우스에 장비가 없고 현재 슬롯에는 장비가 있을 때
        if (!clickedSlotUI.InventorySlot.IsEmpty() && mouseObject.InventorySlot.ItemData == null)
        {

            mouseObject.UpdateSlot(clickedSlotUI.InventorySlot);
            clickedSlotUI.ClearSlot();
            Managers.Event.NotifyEvent<UnEquippedEquipmentEvent>(new UnEquippedEquipmentEvent { itemData = mouseObject.InventorySlot.ItemData });
            return;
        }


        //만약 움직일 시에는 장착하지 못하게 막자
       

        //마우스에 장비가 있을 때
        if (!mouseObject.InventorySlot.IsEmpty())
        {
            EquipmentSlotUI equipmentSlotUI = clickedSlotUI as EquipmentSlotUI;
            if (equipmentSlotUI == null) return;

            EquipmentData equipmentData = mouseObject.InventorySlot.ItemData as EquipmentData;
            if (equipmentData == null)
            {
                HideAllHightLight();
                return;
            }
            //장착할 수 있는 슬롯일 때
            if (equipmentSlotUI.EquipmentSlotTye == equipmentData.EquipmentType)
            {
                //슬롯이 비어있으면 그냥 장착
                if (equipmentSlotUI.InventorySlot.IsEmpty())
                {
                    clickedSlotUI.InventorySlot.AssignItem(mouseObject.InventorySlot);
                    clickedSlotUI.UpdateUISlot();
                    mouseObject.ClearSlot();
                }
                else
                {
                    //슬롯이 비지 않았을 때 스왑
                    Managers.Event.NotifyEvent<UnEquippedEquipmentEvent>(new UnEquippedEquipmentEvent { itemData = mouseObject.InventorySlot.ItemData });
                    SwapSlots(clickedSlotUI);
                }
                Managers.Event.NotifyEvent<EquippedEquipmentEvent>(new EquippedEquipmentEvent { itemData = clickedSlotUI.InventorySlot.ItemData });
            }
            else
            {
                ShowHightLight(equipmentData);
            }
        }
    }



}
