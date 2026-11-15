using Cysharp.Threading.Tasks;
using System.Threading;
using System;
using UnityEngine;
using Unity.VisualScripting.Antlr3.Runtime;

public class StraightMotion : MotionBase
{
    
    private bool isStop = true;

    public override void Init(ProjectileData projectileData, Unit ownUnit, Vector3 startPos, Vector3 targetPos)
    {
        base.Init(projectileData, ownUnit, startPos, targetPos);
        isStop = true;
    }

    public override void DoMotion()
    {
        isStop = false;
    }

    private void Update()
    {
        if (isStop) return;
        if (rb != null) rb.velocity = rb.transform.forward * projectileData.ProjectileSpeed;
    }

    public override void StopMotion()
    {
        isStop = true;
    }
}