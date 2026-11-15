using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class StatBase
{
    protected float value;//이 스텟의 기본 값은 얼마인가

    public StatBase(float value)//생성자
    {
        this.value = value;
    }
    public abstract float Value { get; }//값 꺼내는거 (리소스면 현재값 꺼내면 될듯함요)
    public abstract void AddModifier(StatModifier modifier);//이걸로 리소스스탯 현재값 수정하는거 맞죠?
    public abstract void RemoveAllModifier(object source);//필요 없으면 그냥 리턴 해버려도 오케이
    public abstract void RemoveModifier(StatModifier modifier);//필요 없으면 그냥 리턴 해버려도 오케이
    public abstract void ConsumeStat(float value);
}
