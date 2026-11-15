using Cysharp.Threading.Tasks;
using System;
using UnityEngine;


//필드에 출력되는 아이템 드롭 오브젝트 입니다.
public class DropItem : MonoBehaviour
{
    [SerializeField] private ParticleSystem lineParticle;
    [SerializeField] private ParticleSystem auraParticle;
    

    [SerializeField] private Transform itemModelTr;
    [SerializeField] private ParticleSystem aura;

    [SerializeField] private float duration;
    [SerializeField] private float rotateSpeed = 20f;
    [SerializeField] private float pickUpTime = 1f;

    private PlayerUnit target;
    public ItemData ItemData { get; private set; } = null;
    public int Amount { get; private set; } = 1;
    public bool IsPickedUp { get; private set; } = false;
    public bool CanPickedUp { get; private set; } = false;

    public event Action<DropItem> OnPickUpAnimFinish;

    private float elapsedTime = 0f;
    private Vector3 startPos;


    //새로운 아이템을 초기화
    public void Init(string itemKey, int amount = 1)
    {
        ItemData = Managers.Data.GetData<ItemData>(itemKey);

        if (ItemData == null)
        {
            Logger.LogError($"{itemKey}에 대한 데이터가 없습니다.");
            Destroy(gameObject);
            return;
        }

        Init(ItemData, amount);
    }


    //인게임에 있는 아이템 데이터로 초기화
    public void Init(ItemData itemData, int amount = 1)
    {
        if (itemData != null)
        {
            ItemData = itemData;
            Amount = amount;
            CreateModel(ItemData.ModelPrefab);
            ShowAura(ItemData.Rarity);
            PickDelay();
        }
    }

    private async void PickDelay()
    {
        await UniTask.Delay(TimeSpan.FromSeconds(pickUpTime));
        CanPickedUp = true;
        startPos = transform.position;
    }



    private void Update()
    {
        RotateModel();
        FollowTarget();
    }

    public void PickUp(Transform target)
    {
        if (!CanPickedUp) return;
        IsPickedUp = true;
        this.target = target.GetComponent<PlayerUnit>();
    }

    //드롭준비가 되었다면 플레이어를 따라간다.
    private void FollowTarget()
    {
        if (!IsPickedUp || !CanPickedUp) return;
        if (!HasFreeSlot())
        {
            CanPickedUp = false;
            IsPickedUp = false;
            return;
        }

        if (Vector3.Distance(transform.position, target.transform.position) > 0.1f)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);

            float acc = t * t;

            transform.position = Vector3.Lerp(startPos, target.transform.position, acc);
        }
        else
        {
            elapsedTime = 0f;
            OnPickUpAnimFinish?.Invoke(this);
        }
    }

    private bool HasFreeSlot()
    {
        return target.InventoryHolder.InventorySys.HasFreeSlot();
    }


    private void RotateModel()
    {
        itemModelTr.Rotate(Vector3.up, rotateSpeed * Time.deltaTime);
    }

    //아이템 모델 생성
    private void CreateModel(GameObject model)
    {
        if (model == null)
        {
            Logger.LogError($"{ItemData.ItemName} 의 모델이 등록되지 않았습니다.");
            return;
        }
        GameObject itemModel = Instantiate(model, itemModelTr);
        itemModel.transform.localPosition = Vector3.zero;
        itemModel.transform.localScale = Vector3.one;
    }


    //등급별 아우라 효과
    private void ShowAura(ERarity rarity)
    {
        var lineMain = lineParticle.main;
        var auraMain = auraParticle.main;

        switch (rarity)
        {
            case ERarity.Normal:
                lineMain.startColor = Color.white;
                auraMain.startColor = Color.white;
                break;
            case ERarity.Rare:
                lineMain.startColor = Color.cyan;
                auraMain.startColor = Color.cyan;
                break;
            case ERarity.Epic:
                Color epicColor = new Color(160f / 255f, 32f / 255f, 240f / 255f);
                lineMain.startColor = epicColor;
                auraMain.startColor = epicColor;
                break;

            case ERarity.Legend:
                Color legendColor = new Color(255f / 255f, 127f / 255f, 0f / 255f);
                lineMain.startColor = legendColor;
                auraMain.startColor = legendColor;
                break;
        }
    }

}
