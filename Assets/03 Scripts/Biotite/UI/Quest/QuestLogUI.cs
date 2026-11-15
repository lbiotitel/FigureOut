using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class QuestLogUI : MonoBehaviour
{
    [SerializeField] private TMP_Text questNameText;
    private QuestBase quest;
    private Button button;

    public event Action<QuestBase> OnClickedQuest;
    public EQuestState QuestState => quest.QuestState;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(HandleButtonClicekdEvent);
    }

    public void Init(QuestBase quest)
    {
        this.quest = quest;
        questNameText.text = quest.QuestData.QuestName;
        SetTextColor(quest.QuestState);
    }

    private void SetTextColor(EQuestState state)
    {
        switch (state)
        {
            case EQuestState.BeforeStart:
                questNameText.color = Color.white;
                break;
            case EQuestState.InProgress:
                questNameText.color = Color.yellow;
                break;
            case EQuestState.Completed:
                questNameText.color = Color.green;
                break;
            case EQuestState.RewardClaimed:
                questNameText.color = Color.gray;
                break;
        }
    }

    private void HandleButtonClicekdEvent()
    { 
        OnClickedQuest?.Invoke(quest);
    }


    public void ShowUi() => gameObject.SetActive(true);
    public void HideUi() => gameObject.SetActive(false);

}
