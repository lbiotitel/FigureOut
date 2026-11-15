using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public enum EQuestState
{ 
    BeforeStart,
    InProgress,
    Completed,
    RewardClaimed,
    None
}


public abstract class QuestBase 
{
    protected EQuestState questState = EQuestState.BeforeStart;
    private QuestDataBase questData;
    
    public event Action<string> OnQuestStateChanged;


    public virtual void Init(QuestDataBase questData)
    { 
        this.questData = questData;
    }


    public virtual void AcceptQuest()
    { 
        UpdateQuestState(EQuestState.InProgress);
    }


    public virtual void RewardClaimed()
    { 
        UpdateQuestState(EQuestState.RewardClaimed);
    }


    protected void UpdateQuestState(EQuestState newState)
    {
        questState = newState;
        OnQuestStateChanged?.Invoke(QuestData.QuestID);
    }



    public EQuestState QuestState => questState;
    public QuestDataBase QuestData => questData;
}
