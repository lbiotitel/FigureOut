using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitPoint : MonoBehaviour, IDmgAble
{
    public bool isCriticalPart = false;

    // 부모가 누구인지 몰라도 됨. 그냥 "이 함수를 실행해줘"라는 신호기 역할
    private Action<DmgContext, bool> OnHitAction;

    public void Setup(Action<DmgContext, bool> hitCallback)
    {
        OnHitAction = hitCallback;
    }

    public void TakeDamage(DmgContext context)
    {
        //신호가 등록되어 있다면 실행
        OnHitAction?.Invoke(context, isCriticalPart);
    }
}
