using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreasureChest : MonoBehaviour,IInteractable
{
    [SerializeField] private GameObject chestTop;
    [SerializeField] private string dropTableID;
    private Dropper dropper;
    private bool isOpen = false;

    private void Start()
    {
        dropper = new Dropper();
        Managers.Event.AddEvent<PlayerSpawnEvent>((player) => { dropper.Init(dropTableID); });
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isOpen) return;
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "상자 열기"
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
        if (isOpen) return;
        isOpen = true;
        chestTop.SetActive(false);
        dropper.Drop(transform.position);
    }

    private void OnTriggerExit(Collider other)
    {
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }
}
