using Cysharp.Threading.Tasks.Triggers;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttributionTableUI : MonoBehaviour
{
    [SerializeField] private Transform contentTr;
    [SerializeField] private Transform animTargetTr;
    [SerializeField] private float showAnimDuration = 0.5f;

    private EquipmentAttributionUI equipmentAttributionUI;
    private Vector3 originPos;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        originPos = contentTr.transform.position;
        equipmentAttributionUI = GetComponentInChildren<EquipmentAttributionUI>(true);
        equipmentAttributionUI.Init();
        HideUI();
    }

    public void ShowUI()
    {
        contentTr.transform.position = originPos;
        contentTr.gameObject.SetActive(true);
        contentTr.DOMove(animTargetTr.position, showAnimDuration);
    }

    public void HideUI()
    {
        contentTr.gameObject.SetActive(false);
    }


    public void EvolutionAnimFinish()
    {
        equipmentAttributionUI.Attribution();
    }
}
