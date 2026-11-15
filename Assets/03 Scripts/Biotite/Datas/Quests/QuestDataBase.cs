using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//퀘스트에 대한 데이터 정의 입니다.
//퀘스트는 2종류로 간단히 가자 특정 몹을 잡는 퀘스트 특정 재료를 모으는 퀘스트

public enum EQuestType { KILL, RESOURCE }

[CreateAssetMenu]
public class QuestDataBase : ScriptableObject
{
    [SerializeField] private string questID;
    [SerializeField] private string questName;
    [TextArea][SerializeField] private string questDescription;

    [SerializeField] private List<string> nextQuestID;
    [SerializeField] protected EQuestType questType;


    [Header("Reward")]
    [SerializeField] private List<QuestItemReward> questRweardItems;
    [SerializeField] private int dollarReward;



    public string QuestID => questID;
    public EQuestType QuestType => questType;
    public string QuestName => questName;
    public string QuestDescription => questDescription; 
    public List<string> NextQuestID => nextQuestID;

    public List<QuestItemReward> QuestItemRewards => questRweardItems;
    public int DollarReward => dollarReward;

}


[System.Serializable]
public struct QuestItemReward
{
    public string itemID;
    public int amount;
}