using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRangedWeapon : WeaponBase
{

    protected RangedWeaponData rangedWeaponData;
    protected RecoilController recoilController;

    private bool isReload = false;
    private MuzzleFlash muzzleFlash;
    private Enemy enemy;


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
        enemy = ownUnit as Enemy;
        rangedWeaponData = weaponData as RangedWeaponData;
        recoilController.Init(transform);
        muzzleFlash = Instantiate(rangedWeaponData.ProjectileData.FireEffectPrefab, firePos)
            .GetComponent<MuzzleFlash>();

        rangedWeaponData.CurrentMagazineSize = rangedWeaponData.MagazineSize;
        isReload = false;
    }



    //원거리 무기 불릿 생성
    protected override void OnUse(Transform targetTransform)
    {
        if (rangedWeaponData.CurrentMagazineSize <= 0 || isReload) return;
        recoilController.AddRecoil(rangedWeaponData.Recoil, rangedWeaponData.Recoil, rangedWeaponData.Recoil, 0.2f);
        rangedWeaponData.CurrentMagazineSize = rangedWeaponData.CurrentMagazineSize - 1;

        Managers.Audio.PlaySfx(rangedWeaponData.SoundSFX);
        muzzleFlash.PlayEffect();

        for (int i = 0; i < rangedWeaponData.ProjectileData.ProjectileCount; i++)
        {
            CreateBullet(targetTransform);
        }
    }


    private void CheckReload()
    {
        if (isReload || rangedWeaponData.CurrentMagazineSize > 0) return;
        isReload = true;
        ReloadTime(reloadTime, () =>
        { rangedWeaponData.CurrentMagazineSize = rangedWeaponData.MagazineSize; });
    }

    private async void ReloadTime(float time, Action action)
    {
        await UniTask.WaitForSeconds(time);
        action?.Invoke();
        isReload = false;
    }


    private void CreateBullet(Transform targetTransform)
    {
        if (rangedWeaponData.ProjectileData.ProjectilePrefab == null) return;

        Vector3 spawnPos = firePos.position;
        Vector3 targetPos = targetTransform.position;
        targetPos = new Vector3(targetPos.x, spawnPos.y, targetPos.z);

        Vector3 direction = (targetPos - spawnPos).normalized;
        Quaternion baseRotation = Quaternion.LookRotation(direction);
        float spreadX = UnityEngine.Random.Range(-rangedWeaponData.ProjectileData.ProjectileSpreadAngle, rangedWeaponData.ProjectileData.ProjectileSpreadAngle);
        Quaternion spreadRotation = Quaternion.Euler(0, spreadX, 0);

        Quaternion finalRotation = baseRotation * spreadRotation;

        LayerMask targetLayer = LayerMask.GetMask(KeyDefines.MaskKey.PLAYER_MASK);

        Projectile projecetile = Managers.Object.CreateProjectile(rangedWeaponData.ProjectileData, ownUnit, spawnPos, targetPos, finalRotation,targetLayer);

    }

}
