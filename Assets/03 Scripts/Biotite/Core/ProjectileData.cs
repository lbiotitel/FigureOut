using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum EProjectileMotionType
{ 
    Straight,
    Parabolic,
    Homing,
    Bazier,
    Impact
}

[System.Serializable]
public class ProjectileData
{
    [Header("Projectile Data")]
    [SerializeField] private EProjectileMotionType motionType = EProjectileMotionType.Straight;
    [SerializeField] private float projectileLifeTime;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private int projectileCount;
    [SerializeField] private float projectileSpreadAngle;

    [Header("Prefabs")]
    [SerializeField] private GameObject fireEffectPrefab;
    [SerializeField] private GameObject projectilePrefab;

    public EProjectileMotionType ProjectileMotionType => motionType;

    public float ProjectileLifeTime => projectileLifeTime;
    public float ProjectileSpeed => projectileSpeed;
    public int ProjectileCount => projectileCount;
    public float ProjectileSpreadAngle => projectileSpreadAngle;

    public GameObject FireEffectPrefab => fireEffectPrefab;
    public GameObject ProjectilePrefab => projectilePrefab;
}
