using UnityEngine;

public class UIEventTrigger : MonoBehaviour
{
    private void OnEnable()
    {
        if(Managers.Event == null) return;  

        Managers.Event.NotifyEvent<UIChangeEvent>(new UIChangeEvent
        {
            isOpen = true,
            uiID = this.gameObject.GetInstanceID()
        });
    }

    private void OnDisable()
    {
        if(Managers.Event == null) return;

        Managers.Event.NotifyEvent<UIChangeEvent>(new UIChangeEvent
        {
            isOpen = false,
            uiID = this.gameObject.GetInstanceID()
        });
    }
}

