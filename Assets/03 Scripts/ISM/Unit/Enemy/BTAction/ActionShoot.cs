using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ActionShoot : Node
{
    private Enemy enemy;
    

    public ActionShoot(Enemy enemy)
    {
        this.enemy = enemy;
    }


    public override State Evaluate()
    {
        Debug.Log("ActionShoot");
        if (!enemy.CanSeePlayer) return State.Failure;
        enemy.UpdateGhostPosition(enemy.Target);

        enemy.Attack();

        return State.Running; // 계속 사격 중임을 알림
    }
}