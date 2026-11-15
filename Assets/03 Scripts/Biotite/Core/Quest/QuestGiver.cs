using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class QuestGiver : MonoBehaviour,IInteractable
{
    [SerializeField] private List<string> questIDs;
    [SerializeField] private string interactText = "퀘스트 수락";

    [Header("퀘스트 UI")]
    [TextArea] [SerializeField] private string questDialog;
    [SerializeField] private GameObject objDialogUI;
    [SerializeField] private TextMeshProUGUI objTextUI;
    [SerializeField] private float duration = 0.8f;
    [SerializeField] private GameObject objNotice;

    private bool giveQuest = false;

    private void OnTriggerEnter(Collider other)
    {
        if (giveQuest) return;
        Utils.CheckPlayerInteract(other, this, (playerUnit) => {
            Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                new PlayerTriggerEnter
                {
                    position = transform.position,
                    interactableObject = this,
                    interactText = interactText
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
        if (giveQuest) return;

        giveQuest = true;
  
        for (int i = 0; i < questIDs.Count; i++)
        {
            Managers.Quest.RegistQuest(questIDs[i]);
        }

        objDialogUI.gameObject.SetActive(true);
        objTextUI.text = questDialog;
        objTextUI.maxVisibleCharacters = 0;

        DOTween.To(() => objTextUI.maxVisibleCharacters,
                        x => objTextUI.maxVisibleCharacters = x,
                        questDialog.Length,
                        duration).SetEase(Ease.Linear);

        objNotice.SetActive(false);
    }


    private void OnTriggerExit(Collider other)
    {
        objDialogUI.gameObject.SetActive(false);
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
    }

}
