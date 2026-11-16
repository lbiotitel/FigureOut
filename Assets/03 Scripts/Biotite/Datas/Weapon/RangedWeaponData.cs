using UnityEngine;


public class RangedWeaponData : WeaponData
{
    [Header("원거리 무기 속성")]
    [SerializeField] private EAmmoType ammoType;
    [SerializeField] private float reloadTime;
    [SerializeField] private int magazineSize;
    [SerializeField, Range(0f, 0.6f)] private float recoil;
    [SerializeField] private float soundSize;

    [Header("투사체")]
    [SerializeField] private ProjectileData projectileData;

    private int currentMagazineSize = 0;

    public event System.Action<int, int> OnAmmoChanged;

    public EAmmoType AmmoType => ammoType;
    public float ReloadTime => reloadTime;
    public int MagazineSize => magazineSize;
    public float Recoil => recoil;
    public float SoundSize => soundSize;
    public ProjectileData ProjectileData => projectileData;
    public int CurrentMagazineSize { get => currentMagazineSize; set { currentMagazineSize = Mathf.Clamp(value,0,magazineSize); } }

    public void NotifyAmmoChanged(int hasAmmo)
    {
        OnAmmoChanged?.Invoke(currentMagazineSize, hasAmmo);
    }
}
