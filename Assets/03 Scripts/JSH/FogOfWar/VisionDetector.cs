using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class VisionDetector : MonoBehaviour
{
    private IVisionProvider visionProvider;

    private List<Enemy> previouslyVisibleEnemies = new List<Enemy>();
        
    private Collider[] detectBuffer = new Collider[512];

    public void Init(IVisionProvider provider)
    {
        visionProvider = provider;

        FindTargetWithDelayAsync(0.1f, this.GetCancellationTokenOnDestroy()).Forget();
    }

    private async UniTaskVoid FindTargetWithDelayAsync(float delay, CancellationToken token)
    {
        while(!token.IsCancellationRequested)
        {
            await UniTask.WaitForSeconds(delay, cancellationToken: token);

            if(visionProvider != null && visionProvider.IsVisionActive)
            {
                FindVisibleTargets();
            }
        }
    }

    private void FindVisibleTargets()
    {
        List<Enemy> currentVisibleEnemies = new List<Enemy>();

        Physics.SyncTransforms();

        int hitCount = Physics.OverlapSphereNonAlloc(
            transform.position,
            visionProvider.ViewRadius,
            detectBuffer,
            visionProvider.TargetMask
        );

        for (int i = 0; i < hitCount; i++)
        {
            Transform target = detectBuffer[i].transform;
            Enemy enemy = target.GetComponentInParent<Enemy>();
            if (enemy == null) continue;

            Vector3 originPos = transform.position + Vector3.up * 1.5f;
            Vector3 targetPos = enemy.transform.position + Vector3.up * 1.5f;

            Vector3 dirToTarget = (targetPos - originPos).normalized;
            float dstToTarget = Vector3.Distance(originPos, targetPos);

            bool inCone = Vector3.Angle(transform.forward, dirToTarget) < visionProvider.ViewAngle * 0.5f;
            float maxDistance = inCone ? visionProvider.ViewRadius : visionProvider.CircleRadius;

            if (dstToTarget <= maxDistance)
            {
                if (!Physics.Raycast(originPos, dirToTarget, dstToTarget, visionProvider.VisionObstacleMask))
                {
                    if (!currentVisibleEnemies.Contains(enemy))
                    {
                        currentVisibleEnemies.Add(enemy);
                    }
                    enemy.SetVisible(true);
                }
            }
        }

        foreach (Enemy oldEnemy in previouslyVisibleEnemies)
        {
            if (!currentVisibleEnemies.Contains(oldEnemy))
            {
                if (oldEnemy != null)
                {
                    oldEnemy.SetVisible(false);
                }
            }
        }

        previouslyVisibleEnemies = currentVisibleEnemies;


    }
}
