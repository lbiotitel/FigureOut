using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRangedWeapon : WeaponBase
{

    protected RangedWeaponData rangedWeaponData;
    protected RecoilController recoilController;

    private ReloadUI reloadUI;
    private string ammoType;
    private bool isReload = false;
    private MuzzleFlash muzzleFlash;
    private PlayerUnit playerUnit;


    private void Awake()
    {
        recoilController = GetComponent<RecoilController>();
    }

    private void Update()
    {
        CheckReload();
    }


    public override void Init(Unit ownUnit, WeaponData weaponData)
    {
        base.Init(ownUnit, weaponData);
        playerUnit = ownUnit as PlayerUnit;
        rangedWeaponData = weaponData as RangedWeaponData;
        recoilController.Init(transform);

        ammoType = Utils.AmmoTypeToItemId.GetItemId(rangedWeaponData.AmmoType);
        
        reloadUI = ((PlayerEquipmentController)(playerUnit.EquipmentController)).ReloadUI;
        
        muzzleFlash =  Instantiate(rangedWeaponData.ProjectileData.FireEffectPrefab, firePos)
            .GetComponent<MuzzleFlash>();
        
        isReload = false;
    }



    //원거리 무기 불릿 생성
    protected override void OnUse(Transform targetTransform)
    {
        if (rangedWeaponData.CurrnetDurability <= 0) return;
        if (rangedWeaponData.CurrentMagazineSize <= 0 || isReload) return;
        recoilController.AddRecoil(rangedWeaponData.Recoil, rangedWeaponData.Recoil, rangedWeaponData.Recoil, 0.2f);
        rangedWeaponData.CurrentMagazineSize = rangedWeaponData.CurrentMagazineSize - 1;

        Managers.Audio.PlaySfx(rangedWeaponData.SoundSFX);
        muzzleFlash.PlayEffect();

        for (int i = 0; i < rangedWeaponData.ProjectileData.ProjectileCount; i++)
        {
            CreateBullet(targetTransform);
        }

        rangedWeaponData.CurrnetDurability = rangedWeaponData.CurrnetDurability - 1f;
        NotifyAmmoUI();
    }


    private void CheckReload()
    {
        if (Managers.Input.ReloadButton) Reload();
    }


    //리로드 나중에 리로드 타임에 가속 곱해서 줄여야함 나중에 리팩토링 해서 좀 깔끔하게 다듬어야함
    protected virtual void Reload()
    {
        if (isReload || rangedWeaponData.CurrnetDurability <= 0) return;

        int currentMagazineSize = rangedWeaponData.CurrentMagazineSize;
        int magazineSize = rangedWeaponData.MagazineSize;
        int requriedCount = magazineSize - currentMagazineSize;

        if (requriedCount <= 0) return;
        InventorySystem inventorySystem = playerUnit.InventoryHolder.InventorySys;
        if (!inventorySystem.ContainsItem(ammoType)) return;

        Logger.Log("리로드 중");
        isReload = true;

        ReloadTime(reloadTime, () =>
        { 
            if (CheckAmmo(out List<InventorySlot> invSlots))
            {
                if (invSlots != null)
                {
                    for (int i = 0; i < invSlots.Count; i++)
                    {
                        int invAmmoCount = invSlots[i].StackSize;

                        if (requriedCount < invAmmoCount)
                        {
                            invSlots[i].RemoveFromStack(requriedCount);
                            rangedWeaponData.CurrentMagazineSize = rangedWeaponData.CurrentMagazineSize + invAmmoCount;
                            break;
                        }
                        else
                        {
                            invSlots[i].RemoveFromStack(invAmmoCount);
                            rangedWeaponData.CurrentMagazineSize = rangedWeaponData.CurrentMagazineSize + invAmmoCount;
                            requriedCount -= invAmmoCount;
                        }
                    }
                    Managers.Event.NotifyEvent<InventorySlotDataChanged>(new InventorySlotDataChanged { inventorySlots = invSlots });

                    NotifyAmmoUI();
                }
            }
        });
    }

    private async void ReloadTime(float time, Action action)
    {
        reloadUI?.Reload(time);
        await UniTask.WaitForSeconds(time);
        action?.Invoke();
        isReload = false;
        Managers.Audio.PlaySfx(KeyDefines.SoundKey.ReloadSfx);

        Logger.Log("리로드 완료");
    }



    //나중에 재장전 할 때만 체크하도록 바꿔야할듯 총기를 버리면 총알 개수를 가지고 있어야함
    protected virtual bool CheckAmmo(out List<InventorySlot> invSlots)
    {
        invSlots = null;

        InventorySystem inventorySystem = playerUnit.InventoryHolder.InventorySys;
        if (inventorySystem.ContainsItem(ammoType, out List<InventorySlot> targetSlots))
        {
            if (targetSlots.Count > 0)
            {
                invSlots = targetSlots;
                return true;
            }
        }

        return false;
    }


    private void CreateBullet(Transform targetTransform)
    {
        Logger.Log($"{rangedWeaponData.ItemName} 에서 발사됨");
        if (rangedWeaponData.ProjectileData.ProjectilePrefab == null) return;
        if (ownUnit == null) return;

        Vector3 spawnPos = firePos.position;
        Vector3 targetPos = targetTransform.position;
        targetPos = new Vector3(targetPos.x, spawnPos.y, targetPos.z);

        Vector3 direction = (targetPos - spawnPos).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(direction);
        float spreadX = UnityEngine.Random.Range(-rangedWeaponData.ProjectileData.ProjectileSpreadAngle, rangedWeaponData.ProjectileData.ProjectileSpreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, spreadX, 0);

        Quaternion finalRotation = baseRotation * spreadRotation;

        LayerMask targetLayer = LayerMask.GetMask(KeyDefines.MaskKey.ENEMY_MASK);

        Projectile projecetile = Managers.Object.CreateProjectile(rangedWeaponData.ProjectileData, ownUnit, spawnPos, targetPos, finalRotation ,targetLayer);

    }

    private void NotifyAmmoUI()
    {
        InventorySystem inventorySystem = playerUnit.InventoryHolder.InventorySys;
        int hasAmmo = inventorySystem.CountItem(ammoType);
        rangedWeaponData.NotifyAmmoChanged(hasAmmo);
    }
}

