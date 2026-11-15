using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class CountedQuestData : QuestDataBase
{

    [SerializeField] private List<QuestObjective> questObjectives;

    public List<QuestObjective> QuestObjectives => questObjectives;

}

[System.Serializable]
public class QuestObjective
{
    [SerializeField] private string objectiveID;
    [SerializeField] private int requiredAmount;

    public string ObjectiveID => objectiveID;
    public int RequiredAmount => requiredAmount;
}
