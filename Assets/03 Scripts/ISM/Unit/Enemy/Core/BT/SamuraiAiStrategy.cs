using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Strategy/Samurai")]

public class SamuraiAiStrategy : EnemyAIStrategy
{
    public override Node SetupTree(Enemy enemy)
    {
        return new UtilitySelector(new List<Node>
        {
            new ActionShoot(enemy)
            {
                ScoreModifier = () =>
                {
                    if (enemy.CanSeePlayer&&enemy.GetDistanceFromPlayer()<enemy.enemyData.optimalRange) return 80f;
                    return 0;
                }
            },

            new ActionPatrol(enemy)
            {
                ScoreModifier=()=>
                {
                    if(!enemy.CanSeePlayer)
                    {
                        return 100;
                    }
                    return 0;
                }
            },

            new ActionAcceleratedApproach(enemy, 15f,1.5f){},

            new ActionLifelinePeriodicShare(enemy,EEnemyType.samurai,0.5f,10,1){}


        });
    }
}
