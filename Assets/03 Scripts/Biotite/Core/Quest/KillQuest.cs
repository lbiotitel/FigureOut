using System.Collections.Generic;

public class KillQuest : CountedQuest
{
    public override void Init(QuestDataBase questData)
    {
        base.Init(questData);
    }

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        Managers.Event.AddEvent<EnemyDiedEvent>(HandleEnemyDiedEvent);
    }

    //어떤 적이 죽었을 때 이벤트가 발생, 그 이벤트를 받아서 퀘스트 진행 상황을 업데이트
    private void HandleEnemyDiedEvent(EnemyDiedEvent enemyDiedEvent)
    {
        string enemyID = enemyDiedEvent.enemy.enemyData.unitID;

        Logger.LogWarning($"{enemyID} 가 죽음");

        List<QuestObjective> objectives = countedData.QuestObjectives;
        for (int i = 0; i < objectives.Count; i++)
        {
            if (enemyID == objectives[i].ObjectiveID)
            {
                currentCount[i]++;
                UpdateQuestState(EQuestState.InProgress); //ui 변경을 위해 inProgress로 업데이트
            }
        }

        CheckCompletion();
    }

    public override void RewardClaimed()
    {
        base.RewardClaimed();
        Managers.Event.RemoveEvent<EnemyDiedEvent>(HandleEnemyDiedEvent);
    }

}
