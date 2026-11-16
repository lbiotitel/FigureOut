using UnityEngine;

public class Printer : MonoBehaviour,IInteractable
{
    [SerializeField] private PrinterUI printerUI;


    private void OnTriggerEnter(Collider other)
    {
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = "프린터 사용하기"
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
        printerUI.ShowUI();
        Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });
    }



    private void OnTriggerExit(Collider other)
    {
        printerUI.HideUI();
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }


}
