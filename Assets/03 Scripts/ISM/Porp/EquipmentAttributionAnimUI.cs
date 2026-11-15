using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

public class EquipmentAttributionAnimUI : MonoBehaviour
{

    public event Action OnAnimFinished;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayAnimation(Sprite equipmentImage)
    {
        Managers.UI.Blocker.SetActive(true);
        animator.Play("attriAnimation", 0, 0);
    }


    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false);

    public void AnimFinished()
    {
        Debug.Log("애니메이션 종료됨! 속성 부여실행함!");
        OnAnimFinished?.Invoke();
        Managers.UI.Blocker.SetActive(false);
    }
}
