using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitFactory
{

    public GameObject CreateUnit(UnitData data, Vector3 pos, EnemySpawner spawner = null)
    {

        if (data is EnemyData enemyData)//패턴 매칭  확인 : data(부모타입)가 enemyData(자식타입)가 맞는지 확인 (true, false 반환)
                                        //변환 및 할당 :  맞다면 EnemyData타입으로 형변환 해서 enemyData라는 새로운 변수에 삽입
        {
            GameObject model = data.modelPrefab;
            if (model == null) return null;



            // 뼈대 생성
            GameObject root = GameObject.Instantiate(model, pos, Quaternion.identity);
            // 디버깅을 위한 이름 설정
            root.name = $"{data.unitName}_{root.GetInstanceID()}";
            //컴포넌트 추가
            Enemy enemy = root.AddComponent<Enemy>();
            //스포너 연결
            if (spawner != null)
                enemy.SetSpawner(spawner);
            //데이터 초기화
            enemy.Init(enemyData);

            //공용 러너 장착
            EnemyBTRunner runner = root.AddComponent<EnemyBTRunner>();
            //공용 러너 노드 조립
            runner.Init(enemy, enemyData.aiStrategy);
            return root;
        }
        /*
        else if (data is PlayerData)
        {
            unit = root.AddComponent<Player>();
        }
        */
        return null;
    }
}
