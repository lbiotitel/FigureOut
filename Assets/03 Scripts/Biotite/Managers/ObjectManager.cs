using System.Collections.Generic;
using UnityEngine;


//오브젝트를 만드는 역할을 위임 받습니다.
public class ObjectManager : MonoBehaviour, IManager
{
    private List<DropItem> dropItems;
    private List<EnemySpawner> _activeSpawners = new List<EnemySpawner>();

    public PoolingSystem PoolingSystem { get; private set; }

    public void Init()
    {
        dropItems = new List<DropItem>();

        PoolingSystem = GetComponentInChildren<PoolingSystem>();
        PoolingSystem.Init();
    }

    public Projectile CreateProjectile(ProjectileData projectileData, Unit ownUnit, Vector3 startPos, Vector3 targetPos, Quaternion rotation ,LayerMask layerMask)
    {
        GameObject projectilePrefab = projectileData.ProjectilePrefab;

        GameObject go = PoolingSystem.GetObject(projectilePrefab, startPos, rotation);
        Projectile projectile = go.GetComponent<Projectile>();

        projectile.Init(projectileData, ownUnit, startPos, targetPos, layerMask);

        return projectile;
    }


    public GameObject CreatePaticleEffect(string effectName,Vector3 position, Quaternion rotation)
    {
        GameObject particlePrefab = Managers.Data.GetData<GameObject>(effectName);
        GameObject particleObject = PoolingSystem.GetObject(particlePrefab,position,rotation);
        particleObject.transform.position = position;

        return particleObject;
    }


    public DropItem CreateDropObject(ItemData itemData, int amount, Vector3 pos, Quaternion quaternion)
    {
        GameObject dropPrefab = Managers.Data.GetData<GameObject>(KeyDefines.ResourceKey.DROP_ITEM_PREFAB);
        GameObject dropItemObject = Instantiate(dropPrefab, pos, quaternion);
        DropItem dropItem = dropItemObject.GetComponent<DropItem>();
        dropItem.Init(itemData, amount);
        dropItems.Add(dropItem);

        SetObject(dropItemObject, pos, quaternion);

        return dropItem;
    }


    public DamageText CreateDamageText(float damage, EHitType hitType, Vector3 pos, Quaternion rotation)
    {
        GameObject damageTextPrefab = Managers.Data.GetData<GameObject>(KeyDefines.ResourceKey.DAMAGE_TEXT_PREFAB);
        GameObject damageText = PoolingSystem.GetObject(damageTextPrefab, pos, rotation);
        DamageText res = damageText.GetComponent<DamageText>();
        res.Init(damage, hitType);

        return res;
    }


    private void SetObject(GameObject obj, Vector3 pos, Quaternion quaternion)
    {
        obj.transform.position = pos;
        obj.transform.rotation = quaternion;
    }

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

        else if (data is PlayerData)
        {
            GameObject playerObject = Instantiate(data.modelPrefab);
            PlayerUnit playerUnit = playerObject.GetComponent<PlayerUnit>();

            playerUnit.Init(data);
            playerUnit.transform.position = pos;

            return playerObject;
        }

        return null;
    }

    public void CreateEnemySpawner(EnemyData enemyData, Vector3 pos)//에너미 스포너들을 배치한당!
    {
        GameObject spawnerPrefab = Managers.Data.GetData<GameObject>("EnemySpawner");
        GameObject spawnerObj = Instantiate(spawnerPrefab, pos, Quaternion.identity, transform);

        EnemySpawner spawner = spawnerObj.GetComponent<EnemySpawner>();
        spawner.Initialize(enemyData);

        _activeSpawners.Add(spawner);
    }

    public void ResetAllEnemies()
    {
        Debug.Log("모든 에너미 초기화 및 재배치 시작");
        foreach (var spawner in _activeSpawners)
        {
            spawner.ResetSpawner();
        }
    }


    public void Release()
    {
        throw new System.NotImplementedException();
    }


}

// JSON 파싱용 클래스
[System.Serializable]
public class EnemySpawnList
{
    public List<EnemySpawnInfo> enemies;
}

[System.Serializable]
public class EnemySpawnInfo
{
    public string type;
    public float posX;
    public float posY;
    public float posZ;
}