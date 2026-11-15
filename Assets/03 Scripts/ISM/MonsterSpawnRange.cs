using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawnRange : MonoBehaviour
{
    
    private void OnTriggerStay(Collider other)
    {
        EnemySpawner ES= other.GetComponent<EnemySpawner>();
        if (ES == null)
            return;
        ES.Activate();
    }

    private void OnTriggerExit(Collider other)
    {
        EnemySpawner ES = other.GetComponent<EnemySpawner>();
        if (ES == null) return;

        ES.Deactivate();
    }

}
