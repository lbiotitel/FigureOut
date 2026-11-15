using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Strategy/Spy")]

public class SpyAiStrategy : EnemyAIStrategy
{
    public override Node SetupTree(Enemy enemy)
    {
        return new UtilitySelector(new List<Node>
        {
            new ActionShoot(enemy)
            {
                ScoreModifier = () =>
                {
                    if (enemy.CanSeePlayer) return 80f;
                    return 0;
                }
            },

            new ActionEncounter(enemy)
            {
                
            },

            new ActionLifelineEscape(enemy,0.7f)
            {

            },

            new ActionLifelineShare(enemy,EEnemyType.None,0.5f,10)
            {

            },

            new ActionLifelineBuff(enemy,0.3f)
            {

            },

            new ActionPatrol(enemy)
            {
                ScoreModifier=()=>
                {
                    return 10;
                }
            }


        });
    }
}
