using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributionTable : MonoBehaviour, IInteractable
{
    [SerializeField] private AttributionTableUI attributionTableUI;

    private void OnTriggerEnter(Collider other)
    {
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "속성 부여 테이블 사용하기"
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
        attributionTableUI.ShowUI();
        Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });
    }



    private void OnTriggerExit(Collider other)
    {
        attributionTableUI.HideUI();
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }


}
