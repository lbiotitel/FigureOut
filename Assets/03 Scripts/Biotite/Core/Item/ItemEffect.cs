using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//아이템 사용 효과를 정의합니다.

[Serializable]
public abstract class ItemEffect 
{
    public abstract void ExcuteEffect(Unit targetUnit);
    public abstract string BuildDescription();
}
