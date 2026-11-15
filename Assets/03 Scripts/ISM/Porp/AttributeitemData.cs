using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttributeitemData", menuName = "Scriptable Objects/AttributeitemData")]

public class AttributeitemData : ItemData
{
    [Header("속성 부여 아이템")]
    [SerializeField] private EStatType statType;
    [SerializeField] private float number;
    private void Awake()
    {
        maxStackCount = 1;
    }
    public EStatType StatType=>statType;
    public float Number => number;
   
}
