using DG.Tweening;
using System.Reflection;
using UnityEngine;



public class PrinterUI : MonoBehaviour
{
    [SerializeField] private Transform contentTr;
    [SerializeField] private Transform animTargetTr;
    [SerializeField] private float showAnimDuration = 0.5f;

    private EquipmentEevolutionUI equipmentEevolutionUI;
    private Vector3 originPos;


    private void Start()
    {
        Init();
    }

    public void Init()
    {
        originPos = contentTr.transform.position;
        equipmentEevolutionUI = GetComponentInChildren<EquipmentEevolutionUI>(true);
        equipmentEevolutionUI.Init();
        HideUI();
    }

    public void ShowUI()
    {
        if(contentTr.gameObject.activeSelf) return;

        contentTr.transform.position = originPos;
        contentTr.gameObject.SetActive(true);
        contentTr.DOMove(animTargetTr.position, showAnimDuration);
        equipmentEevolutionUI.ShowUI();
    }

    public void HideUI()
    {
        equipmentEevolutionUI.HideUI();
        contentTr.gameObject.SetActive(false);
    }


    public void EvolutionAnimFinish()
    {
        equipmentEevolutionUI.Evolution();
    }
}
