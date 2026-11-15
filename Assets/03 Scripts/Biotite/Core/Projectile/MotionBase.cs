using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionBase : MonoBehaviour
{

    protected Unit ownUnit;
    protected ProjectileData projectileData;
    protected Vector3 startPos;
    protected Vector3 targetPos;
    protected Rigidbody rb;

    public virtual void Init(ProjectileData projectileData, Unit ownUnit,Vector3 startPos, Vector3 targetPos)
    {
        rb = GetComponent<Rigidbody>();
        this.startPos = startPos;
        this.targetPos = targetPos;
        this.projectileData = projectileData;
        this.ownUnit = ownUnit;
    }

    public virtual void DoMotion() { }

    public virtual void StopMotion() { }
}
