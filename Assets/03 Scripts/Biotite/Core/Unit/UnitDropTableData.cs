using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class UnitDropTableData : ScriptableObject
{
    [Header("보상 달러")]
    [SerializeField] private int dollarAmount;


    [Header("확정 드랍")]
    [SerializeField] private List<DropTableInfo> essentialDropTableInfos;
    
    [Header("드랍 시행 횟수")]
    [SerializeField] private int dropAmount = 1;
    [SerializeField] private List<DropTableInfo> dropTableInfos;

    public List<DropTableInfo> DropTableInfos => dropTableInfos;
    public List<DropTableInfo> EssentialDropTableInfos => essentialDropTableInfos;
    public int DollarAmount => dollarAmount;
    public int DropAmount => dropAmount;    
}

[Serializable]
public struct DropTableInfo
{
    public string ItemId;
    public int DropWeight;
}
