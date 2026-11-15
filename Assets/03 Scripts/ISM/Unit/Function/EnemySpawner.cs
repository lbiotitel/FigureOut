using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyData data;
   [HideInInspector] public GameObject _spawnedEnemy;
    private bool _isDead = false; // 화톳불 전까지 유지되는 데이터

    // 1. 초기화 (매니저가 호출)
    public void Initialize(EnemyData enemyData)
    {
        data = enemyData;
    }

    // 2. 플레이어 트리거가 들어왔을 때 (Activate)
    public void Activate()
    {
        if (_isDead) return; // 죽어 있는 경우

        if (_spawnedEnemy != null) // 스폰한적 있던 경우
        {
            if (_spawnedEnemy.activeSelf) return;

            _spawnedEnemy.SetActive(true);

            Enemy enemyComponent = _spawnedEnemy.GetComponent<Enemy>();
            if (enemyComponent != null)
            {
                enemyComponent.ResetEnemyState(transform.position);
            }

            EnemyBTRunner runnerComponent = _spawnedEnemy.GetComponent<EnemyBTRunner>();
            if (runnerComponent != null)
            {
                runnerComponent.ResetRunner(data.aiStrategy);
            }
        }
        else
        {
            Spawn();
        }
    }

    // 3. 플레이어 트리거가 나갔을 때 (Deactivate)
    public void Deactivate()
    {
        // 멀어지면 적을 제거해서 CPU/메모리 확보
        if (_spawnedEnemy != null)
        {
            Despawn();
        }
    }

    private void Spawn()
    {
        //Debug.Log("스폰해줘");
        _spawnedEnemy = Managers.Object.CreateUnit(data, transform.position, this);
    }

    private void Despawn()
    {
        _spawnedEnemy.SetActive(false);
    }

    // 4. 에너미가 죽었을 때 호출됨
    public void ReportDeath()
    {
        _isDead = true;
        _spawnedEnemy = null; // 죽은 객체는 알아서 파괴되게 둠
    }

    // 5. 체크포인트 리셋
    public void ResetSpawner()
    {
        _isDead = false;
        // 리셋 시점에 소환되어 있던 적도 깔끔하게 지우기
        if (_spawnedEnemy != null) Despawn();
    }
}