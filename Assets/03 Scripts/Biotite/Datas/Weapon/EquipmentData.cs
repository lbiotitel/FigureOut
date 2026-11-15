using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public struct SStatContainer
{
    public EStatType statType;
    public EStatModifireType modifireType;
    public float value;
}

public class EquipmentData : ItemData
{
    [SerializeField] private EEquipmentType equipmentType;
    [SerializeField] private Vector3 grabPos;
    [SerializeField] private List<SStatContainer> equipmentStats;
    [SerializeField] private string revolutionRecipeID;
    [SerializeField] private float maxDurability;
    private float currentDurability;

    public event Action<float, float> OnDurabilityChanged;

    private void Awake()
    {
        maxStackCount = 1;
        currentDurability = maxDurability;
    }

    public EEquipmentType EquipmentType => equipmentType;
    public Vector3 GrabPos => grabPos;
    public List<SStatContainer> EquipmentStats => equipmentStats;
    public string RevolutionRecipeID => revolutionRecipeID;
    public float MaxDurability => maxDurability;
    public float CurrnetDurability { get => currentDurability; 
        set
        { currentDurability = Mathf.Clamp(value, 0, maxDurability);
            OnDurabilityChanged?.Invoke(currentDurability, maxDurability);} 
    }
}
