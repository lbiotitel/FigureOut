using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


//퀘스트의 자세한 정보를 그리는 UI 입니다.
public class QuestDetailUI : MonoBehaviour
{
    [Header("Detail UI")]
    [SerializeField] private TMP_Text questNameText;
    [SerializeField] private TMP_Text questDescription;

    [Header("Quest Button")]
    [SerializeField] private TMP_Text questButtonText;
    [SerializeField] private Button questButton;

    [Header("Quest Objective UI")]
    [SerializeField] private GameObject questObjectiveContent;
    [SerializeField] private TMP_Text questObjectiveText;

    [Header("Reward UI")]
    [SerializeField] private GameObject rewardNodePrefab;
    [SerializeField] private GameObject rootContent;
    [SerializeField] private GameObject rewardContent;
    [SerializeField] private TMP_Text rewardDollarText;

    private List<GameObject> rewardNodeUIs = new List<GameObject>();


    public Action OnQuestButtonClicked;

    private void Awake()
    {
        HideDetailUI();
        InitRewardNodeUIs(6);
        questButton.onClick.AddListener(() => OnQuestButtonClicked?.Invoke());
    }

    public void ShowUI(QuestBase quest)
    {
        HideDetailUI();
        HideAllRewardNodeUI();

        if (quest.QuestData != null)
        {
            questNameText.text = quest.QuestData.QuestName;
            questDescription.text = quest.QuestData.QuestDescription;

            DrawQuestButton(quest);
            DrawQuestObjective(quest);
            DrawRewardUI(quest);
        }
    }

    private void InitRewardNodeUIs(int count)
    {
        for (int i = 0; i < count; i++)
        {
            GameObject rewardNode = Instantiate(rewardNodePrefab, rewardContent.transform);
            rewardNodeUIs.Add(rewardNode);
        }
    }

    private void DrawQuestButton(QuestBase quest)
    {
        questButton.gameObject.SetActive(true);

        if (quest.QuestState == EQuestState.BeforeStart)
        {
            questButtonText.text = "퀘스트 시작";
            questButton.enabled = true;
        }
        else if (quest.QuestState == EQuestState.InProgress)
        {
            questButtonText.text = "진행중...";
            questButton.enabled = false;
        }
        else if (quest.QuestState == EQuestState.Completed)
        {
            questButtonText.text = "보상 받기";
            questButton.enabled = true;
        }
        else if (quest.QuestState == EQuestState.RewardClaimed)
        {
            questButtonText.text = "퀘스트 완료";
            questButton.enabled = false;
        }
    }


    private void DrawQuestObjective(QuestBase quest)
    {
        questObjectiveContent.SetActive(true);

        string res = "";
        //퀘스트 종류에 따라 다르게 렌더링
        if (quest is CountedQuest)
        {
            CountedQuest countedQuest = quest as CountedQuest;
            CountedQuestData countedQuestData = countedQuest.CountedQuestData;
            List<QuestObjective> questObjectives = countedQuestData.QuestObjectives;

            for (int i = 0; i < countedQuest.CurrentCount.Length; i++)
            {
                string objectiveID = questObjectives[i].ObjectiveID;

                if (countedQuestData.QuestType == EQuestType.KILL)
                {
                    EnemyData enemyData = null;
                    enemyData = Managers.Data.GetData<EnemyData>(objectiveID);


                    if (enemyData == null)
                    {
                        Logger.LogError($"퀘스트에 등륵한 {objectiveID}는 잘못된 적 ID 입니다");
                        continue;
                    }

                    res += $"{enemyData.unitName} : {countedQuest.CurrentCount[i]} / {questObjectives[i].RequiredAmount}\n";
                }
                else if (countedQuestData.QuestType == EQuestType.RESOURCE)
                {
                    ItemData itemData = Managers.Data.GetData<ItemData>(objectiveID);
                    if (itemData == null)
                    {
                        Logger.LogError($"퀘스트에 등륵한 {objectiveID}는 잘못된 아이템 ID 입니다");
                        continue;
                    }

                    res += $"{itemData.ItemName} : {countedQuest.CurrentCount[i]} / {questObjectives[i].RequiredAmount}\n";
                }


            }

        }

        questObjectiveText.text = res;
    }


    //보상 리워드 그리기
    private void DrawRewardUI(QuestBase quest)
    {
        rootContent.gameObject.SetActive(true);
        rewardDollarText.text = $"{quest.QuestData.DollarReward}$";

        List<QuestItemReward> questItemRewards = quest.QuestData.QuestItemRewards;

        if (questItemRewards == null || questItemRewards.Count == 0) return;

        for (int i = 0; i < questItemRewards.Count; i++)
        {
            ItemData itemData = Managers.Data.GetData<ItemData>(questItemRewards[i].itemID);
            if (itemData == null)
            {
                Logger.LogError($"퀘스트에 등록된 {questItemRewards[i].itemID}는 잘못된 아이템 ID 입니다");
                continue;
            }

            GameObject rewardNode = rewardNodeUIs[i];
            TMP_Text rewardAmountText = rewardNode.GetComponentInChildren<TMP_Text>();
            Image rewardImage = rewardNode.transform.Find("Item Reward Image").GetComponent<Image>();
            rewardImage.sprite = itemData.ItemIcon;
            rewardImage.preserveAspect = true;
            rewardAmountText.text = $"{questItemRewards[i].amount}";
            rewardNode.SetActive(true);
        }
    }


    private void HideAllRewardNodeUI()
    {
        foreach (var node in rewardNodeUIs)
        {
            node.SetActive(false);
        }
    }


    private void HideDetailUI()
    {
        questNameText.text = string.Empty;
        questDescription.text = string.Empty;
        rootContent.gameObject.SetActive(false);
        questObjectiveContent.gameObject.SetActive(false);
        questButton.gameObject.SetActive(false);
    }
}
