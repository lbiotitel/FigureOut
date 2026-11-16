using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;


//진화 애니메이션을 담당하는 클래스 입니다.
public class EvolutionAnimUI : MonoBehaviour
{
    [SerializeField] private Image equipmentImage;

    public event Action OnBlackScreen;
    public event Action OnAnimFinished;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();    
        equipmentImage.preserveAspect = true;
    }

    public void PlayAnimation(Sprite equipmentImage)
    {
        Managers.UI.Blocker.SetActive(true);
        animator.SetTrigger("Evolution");
        this.equipmentImage.sprite = equipmentImage;
    }


    public void ShowUI() => gameObject.SetActive(true);
    public void HideUI() => gameObject.SetActive(false); 

    public void BlackScreenEnter() => OnBlackScreen?.Invoke();
    public void AnimFinished()
    {
        OnAnimFinished?.Invoke();
        Managers.UI.Blocker.SetActive(false);
    }
}
