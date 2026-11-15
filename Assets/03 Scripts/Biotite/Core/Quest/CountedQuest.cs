using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//재료모으기나 사냥퀘스트같은 카운트 퀘스트의 베이스 클래스
public class CountedQuest : QuestBase
{
    protected int[] currentCount;
    public int[] CurrentCount => currentCount;

    protected CountedQuestData countedData;
    public CountedQuestData CountedQuestData => countedData;

    public override void Init(QuestDataBase questData)
    {
        countedData = questData as CountedQuestData;
        base.Init(questData);
        currentCount = new int[countedData.QuestObjectives.Count];
    }

    protected virtual void CheckCompletion()
    {
        List<QuestObjective> objectives = countedData.QuestObjectives;

        for (int i = 0; i < objectives.Count; i++)
        {
            if (currentCount[i] < objectives[i].RequiredAmount)
            {
                return;
            }
        }

        //완료
        UpdateQuestState(EQuestState.Completed);
    }


    public override void RewardClaimed()
    {
        base.RewardClaimed();
    }
}
