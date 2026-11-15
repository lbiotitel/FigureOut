using UnityEngine;




public class WeaponData : EquipmentData
{
    [SerializeField] private string soundSFX;

    [SerializeField] private Vector3 firePos;

    [SerializeField] private EWeaponType weaponType;

    public string SoundSFX => soundSFX;
    public Vector3 FirePos => firePos;
    public EWeaponType WeaponType => weaponType;
}
