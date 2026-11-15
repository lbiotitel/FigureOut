using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//화면 선택창의 클래스 버튼
public class ClassButtonUI : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private EClassType classType;
    [SerializeField] private float animDuration;
    [SerializeField] private float animScale;
    private Button button;
    private bool isAnim = false;

    public event Action<EClassType> OnClicked;


    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(() => { OnClicked?.Invoke(classType); });
    }


    //마우스가 들어왔을 때 하이라이트
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (isAnim) return;
        isAnim = true;
        transform.DOPunchScale(Vector3.one * animScale, animDuration).OnComplete(() => { 
            isAnim  = false;
        });
    }
}
