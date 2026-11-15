using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisionTest : MonoBehaviour
{
    private StatBase atkRange;
    private IVisionProvider visionProvider;

    private float atkConeRange;

    [SerializeField] private float distanceMargin = 2f;
    [SerializeField] private float angleMargin = 3f;
    private float trickAngle;

    private Collider[] buffer = new Collider[512];

    private Transform lookTransform;

    private Camera mainCamera;

    public void Init(StatBase atkRange, IVisionProvider provider)
    {
        lookTransform = transform;
        this.atkRange = atkRange;
        this.visionProvider = provider;

        mainCamera = Camera.main;   
    }

    public Transform Detect()
    {;
        atkConeRange = Mathf.Max(0, visionProvider.ViewRadius - distanceMargin);

        float maxDetectionRadius = Mathf.Max(visionProvider.CircleRadius, atkConeRange);

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            maxDetectionRadius,
            buffer,
            visionProvider.TargetMask
        );

        Transform closestTarget = null;
        float closestDistance = float.MaxValue;

        trickAngle = Mathf.Max(0, visionProvider.ViewAngle - angleMargin);
        float cosHalfAngle = Mathf.Cos((trickAngle * 0.5f) * Mathf.Deg2Rad);

        for (int i = 0; i < hitCount; i++)
        {
            Collider col = buffer[i];

            if (col == null)
                continue;

            if (mainCamera != null)
            {
                Vector3 viewportPos = mainCamera.WorldToViewportPoint(col.transform.position);

                bool isInScreen = viewportPos.x >= 0.02f && viewportPos.x <= 0.98f &&
                                  viewportPos.y >= 0.02f && viewportPos.y <= 0.98f &&
                                  viewportPos.z > 0;

                if(!isInScreen)
                    continue;
            }

            Vector3 dir =
                col.transform.position - transform.position;

            float distance = dir.magnitude;

            if(distance < 0.001f) 
                continue;

            Vector3 dirNormalized = dir / distance;

            bool isCircle = 
                distance <= visionProvider.CircleRadius;

            bool isCone = false;

            if(!isCircle)
            {
                float dot = Vector3.Dot
                    (lookTransform.forward, dirNormalized);

                isCone = dot >= cosHalfAngle;
            }

            bool blocked = Physics.Raycast(
                transform.position,
                dir.normalized,
                distance,
                visionProvider.AttackObstacleMask
            );

            if (!isCircle && !isCone)
                continue;

            if (blocked)
            {
                Debug.DrawLine(
                    transform.position,
                    col.transform.position,
                    Color.red
                );

                continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = col.transform;
            }

        }
        return closestTarget;
    }

    private void OnDrawGizmos()
    {
        if (lookTransform == null) return;
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            visionProvider.CircleRadius
        );

        Gizmos.color = Color.white;

        Vector3 left = Quaternion.Euler(0, -trickAngle * 0.5f, 0) * lookTransform.forward;
        Vector3 right = Quaternion.Euler(0, trickAngle * 0.5f, 0) * lookTransform.forward;

        Gizmos.DrawRay(lookTransform.position, left * atkConeRange);
        Gizmos.DrawRay(lookTransform.position, lookTransform.forward * atkConeRange);
        Gizmos.DrawRay(lookTransform.position, right * atkConeRange);
    }
}