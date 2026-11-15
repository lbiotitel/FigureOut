using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class JsonExporter : MonoBehaviour
{
    [MenuItem("Tools/Enemy Json Tool")]// 인스펙터 컴포넌트 우클릭 메뉴에 추가됨
    public static void Export()
    {
        EnemySpawnList root = new EnemySpawnList();
        root.enemies = new List<EnemySpawnInfo>();

        // 1. StreamingAssets 폴더 경로 설정
        string folderPath = Application.streamingAssetsPath;
        string filePath = Path.Combine(folderPath, "EnemySpawnData.json");
        // 2. 폴더가 없다면 자동으로 생성 (중요!)
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        // 씬에서 "Enemy" 태그를 가진 모든 오브젝트를 찾음
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("EnemySpawner");

        foreach (GameObject go in enemiesInScene)
        {
            if (go.TryGetComponent(out EnemySpawner enemySpawner))
            {
                EnemyData data = enemySpawner.data;
                if (data == null) continue;
                EnemySpawnInfo info = new EnemySpawnInfo();
                info.type = data.name;
                info.posX = go.transform.position.x;
                info.posY = go.transform.position.y;
                info.posZ = go.transform.position.z;

                root.enemies.Add(info);
            }
            else
            { 
                Logger.LogError($"EnemySpawner 컴포넌트가 없는 오브젝트 발견: {go.name}");   
            }
        }

        // JSON 문자열로 변환 (true: 보기 좋게 줄바꿈)
        string json = JsonUtility.ToJson(root, true);
        File.WriteAllText(filePath, json);

        // 유니티 엔진이 파일을 새로 고침해서 인식하게 함
#if UNITY_EDITOR
        UnityEditor.AssetDatabase.Refresh();
#endif

        Debug.Log($"저장 완료! 경로: {filePath}");
    }
}