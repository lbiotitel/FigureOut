using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public interface IEventData { }


public struct PlayerSpawnEvent : IEventData 
{
    public PlayerUnit player;
}

public struct MouseObjectUpdatedEvent : IEventData
{
    public ItemData itemData;
}

public struct SlotClickedEvent : IEventData
{
    public ItemData itemData;
}

public struct UnEquippedEquipmentEvent : IEventData 
{
    public ItemData itemData;
}

public struct EquippedEquipmentEvent : IEventData
{
    public ItemData itemData;
}


//인벤토리 업데이트 이벤트, 아이템이 추가되거나 제거될 때 발생
public struct InventoryDataChangedEvent : IEventData
{
    public InventorySlot inventorySlot;
}


public struct PlayerTriggerEnter : IEventData
{
    public Vector3 position;
    public IInteractable interactableObject;
    public string interactText;
}

public struct PlayerTriggerExit : IEventData { }


public struct InteractEvent : IEventData
{
    public Vector3 position;
    public IInteractable interactableObject;
    public string interactText;
}

public struct InventorySlotDataChanged : IEventData
{
    public List<InventorySlot> inventorySlots;
}

public struct EnemyDiedEvent : IEventData
{
    public Enemy enemy;
}

public struct DurabilityChangedEvnet : IEventData
{
    public EEquipmentType equipmentType;
    public float currentDurability;
    public float maxDurability;
}

public struct QuestStateChangedEvent : IEventData
{
    public string questID;
}

public struct UIChangeEvent : IEventData
{
    public bool isOpen;
    public int uiID;
}

public struct PlayerDeathEvent : IEventData
{
    public string killerName;
}