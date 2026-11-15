using System;
using UnityEngine;

public class ResourceStat : StatBase
{
    private float currentValue;

    private StatBase maxStatValue;

    public event Action<float, float> OnValueChanged;

    public override float Value => currentValue;

    public ResourceStat(StatBase maxStatValue,bool startEmpty = false) : base(maxStatValue.Value)
    {
        this.maxStatValue = maxStatValue;
        currentValue = startEmpty ? 0f : maxStatValue.Value;
    }

    public override void AddModifier(StatModifier modifier)
    {
        float prevValue = Value;

        if (modifier.Type == EStatModifireType.Add)
            currentValue = Mathf.Clamp(currentValue + modifier.Value, 0f, maxStatValue.Value);
        else if (modifier.Type == EStatModifireType.Percent)
            currentValue = Mathf.Clamp(currentValue * (1f + modifier.Value), 0f, maxStatValue.Value);

        NotifyValueChanged(prevValue);
    }

    public override void RemoveAllModifier(object source) { return; }

    public override void RemoveModifier(StatModifier modifier) { return; }

    public override void ConsumeStat(float value)
    {
        float prevValue = currentValue;

        // 1. 값 더하기 (음수가 들어오면 빼기) 및 0 ~ 최대치 사이로 제한 (마이너스 방지)
        currentValue = Mathf.Clamp(currentValue + value, 0f, maxStatValue.Value);

        // 2. 체력 UI가 깎인 걸 알 수 있게 이벤트 호출! (핵심)
        if (prevValue != currentValue)
        {
            NotifyValueChanged(prevValue);
        }
    }

    protected void NotifyValueChanged(float prevValue)
    {
        OnValueChanged?.Invoke(prevValue,Value);
    }
}
