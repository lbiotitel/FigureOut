using UnityEngine;

public interface IVisionProvider
{
    Transform VisionTransform { get; }

    float ViewRadius { get; }

    float ViewAngle { get; }

    float CircleRadius { get; }

    LayerMask TargetMask { get; }

    LayerMask VisionObstacleMask { get; }

    LayerMask AttackObstacleMask { get; }

    bool IsVisionActive { get; }

}
