using System.Collections.Generic;
using UnityEngine;
using MackySoft.SerializeReferenceExtensions.Editor;

[CreateAssetMenu(fileName = "ConsumableData", menuName = "Scriptable Objects/ConsumableData")]
public class ConsumableData : ItemData
{
    [Header("家葛前 酒捞袍 加己")]
    [SerializeField] private float coolTime;

    [SerializeReference,SubclassSelector] private List<ItemEffect> effects;

    public float CoolTime => coolTime;
    public List<ItemEffect> Effects => effects;


    public string BuildDescription()
    { 
        string res = "\n";

        for (int i = 0; i < effects.Count; i++)
        {
            res += effects[i].BuildDescription() + "\n";
        }

        return res;
    }
}
