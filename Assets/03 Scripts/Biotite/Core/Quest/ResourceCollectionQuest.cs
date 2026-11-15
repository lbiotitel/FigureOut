using System.Collections.Generic;

public class ResourceCollectionQuest : CountedQuest
{

    public override void Init(QuestDataBase questData)
    {
        base.Init(questData);
    }

    public override void AcceptQuest()
    {
        base.AcceptQuest();
        Managers.Event.AddEvent<InventoryDataChangedEvent>(HandleInventoryDataChanged);
        HandleInventoryDataChanged(new InventoryDataChangedEvent { });
    }

    //인벤토리 업데이트 이벤트를 받아서 퀘스트 진행 상황을 업데이트
    private void HandleInventoryDataChanged(InventoryDataChangedEvent inventoryDataChangedEvent)
    {

        List<QuestObjective> objectives = countedData.QuestObjectives;

        InventorySystem invSys = Managers.Game.PlayerUnit.InventoryHolder.InventorySys;

        for (int i = 0; i < objectives.Count; i++)
        {
            string itemID = objectives[i].ObjectiveID;
            int currentAmount = invSys.CountItem(itemID);

            currentCount[i] = currentAmount;
        }
        UpdateQuestState(EQuestState.InProgress);

        CheckCompletion();
    }

    public override void RewardClaimed()
    {
        base.RewardClaimed();
        Managers.Event.RemoveEvent<InventoryDataChangedEvent>(HandleInventoryDataChanged);
    }

}
