using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class EnemyParsingManager : MonoBehaviour
{
    private void Start()
    {
        Managers.Event.AddEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }

    private void HandlePlayerSpawn(PlayerSpawnEvent data)
    {
        Init();
        Managers.Event.RemoveEvent<PlayerSpawnEvent>(HandlePlayerSpawn);
    }


    public void Init()
    {
        string filePath = Path.Combine(Application.streamingAssetsPath, "EnemySpawnData.json");
        if (!File.Exists(filePath)) return;

        string jsonData = File.ReadAllText(filePath);
        EnemySpawnList spawnList = JsonUtility.FromJson<EnemySpawnList>(jsonData);

        if (spawnList == null || spawnList.enemies == null) return;//JSON 파일이 비어있거나 형식이 잘못되었을 때를 위한 방비

        foreach (EnemySpawnInfo info in spawnList.enemies)
        {
            EnemyData enemyData = Managers.Data.GetData<EnemyData>(info.type);
            Vector3 vector3 = new Vector3(info.posX, info.posY, info.posZ);
            Managers.Object.CreateEnemySpawner(enemyData, vector3);
        }
    }
}
