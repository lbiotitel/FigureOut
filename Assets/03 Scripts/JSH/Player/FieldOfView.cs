using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    [SerializeField] private float viewRadius;

    [Range(0f, 360f)]
    [SerializeField] private float viewAngle;

    public LayerMask targetMask;
    public LayerMask obstacleMask;

    private Collider[] targetBuf = new Collider[20];

    private void FindVisibleTargets()
    {
        int hitTarget = Physics.OverlapSphereNonAlloc(
            transform.position,
            viewRadius,
            targetBuf,
            targetMask
        );

        for (int i = 0; i < hitTarget; i++)
        {

        }
    }

    public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            angleInDegrees = transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }

}
