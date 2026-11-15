using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ShowUI : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject objUI;

    [SerializeField] private Transform parentCraftUI;
    [SerializeField] private Transform parentShopUI;
    //[SerializeField] private AssetReference assetCraftUI;
    //[SerializeField] private AssetReference assetShopUI;

    [Header("인벤토리 접근")]
    [SerializeField] private List<ShopSlotData> shopItemSlot;
    [SerializeField] private CraftManager craftManager;

    [Header("상점 UI 제어")]
    public Action<bool> OnUIEnabled;

    private void Start()
    {
        //AddressablesAsync().Forget();
    }

    private void OnEnable()
    {
        
        if (gameObject.CompareTag("CraftNPC"))
        {
            objUI = parentCraftUI.GetChild(0).gameObject;
            //objUI = Managers.Data.GetData<GameObject>("Canvas_Craft");
        }

        else if (gameObject.CompareTag("ShopNPC"))
        {
            objUI = parentShopUI.GetChild(0).gameObject;
            //objUI = Managers.Data.GetData<GameObject>("Canvas_Shop");
        }

        craftManager = FindFirstObjectByType<CraftManager>(FindObjectsInactive.Exclude);
    }

    /*
    private async UniTaskVoid AddressablesAsync()
    {
        if(gameObject.CompareTag("CraftNPC"))
        {
            //var loadAsset = await assetCraftUI.LoadAssetAsync<GameObject>();
            objUI = await assetCraftUI.InstantiateAsync();
        }

        else if(gameObject.CompareTag("ShopNPC"))
        {
            //var loadAsset = await assetShopUI.LoadAssetAsync<GameObject>();
            objUI = await assetShopUI.InstantiateAsync();
        }
    }
    */
    private void OnTriggerEnter(Collider other)
    {
        if(gameObject.CompareTag("CraftNPC"))
        {
            Utils.CheckPlayerInteract(other, this, (playerUnit) => {
                Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                    new PlayerTriggerEnter
                    {
                        position = transform.position,
                        interactableObject = this,
                        interactText = "제작하기"
                    });
            });
        }

        else if (gameObject.CompareTag("ShopNPC"))
        {
            Utils.CheckPlayerInteract(other, this, (playerUnit) => {
                Managers.Event.NotifyEvent<PlayerTriggerEnter>(
                    new PlayerTriggerEnter
                    {
                        position = transform.position,
                        interactableObject = this,
                        interactText = "방랑 상점 방문"
                    });
            });
        }

    }


    private void OnTriggerStay(Collider other)
    {
        if (Managers.Input.InteractButton)
        {
            Utils.CheckPlayerInteract(other, this, Interact);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        objUI.SetActive(false);
        OnUIEnabled?.Invoke(false);
        Managers.Event.NotifyEvent<PlayerTriggerExit>(new PlayerTriggerExit());
        //CloseUI();
    }

    public void Interact(PlayerUnit unit)
    {
        if (gameObject.CompareTag("CraftNPC"))
        {
            Logger.Log("제작하기");
            objUI.SetActive(true);
            Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });

            craftManager.player = unit;
            craftManager.inventorySystem = unit.InventoryHolder.InventorySys;
        }

        else if (gameObject.CompareTag("ShopNPC"))
        {
            Logger.Log("방랑 상점 방문");
            objUI.SetActive(true);
            OnUIEnabled?.Invoke(true);
            Managers.Event.NotifyEvent<InteractEvent>(new InteractEvent { interactableObject = this });

            ShopSlotData[] slots = FindObjectsByType<ShopSlotData>(FindObjectsSortMode.None);
            for (int i = 0; i < slots.Length; i++)
            {
                shopItemSlot.Add(slots[i]);
                slots[i].player = unit;
                slots[i].InitializeInventory();
            }
        }

    }

    private void CloseUI()
    {
        //Addressables.ReleaseInstance(objUI);
        objUI = null;
    }
}
