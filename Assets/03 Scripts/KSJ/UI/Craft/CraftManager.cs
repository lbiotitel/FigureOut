using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.AddressableAssets;
using System.Linq;
using DG.Tweening;

public class CraftManager : MonoBehaviour
{
    [Header("아이템 참조")]
    // 아이템 풀 리스트
    [SerializeField] private List<ItemData> itemData;
    // 장비아이템 풀 리스트
    [SerializeField] private List<ItemData> equipItemData;
    // 아이템 제작 레시피 리스트
    [SerializeField] private Dictionary<string, ItemCraftRecipt> itemCraftRecipt;
    // 모든 아이템 제작 레시피 리스트
    [SerializeField] List<ItemData> allItemList;

    [Header("제작 아이템 카탈로그")]
    // UI 버튼 부모
    [SerializeField] private Transform parentCatalogUI;
    // 아이템 정보를 저장할 UI 버튼
    [SerializeField] private GameObject catalogButtonUI;
    [SerializeField] private List<GameObject> catalogUIGroup;
    [SerializeField] private List<CraftThumbnailData> catalogData;

    [Header("제작 UI 영역")]
    public ItemData targetItem;
    [SerializeField] private Outline uiShowGrade;
    [SerializeField] private Image targetItemImage;
    [SerializeField] private TextMeshProUGUI targetItemName;
    [SerializeField] private TextMeshProUGUI targetItemDescription;

    [SerializeField] private ItemCraftRecipt currItemRecipt;
    [SerializeField] private GameObject reqItemIconPrefab;
    [SerializeField] private Transform transformParentReqItemIcon;

    [Header("제작 버튼")]
    [SerializeField] private Button buttonCraft;
    [SerializeField] private ItemData targetItemData;
    [SerializeField] private List<CraftReqItemData> requiredItemList;

    [Header("제작 연출")]
    [SerializeField] private Slider progressSlider;
    private Tween progressTween;
    [SerializeField] private bool isCrafting = false;
    [SerializeField] private Button progressAccelButton;
    [SerializeField] private GameObject spriteAnvil;

    // 데이터 보관용
    private CraftReqItemData requiredIngredient;

    [Header("유저 인벤토리 정보")]
    public PlayerUnit player;
    public InventorySystem inventorySystem;

    [Header("등급 표시 색")]
    [ColorUsage(true)]
    [SerializeField] private Color colorGradeNormal;
    [ColorUsage(true)]
    [SerializeField] private Color colorGradeRare;
    [ColorUsage(true)]
    [SerializeField] private Color colorGradeEpic;
    [ColorUsage(true)]
    [SerializeField] private Color colorGradeLegend;

    private void Start()
    {
        Utils.Delay(2f, () =>
        {
            MergeItemTable();
        });

        Managers.Event.AddEvent<InventoryDataChangedEvent>(HandleInventorySlotChnagedEvent);
    }

    private void MergeItemTable()
    {
        // 아이템 SO는 ItemData 스크립트로 묶이지 않는다!
        // 각각의 아이템 SO 분류 리스트를 캐싱하고 다시 하나의 목록으로 합치는 작업이 필요

        // 만들어주신 GetDatas<>()와 Cast<>()를 사용하여 value 참조로 가져온다
        // (원본 값이 바뀌지 않도록 값만을 복사하여 가져온다)

        // 소모성 아이템 캐스팅
        List<ConsumableData> consumableDatas = Managers.Data.GetDatas<ConsumableData>();
        itemData = consumableDatas.Cast<ItemData>().ToList();
        allItemList.AddRange(itemData);

        // 장비 아이템 캐스팅
        equipItemData.Cast<ItemData>().ToList();
        allItemList.AddRange(equipItemData);

        List<ItemCraftRecipt> craftReciptDatas = Managers.Data.GetDatas<ItemCraftRecipt>().
            Cast<ItemCraftRecipt>().ToList();
        itemCraftRecipt = craftReciptDatas.ToDictionary(key => key.CraftItemID);

        for (int i = 0; i < allItemList.Count; i++)
        {
            // 리스트에 맞게 아이템을 가진 UI 생성
            GameObject clone = Instantiate(catalogButtonUI, parentCatalogUI);
            CraftThumbnailData cloneUIData = clone.GetComponent<CraftThumbnailData>();
            cloneUIData.CopyItemData(allItemList[i]);
            cloneUIData.craftManager = this;
        }

        if (targetItem != null) SetUIInfo(targetItem);
        else                    SetUIInfo(allItemList[0]);
    }

    // 제작창 분류
    public void OnDropdownChanged(int index)
    {
        foreach(Transform child in parentCatalogUI)
        {
            child.gameObject.SetActive(false);
        }

        List<ItemData> currItemSort = new List<ItemData>();
        
        switch(index)
        {
            case 0:
                currItemSort = allItemList;
                break;

            case 1:
                for (int i = 0; i < allItemList.Count; i++)
                {
                    if (allItemList[i].ItemID.Contains("FO") || allItemList[i].ItemID.Contains("RC"))
                    {
                        currItemSort.Add(allItemList[i]);
                    }
                }
                break;

            case 2:
                for (int i = 0; i < allItemList.Count; i++)
                {
                    if (allItemList[i].ItemID.Contains("AM"))
                    {
                        currItemSort.Add(allItemList[i]);
                    }
                }
                break;

            case 3:
                for (int i = 0; i < allItemList.Count; i++)
                {
                    if (allItemList[i].ItemID.Contains("10010"))
                    {
                        currItemSort.Add(allItemList[i]);
                    }
                }
                break;
        }

        if (currItemSort.Count == 0) return;

        for (int i = 0; i < currItemSort.Count; i++)
        {
            // 리스트에 맞게 아이템을 가진 UI 생성
            GameObject clone = Instantiate(catalogButtonUI, parentCatalogUI);
            CraftThumbnailData cloneUIData = clone.GetComponent<CraftThumbnailData>();
            cloneUIData.CopyItemData(currItemSort[i]);
            cloneUIData.craftManager = this;
        }

        if (targetItem != null) SetUIInfo(targetItem);
        else SetUIInfo(itemData[0]);
    }


    public void SetUIInfo(ItemData itemData)
    {
        targetItemData = itemData;

        if (itemData.ItemIcon != null)
            targetItemImage.sprite = itemData.ItemIcon;

        targetItemName.text = itemData.ItemName;
        targetItemDescription.text = itemData.ItemDescription;

        switch (itemData.Rarity)
        {
            case ERarity.Normal:
                uiShowGrade.effectColor = colorGradeNormal;
                break;

            case ERarity.Rare:
                uiShowGrade.effectColor = colorGradeRare;
                break;

            case ERarity.Epic:
                uiShowGrade.effectColor = colorGradeEpic;
                break;

            case ERarity.Legend:
                uiShowGrade.effectColor = colorGradeLegend;
                break;

            default:
                // 기본적으로 노말 등급을 띄움
                uiShowGrade.effectColor = colorGradeNormal;
                break;
        }

        if (itemCraftRecipt.TryGetValue(itemData.ItemID, out ItemCraftRecipt recipt))
        {
            currItemRecipt = recipt;

            ClearItemList();
            ShowRequiredItemList(recipt);
        }
    }

    /// <summary>
    /// 반복 생성된 필요 아이템 UI 초기화
    /// </summary>
    private void ClearItemList()
    {
        CraftReqItemData[] uiChilds = transformParentReqItemIcon.GetComponentsInChildren<CraftReqItemData>();

        for(int i = 0; i < uiChilds.Length; i++)
        {
            uiChilds[i].gameObject.SetActive(false);
        }

        requiredItemList.Clear();
    }

    /// <summary>
    /// 각 아이템마다 요구하는 재료 보여주기
    /// </summary>
    /// <param name="recipt"></param>
    private void ShowRequiredItemList(ItemCraftRecipt recipt)
    {
        if (recipt.CraftRecipt != null)
        {
            for (int i = 0; i < recipt.CraftRecipt.Count; i++)
            {
                GameObject clone = Instantiate(reqItemIconPrefab, transformParentReqItemIcon);
                CraftReqItemData reciptIngredient = clone.GetComponent<CraftReqItemData>();
                reciptIngredient.CopyItemData(Managers.Data.GetData<ItemData>(recipt.CraftRecipt[i].reqItemID));


                requiredIngredient = reciptIngredient;

                // 해당 재료의 소지 수량 / 목표 수량 넘겨주기
                reciptIngredient.GetIngredientAmount(inventorySystem.CountItem(recipt.CraftRecipt[i].reqItemID), recipt.CraftRecipt[i].ReqItemAmount);

                requiredItemList.Add(reciptIngredient);
            }

            foreach (CraftReqItemData recipe in requiredItemList)
            {
                if (recipe.currAmountValue >= recipe.reqAmountValue)
                {
                    buttonCraft.interactable = true;
                }

                else
                {
                    buttonCraft.interactable = false;
                }
            }
        }
    }


    public void DoCraft()
    {
        // 슬라이더 연출을 위해 값 초기화
        progressSlider.gameObject.SetActive(true);
        progressSlider.value = 0;
        progressSlider.DOKill();
        progressAccelButton.gameObject.SetActive(true);
        spriteAnvil.SetActive(true);

        int count = 0;

        foreach (CraftReqItemData recipe in requiredItemList)
        {
            if (recipe.currAmountValue >= recipe.reqAmountValue)
            {
                count++;
            }
        }

        // 아이템 개수가 충분한 걸 확인했으므로 제작 연출 시작
        if (count == requiredItemList.Count)
        {
            isCrafting = true;
            CraftProgress();
        }

        else
        {
            CancelCraft();
        }
    }

    public void CraftProgress()
    {
        float remainingValue = 1f - progressSlider.value;
        // 현재 남은 게이지의 비율에 따라 남은 시간도 줄어들어야 한다.
        float remainingTIme = remainingValue * 15f;

        // 0보다 작거나 같으면 즉시 완료
        if (remainingTIme <= 0) remainingTIme = 0.01f;

        // 연출 실행, 끝나면 아이템 지급
        progressTween = progressSlider.DOValue(1f, remainingTIme).SetEase(Ease.Linear).OnComplete(() =>
        {
            foreach (CraftReqItemData recipt in requiredItemList)
            {
                inventorySystem.RemoveItem(recipt.craftItemID, recipt.reqAmountValue);
                recipt.GetIngredientAmount(inventorySystem.CountItem(recipt.craftItemID), recipt.reqAmountValue);
            }

            inventorySystem.AddToInventory(targetItemData, 1);

            if (targetItemData.ItemID.Contains("AM"))
            {
                inventorySystem.AddToInventory(targetItemData, 19);
            }

            isCrafting = false;
            progressSlider.gameObject.SetActive(false);
            progressAccelButton.gameObject.SetActive(false);
            spriteAnvil.SetActive(false);
            spriteAnvil.GetComponent<Animator>().enabled = false;
        });
    }

    public void OnScreenClick()
    {
        if (!isCrafting) return;

        Animator anvilAnim = spriteAnvil.gameObject.GetComponent<Animator>();
        anvilAnim.enabled = true;
        anvilAnim.Play("ui_craft_anvil", -1, 0f);

        // 클릭으로 들어오는 값을 DOTween으로 증가시켜야 하므로 기존의 트윈을 제거
        progressTween.Kill();

        // 클릭으로 인한 값 즉시 증가
        progressSlider.value += 0.1f;

        if(progressSlider.value >= 1f)
        {
            progressSlider.value = 1f;
        }

        // 증가 후 다시 시간에 따른 슬라이더값 추가
        CraftProgress();
    }

    private void CancelCraft()
    {
        // 제작 불가
        buttonCraft.interactable = false;
    }


    // UI 상시 갱신
    private void HandleInventorySlotChnagedEvent(InventoryDataChangedEvent inventoryDataChanged)
    {
        if (currItemRecipt == null) return;

        for(int i = 0; i < requiredItemList.Count; i++)
        {
            requiredItemList[i].GetIngredientAmount(inventorySystem.CountItem(currItemRecipt.CraftRecipt[i].reqItemID), currItemRecipt.CraftRecipt[i].ReqItemAmount);
        }

        foreach (CraftReqItemData recipe in requiredItemList)
        {
            if (recipe.currAmountValue >= recipe.reqAmountValue)
            {
                buttonCraft.interactable = true;
            }

            else
            {
                buttonCraft.interactable = false;
            }
        }
    }


    private void OnDisable()
    {
        Managers.Event.RemoveEvent<InventoryDataChangedEvent>(HandleInventorySlotChnagedEvent);
    }
}
