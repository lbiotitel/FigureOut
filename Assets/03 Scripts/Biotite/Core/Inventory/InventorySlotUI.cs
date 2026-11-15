using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlotUI : MonoBehaviour
{

    [SerializeField] protected Image itemImage;
    [SerializeField] protected Image itemBG;
    [SerializeField] protected TMP_Text itemAmountText;
    [SerializeField] protected GameObject itemNameBG;
    [SerializeField] protected TMP_Text itemNameText;
    protected InventorySlot inventorySlot;
    
    public InventoryUIBase ParentInventoryUI { get; private set; }

    public InventorySlot InventorySlot => inventorySlot;    

    private Button button;

    private void Awake()
    {
        ClearSlot();

        itemImage.preserveAspect = true;
        button = GetComponent<Button>();
        button?.onClick.AddListener(OnUISlotClicked);
    }

    public void Init(InventorySlot slot)
    {
        inventorySlot = slot;
        UpdateUISlot(slot);
    }

    public void SetParentUI(InventoryUIBase parentUI)
        => ParentInventoryUI = parentUI; 

    public virtual void UpdateUISlot(InventorySlot slot)
    {
        if (slot.ItemData != null)
        {
            itemImage.sprite = slot.ItemData.ItemIcon;
            itemImage.color = Color.white;
            itemNameBG.SetActive(true);
            itemNameText.text = slot.ItemData.ItemName;
            SetBGColor(slot.ItemData.Rarity);
        }
        else
        {
            ClearSlot();
        }

        if (slot.StackSize > 1) itemAmountText.text = slot.StackSize.ToString();
        else itemAmountText.text = string.Empty;
    }

    private void SetBGColor(ERarity rarity)
    {
        switch (rarity)
        {
            case ERarity.Normal:
                itemBG.color = new Color(42f/255f,47/255f,58/255f);
                break;
            case ERarity.Rare:
                itemBG.color = Color.cyan;
                break;
            case ERarity.Epic:
                itemBG.color = Color.magenta; 
                break;
            case ERarity.Legend:
                itemBG.color = Color.red; 
                break;
        }
    }

    public void UpdateUISlot()
    { 
        if(inventorySlot != null)UpdateUISlot(inventorySlot);
    }

    public virtual void ClearSlot()
    {
        inventorySlot?.ClearSlot();
        itemImage.color = Color.clear;
        itemAmountText.text = string.Empty;
        itemNameBG.SetActive(false);
        itemNameText.text = string.Empty;
        SetBGColor(ERarity.Normal);
    }

    protected virtual void OnUISlotClicked()
    {
        Managers.Event.NotifyEvent<SlotClickedEvent>(new SlotClickedEvent { itemData = InventorySlot.ItemData });
        if (inventorySlot.ItemData != null) Managers.Audio.PlaySfx(KeyDefines.SoundKey.EquipSfx);
        ParentInventoryUI?.SlotClicked(this);
    }
}
