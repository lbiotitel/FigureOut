using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class QuestUI : MonoBehaviour
{
    [SerializeField] private GameObject rootParent;
    [SerializeField] private GameObject scrollContentParent;
    private GameObject questLogUIPrefab;

    [SerializeField] private QuestDetailUI questDetailUI;
    private QuestBase selectedQuest = null;

    [SerializeField] private Button allFilterButton;
    [SerializeField] private Button processingFilteredButton;
    [SerializeField] private Button completedFilteredButton;
    
    private Dictionary<string, QuestLogUI> questLogDict;


    private void Start()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
        questDetailUI.OnQuestButtonClicked += HandleQuestButtonClicked;

        allFilterButton.onClick.AddListener(() => FilterQuests(EQuestState.None)); 
        processingFilteredButton.onClick.AddListener(() => FilterQuests(EQuestState.InProgress));
        completedFilteredButton.onClick.AddListener(() => FilterQuests(EQuestState.RewardClaimed));
    }

    private void OnDestroy()
    {
        if (Managers.Quest != null)
        {
            Managers.Quest.OnQuestRegisted -= HandleQuestRegistedEvent;
            Managers.Quest.OnQuestStateChanged -= HandleQuestStateChangedEvent;
        }
    }

    private void Update()
    {
        ToggleQuestUI();
    }

    private void ToggleQuestUI()
    {
        if (Managers.Input.QuestButton)
        {
            if (rootParent.activeSelf)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
        }
    }

    
    //퀘스트 필터링
    private void FilterQuests(EQuestState filterState)
    {
        foreach (var questLog in questLogDict)
        {
            if (filterState == EQuestState.None)
            {
                questLog.Value.ShowUi();
                continue;
            }

            if (questLog.Value.QuestState == filterState)
            {
                questLog.Value.ShowUi();
            }
            else
            { 
                questLog.Value.HideUi();
            }
        }
    }



    private void HandlePlayerSpawnEvent(PlayerSpawnEvent playerSpawnEvent)
    { 
        questLogUIPrefab = Managers.Data.GetData<GameObject>("Quest Log UI Prefab");
        questLogDict = new Dictionary<string, QuestLogUI>();

        QuestManager questManager = Managers.Quest;

        questManager.OnQuestRegisted += HandleQuestRegistedEvent;
        questManager.OnQuestStateChanged += HandleQuestStateChangedEvent;

        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
    }


    //퀘스트 스크롤에 퀘스트 로그를 만들어서 등록
    private void HandleQuestRegistedEvent(QuestBase quest)
    {
        QuestLogUI questLogUI = Instantiate(questLogUIPrefab, scrollContentParent.transform).GetComponent<QuestLogUI>();
        questLogDict.Add(quest.QuestData.QuestID, questLogUI);
        questLogUI.OnClickedQuest += HandleQuestLogUIClicked;
        questLogUI.Init(quest);
    }

    
    //특정 상태의 퀘스트가 업데이트 될 때 퀘스트 로그 UI도 업데이트
    private void HandleQuestStateChangedEvent(QuestBase quest)
    {
        if (questLogDict.TryGetValue(quest.QuestData.QuestID, out QuestLogUI questLogUI))
        { 
            questLogUI.Init(quest);
            questDetailUI.ShowUI(quest);
        }
    }



    //현재 클릭된 log ui의 자세한 내용을 보여주고 현재 선택된 퀘스트를 업데이트
    private void HandleQuestLogUIClicked(QuestBase quest)
    { 
        selectedQuest = quest;
        questDetailUI.ShowUI(selectedQuest);
    }


    private void HandleQuestButtonClicked()
    {
        if (selectedQuest.QuestState == EQuestState.BeforeStart)
        {
            Managers.Quest.AcceptQuest(selectedQuest.QuestData.QuestID);
        }
        else if (selectedQuest.QuestState == EQuestState.InProgress)
        {

        }
        else if (selectedQuest.QuestState == EQuestState.Completed)
        {
            Managers.Quest.CompleteQuest(selectedQuest.QuestData.QuestID);
        }
    }


    public void ShowUI() => rootParent.SetActive(true);
    public void HideUI() => rootParent.SetActive(false);
}
