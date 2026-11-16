using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;


// 드롭에 관한 클래스
public class Dropper 
{
    public readonly float DropRange = 1.5f;

    private string dropTableID;
    private int totalWeight;    

    private UnitDropTableData dropTableData;
    List<DropTableInfo> dropTableInfos;
    List<DropTableInfo> essentialDroptableInfos;

    public void Init(string dropTableID)
    {
        this.dropTableID = dropTableID;
        CalcDropWeight();
    }

    private void CalcDropWeight()
    {
        dropTableData = Managers.Data.GetData<UnitDropTableData>(dropTableID);

        dropTableInfos = dropTableData.DropTableInfos;
        essentialDroptableInfos = dropTableData.EssentialDropTableInfos;

        foreach (var drop in dropTableInfos)
        {
            totalWeight += drop.DropWeight;   
        }
    }

    public void Drop(Vector3 pos)
    {
        Managers.Game.AddDollar(dropTableData.DollarAmount);

        if (essentialDroptableInfos != null)
        {
            for (int i = 0; i < essentialDroptableInfos.Count; i++)
            {
                ItemData itemData = Managers.Data.GetData<ItemData>(essentialDroptableInfos[i].ItemId);
                CreateDropObject(itemData, pos);
            }
        }

        if (dropTableData != null)
        {
            for (int i = 0; i < dropTableData.DropAmount; i++)
            {
                ItemData itemData = Managers.Data.GetData<ItemData>(GetRandomItemID());
                CreateDropObject(itemData, pos);
            }
        }
    }

    private void CreateDropObject(ItemData itemData, Vector3 pos)
    {
        if (itemData == null) return;

        DropItem dropItem = Managers.Object.CreateDropObject(itemData, 1, pos, Quaternion.identity);

        float randX = Random.Range(-DropRange, DropRange);
        float randZ = Random.Range(-DropRange, DropRange);
        pos = new Vector3(pos.x, 0, pos.z);
        dropItem.transform.DOJump(pos + new Vector3(randX, 0f, randZ), 3, 1, 0.4f);
    }


    private string GetRandomItemID()
    { 
        int randomWeight = Random.Range(0, totalWeight);

        for(int i = 0; i < dropTableInfos.Count; i++)
        {
            if(randomWeight < dropTableInfos[i].DropWeight)
            {
                return dropTableInfos[i].ItemId;
            }
            else
            {
                randomWeight -= dropTableInfos[i].DropWeight;
            }
        }

        return string.Empty;
    }

}
