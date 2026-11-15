using UnityEngine;

public class ItemData : ScriptableObject
{
    [SerializeField] protected string itemId;
    [SerializeField] protected string itemName;
    [SerializeField,TextArea] protected string  itemDescription;
    [SerializeField] protected Sprite itemIcon;
    [SerializeField] protected EItemType itemType;
    [SerializeField] protected ERarity rarity;
    [SerializeField] protected GameObject modelPrefab;
    [SerializeField] protected int maxStackCount;
    [SerializeField] protected int price;

    public string ItemID => itemId;
    public string ItemName => itemName;
    public string ItemDescription => itemDescription;   
    public Sprite ItemIcon => itemIcon;
    public EItemType ItemType => itemType;
    public ERarity Rarity => rarity;
    public int MaxStackCount => maxStackCount;
    public int Price => price;
    public GameObject ModelPrefab => modelPrefab;
}
