using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyAIStrategy : ScriptableObject
{
    // 각 몬스터가 어떤 행동 트리(Node)를 가질지 여기서 정의.
    public abstract Node SetupTree(Enemy enemy);
}