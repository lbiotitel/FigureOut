using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Repair : MonoBehaviour, IInteractable
{
    [SerializeField] private RepairUI repairUI;

    private void OnTriggerEnter(Collider other)
    {
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "모루 사용하기"
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
        repairUI.ShowUI();
        Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });
    }

    private void OnTriggerExit(Collider other)
    {
        repairUI.HideUI();
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }
}
