using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;

[Serializable]
public struct StatNode
{
    public string nodeName;
    public EStatType statType;
    public TMP_Text valueText;
}

public class PlayerStatUI : MonoBehaviour
{
    [SerializeField] private Transform playerStatTr;
    [SerializeField] private Transform animTargetTr;
    [SerializeField] private float showAnimDuration = 0.5f;
    [SerializeField] private List<StatNode> statNodes = new List<StatNode>();

    private Vector3 originPos;

    // Start is called before the first frame update
    void Start()
    {
        originPos = playerStatTr.position;
        HideUI();
        Managers.Event.AddEvent<EquippedEquipmentEvent>(e => OnEquipmentChanged());
        Managers.Event.AddEvent<UnEquippedEquipmentEvent>(e => OnEquipmentChanged());
    }

    private void OnDestroy()
    {
        Managers.Event.RemoveEvent<EquippedEquipmentEvent>(e => OnEquipmentChanged());
        Managers.Event.RemoveEvent<UnEquippedEquipmentEvent>(e => OnEquipmentChanged());
    }

    // Update is called once per frame
    void Update()
    {
        if (Managers.Input.PlayerStatButton)
        {
            if (playerStatTr.gameObject.activeSelf)
            {
                HideUI();
            }
            else
            {
                ShowUI();
            }
        }
    }

    private void OnEquipmentChanged()
    {
        if (playerStatTr.gameObject.activeSelf)
        {
            DelayedUpdateStats().Forget();
        }
    }

    private async UniTaskVoid DelayedUpdateStats()
    {
        await UniTask.Yield();

        if (playerStatTr.gameObject.activeSelf)
        {
            UpdateAllStats();
        }
    }

    private void UpdateAllStats()
    {
        PlayerUnit playerUnit = Managers.Game.PlayerUnit;

        if (playerUnit == null || playerUnit.PlayerStat == null) return;

        foreach(var node in statNodes)
        {
            float statValue = GetFinalStatValue(playerUnit, node.statType);

            if (node.valueText != null)
            {
                node.valueText.text = $"{statValue}";
            }
        }
    }

    private float GetFinalStatValue(PlayerUnit playerUnit, EStatType statType)
    {
        if (playerUnit.PlayerStat.GetStatBase(statType) == null) return 0f;

        return playerUnit.PlayerStat.GetStat(statType);
    }

    private void ShowUI()
    {
        playerStatTr.transform.position = originPos;
        playerStatTr.gameObject.SetActive(true);
        UpdateAllStats();
        playerStatTr.DOMove(animTargetTr.position, showAnimDuration);
    }

    private void HideUI()
    {
        playerStatTr.gameObject.SetActive(false);
    }
}
