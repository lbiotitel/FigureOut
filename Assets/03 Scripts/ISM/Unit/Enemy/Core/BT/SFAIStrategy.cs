using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "AI/Strategy/SF")]
public class SFAIStrategy : EnemyAIStrategy
{
    public override Node SetupTree(Enemy enemy)
    {
        // 유틸리티 셀렉터에 가중치 로직을 담은 노드들을 주입 
        return new UtilitySelector(new List<Node> {
            
            


            // 사격 노드: 적이 보이면 기본 점수, 거리가 적절하면 보너스
            new ActionShoot(enemy)
            {
                ScoreModifier = () =>
                {
                    if (enemy.CanSeePlayer) return 80f;
                    return 0;
                }
            },

           
            new ActionPatrol(enemy)
            {
                ScoreModifier = () =>
                {
                if (!enemy.CanSeePlayer&&enemy.unitStat.GetStat(EStatType.Aggro)>0)
                    {
                        return 40f; // 일반 정찰(10점)보다 높은 점수로 수색 유도
                    }
                return 10f;
                }
            },

            new ActionPeriodicShare(enemy,EEnemyType.None   ,30f,2) {},

            new ActionSpawnLookingAround(enemy)
            {
                 ScoreModifier = () =>
                {
                    if(enemy.CanSeePlayer) return 0;
                    if (enemy.unitStat.GetStat(EStatType.Aggro)<=0) return 80f;
                    return 0;
                }
            }

        });//new UtilitySelector(new List<Node> { 의 끝 UtilitySelector 생성자의 끝

    }
 
}