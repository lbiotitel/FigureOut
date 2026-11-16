using System.Collections.Generic;
using UnityEngine;

public class ItemStorage : InventoryHolder, IInteractable
{
    
    [SerializeField] private DynamicInventoryUI inventoryUI;


    private void OnTriggerEnter(Collider other)
    {
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "창고 열기"
                });
        });
    }


    private void OnTriggerStay(Collider other)
    {
        if (Managers.Input.InteractButton)
        {
            Utils.CheckPlayerInteract(other, this, Interact);
        }
    }

    public void Interact(PlayerUnit unit)
    {
        Logger.Log("창고 상호 작용");
        inventoryUI.ShowUI();
        Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });
    }


    private void OnTriggerExit(Collider other)
    {
        inventoryUI.HideUI();
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }


}
