using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopUIManager : MonoBehaviour
{
    [Header("아이템 목록")]
    // 아이템 개수
    [SerializeField] private int purchasableItemCount = 4;

    // 아이템 데이터 받아오는 리스트
    private List<ConsumableData> consumableDatas;

    // 아이템 풀 리스트
    [SerializeField] private List<ItemData> itemData;
    // 랜덤 아이템 리스트(인덱스로 담음)
    [SerializeField] private int[] randomItemContainer;

    [Header("상점 UI")]
    // UI 아이템 슬롯 오브젝트
    [SerializeField] private List<GameObject> itemSlots = new List<GameObject>();
    // UI 아이템 슬롯의 아이템 정보 담는 스크립트
    [SerializeField] private List<ShopSlotData> shopItemSlotData;

    [Header("상점 갱신")]
    [SerializeField] private DayCycleTimer dayCycle;


    private void Start()
    {
        dayCycle.onIntervalTimeChanged += RerollShop;

        Utils.Delay(2f, () =>
        {
            MergeItemTable();
        });
    }


    private void MergeItemTable()
    {
        // 아이템 SO는 ItemData 스크립트로 묶이지 않는다!
        // 각각의 아이템 SO 분류 리스트를 캐싱하고 다시 하나의 목록으로 합치는 작업이 필요

        // 만들어주신 GetDatas<>()와 Cast<>()를 사용하여 value 참조로 가져온다
        // (원본 값이 바뀌지 않도록 값만을 복사하여 가져온다)
        consumableDatas = Managers.Data.GetDatas<ConsumableData>();
        itemData = consumableDatas.Cast<ItemData>().ToList();

        // 아이템 풀에서 랜덤으로 뽑아오기
        RandomizeItemPool();

        // 뽑은 아이템 정보를 UI에 적용
        RefreshUI();
    }

    // 아이템 풀에서 랜덤으로 뽑아오기
    private void RandomizeItemPool()
    {
        randomItemContainer = Randomize.RandomNumbers(itemData.Count, purchasableItemCount);
    }

    // 뽑은 아이템 정보를 UI에 적용
    private void RefreshUI()
    {
        for (int i = 0; i < itemSlots.Count; i++)
        {
            itemSlots[i].GetComponent<ShopSlotData>().CopyItemData(itemData[randomItemContainer[i]]);
        }
    }

    // 밤낮이 바뀔 때마다 상점 갱신
    private void RerollShop(int time)
    {
        if(time == 0)
        {
            RandomizeItemPool();
            RefreshUI();
        }
    }

    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.F12))
        {
            RandomizeItemPool();
            RefreshUI();
        }
    }

    private void OnDisable()
    {
        dayCycle.onIntervalTimeChanged -= RerollShop;
    }

    [Serializable]
    public struct ShopItemPool
    {
        public EItemType itemSort;
        public string itemID;
    }
}

