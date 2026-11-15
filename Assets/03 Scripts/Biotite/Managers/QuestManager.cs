using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour, IManager
{

    //퀘스트 데이터들 전부 캐싱
    private Dictionary<string, QuestDataBase> questDataDict;
    private Dictionary<string, QuestBase> currentQuests;


    public event Action<QuestBase> OnQuestRegisted;
    public event Action<QuestBase> OnQuestStateChanged;


    public void Init()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
    }

    private void HandlePlayerSpawnEvent(PlayerSpawnEvent e)
    {
        LoadQuestData();
        currentQuests = new Dictionary<string, QuestBase>();
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawnEvent);
    }


    //퀘스트 데이터들 미리 캐싱
    private void LoadQuestData()
    {
        questDataDict = new Dictionary<string, QuestDataBase>();
        List<QuestDataBase> questDatas = Managers.Data.GetDatas<QuestDataBase>();
        foreach (var questData in questDatas)
        {
            questDataDict.Add(questData.QuestID, questData);
        }
    }


    //퀘스트 등록
    public void RegistQuest(string questID)
    {
        if (questID == null)
        {
            Logger.LogError($"잘못된 퀘스트 ID 입니다 => {questID}");
            return;
        }

        QuestBase quest = MakeQuest(questID);
        if (quest == null) return;
        
        currentQuests.Add(questID, quest);
        OnQuestRegisted?.Invoke(quest);
    }


    //퀘스트 수락
    public void AcceptQuest(string questID)
    {
        if (currentQuests.TryGetValue(questID, out QuestBase quest))
        {
            quest.AcceptQuest();
            OnQuestStateChanged?.Invoke(quest);
            Managers.Audio.PlaySfx(KeyDefines.SoundKey.QuestAcceptSfx);
        }
    }


    public void CompleteQuest(string questID)
    {
        if (currentQuests.TryGetValue(questID, out QuestBase quest))
        {
            //보상 주기
            GiveReward(quest);

            //연계 퀘스트 다 등록
            if (quest.QuestData.NextQuestID != null)
            {
                foreach (var id in quest.QuestData.NextQuestID)
                {
                    RegistQuest(id);
                }
            }

            OnQuestStateChanged?.Invoke(quest);
            quest.RewardClaimed();
            Managers.Audio.PlaySfx(KeyDefines.SoundKey.QuestCompleteSfx);
        }
    }


    //보상 만들어서 지급
    private static void GiveReward(QuestBase quest)
    {
        Managers.Game.AddDollar(quest.QuestData.DollarReward);

        List<QuestItemReward> itemRewards = quest.QuestData.QuestItemRewards;

        if (itemRewards == null) return;

        foreach (var reward in itemRewards)
        {
            string itemId = reward.itemID;
            int amount = reward.amount;

            ItemData itemData = Managers.Data.GetData<ItemData>(itemId);

            Managers.Game.PlayerUnit.InventoryHolder.InventorySys.AddToInventory(itemData, amount);
        }
    }


    //퀘스트 생성
    private QuestBase MakeQuest(string questID)
    {
        QuestBase res = null;
        if (questDataDict.TryGetValue(questID, out QuestDataBase questData))
        {
            if (questData.QuestType == EQuestType.KILL)
            {
                res = new KillQuest();
            }
            else if (questData.QuestType == EQuestType.RESOURCE)
            {
                res = new ResourceCollectionQuest();
            }

            res.Init(questData);
            res.OnQuestStateChanged += HandleQuestStateChanged;
        }


        return res;
    }


    //UI에 퀘스트 상태 변경 알림
    private void HandleQuestStateChanged(string questID)
    { 
        if (currentQuests.TryGetValue(questID, out QuestBase quest))
        {
            OnQuestStateChanged?.Invoke(quest);
        }
    }



    public void Release()
    {
        questDataDict.Clear();
        currentQuests.Clear();
    }
}
