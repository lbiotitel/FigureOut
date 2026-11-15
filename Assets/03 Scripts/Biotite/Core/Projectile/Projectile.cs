using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(BoxCollider))]
public class Projectile : MonoBehaviour, IPoolingObject
{
    protected ProjectileData projectileData;
    protected Unit ownUnit;

    protected Vector3 startPos;
    protected Vector3 targetPos;

    protected DmgContext context;
    protected Rigidbody rb;

    private LayerMask targetLayer;
    private bool isAttack = false;

    private MotionBase motion;

    public GameObject OriginPrefab { get; set; }
    private bool isReturn = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Init(ProjectileData projectileData, Unit ownUnit, Vector3 startPos, Vector3 targetPos, LayerMask layerMask)
    {
        this.projectileData = projectileData;
        this.ownUnit = ownUnit;
        this.startPos = startPos;
        this.targetPos = targetPos;

        rb.detectCollisions = true;
        rb.isKinematic = false;
        rb.velocity = Vector3.zero;

        context = new DmgContext(ownUnit);

        isReturn = false;
        isAttack = false;

        targetLayer = layerMask;

        InitMotion();

        Utils.Delay(projectileData.ProjectileLifeTime, () => ReturnObject());
    }

    private void InitMotion()
    {
        if (projectileData.ProjectileMotionType == EProjectileMotionType.Straight)
        {
            if (motion == null)
            {
                motion = gameObject.AddComponent<StraightMotion>();
            }

            motion.Init(projectileData, ownUnit, startPos, targetPos);
            motion.DoMotion();
        }
    }

    protected virtual void DoDamage(IDmgAble dmgAble)
    {
        dmgAble.TakeDamage(context);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (isAttack || isReturn) return;

        if ((targetLayer.value & (1 << other.gameObject.layer)) != 0)
        {
            if (other.gameObject.TryGetComponent<IDmgAble>(out var damageable))
            {
                isAttack = true;
                DoDamage(damageable);
                ClearMotion();
                Utils.Delay(3f, () => ReturnObject());
            }
        }
        else
        {
            ClearMotion();
            ReturnObject();
        }

        rb.detectCollisions = false;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ClearMotion()
    {
        if (motion != null)
        {
            motion.StopMotion();
        }
    }

    public void ResetObject()
    {
        isReturn = true;
        projectileData = null;
        ownUnit = null;
        context = null;
        isAttack = false;

        ClearMotion();
    }

    public void ReturnObject()
    {
        if (isReturn) return;

        ResetObject();
        Managers.Object.PoolingSystem.ReturnObject(gameObject);
    }
}