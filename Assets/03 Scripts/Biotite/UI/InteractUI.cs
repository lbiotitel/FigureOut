using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InteractUI : MonoBehaviour
{
    [SerializeField] private GameObject content;
    [SerializeField] private TMP_Text interactText;
    private Camera cam;

    private void Start()
    {
        cam = Camera.main;
        Managers.Event.AddEvent<PlayerTriggerEnter>(Show);
        Managers.Event.AddEvent<PlayerTriggerExit>((data) => { Hide(); });
        Managers.Event.AddEvent<InteractEvent>((data) => { Hide(); });
        Hide();
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<PlayerTriggerEnter>(Show);
        Managers.Event.RemoveEvent<PlayerTriggerExit>((data) => { Hide(); });
        Managers.Event.RemoveEvent<InteractEvent>((data) => { Hide(); });
    }

    public void Show(PlayerTriggerEnter triggerEnter)
    {
        interactText.text = triggerEnter.interactText;
        transform.position = new Vector3(triggerEnter.position.x, triggerEnter.position.y + 3f, triggerEnter.position.z);
        content.SetActive(true);
    }


    public void Hide()
    { 
        content.SetActive(false);
    }
}
