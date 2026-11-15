using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class TestSpawner : MonoBehaviour
{
    private void Start()
    {
        Utils.Delay(1F, () =>
        {
            TestSpawn();
        });

    }

    [SerializeField] private GameObject spawnPrefab;
    [SerializeField] private Transform startPos;
    [SerializeField] private float xInterval = 10f;
    [SerializeField] private float zInterval = 10f;

    [SerializeField] private int maxXCount = 5;
    private int currentXCount = 0;
    private int currentZCount = 0;

    private void TestSpawn()
    {
        GameObject prefab = Managers.Data.GetData<GameObject>("Player");
        GameObject playerObject = GameObject.Instantiate(prefab);
        PlayerUnit playerUnit = playerObject.GetComponent<PlayerUnit>();
        UnitData unitData = Managers.Data.GetData<UnitData>("Player Data");

        playerUnit.Init(unitData);
        playerUnit.transform.position = new Vector3(transform.position.x, 0.5f, transform.position.z);

        Managers.Event.NotifyEvent<PlayerSpawnEvent>(new PlayerSpawnEvent { player = playerUnit });


        List<ConsumableData> consumableDatas = Managers.Data.GetDatas<ConsumableData>();
        CreateModel(consumableDatas.Cast<ItemData>().ToList());

        List<RangedWeaponData> weaponDatas = Managers.Data.GetDatas<RangedWeaponData>();
        CreateModel(weaponDatas.Cast<ItemData>().ToList());

        List<ArmorData> armorDatas = Managers.Data.GetDatas<ArmorData>();
        CreateModel(armorDatas.Cast<ItemData>().ToList());

        List<ResourceData> resourceDatas = Managers.Data.GetDatas<ResourceData>();
        CreateModel(resourceDatas.Cast<ItemData>().ToList());


    }


    private void CreateModel(List<ItemData> itemDatas)
    {
        for (int i = 0; i < itemDatas.Count; i++)
        {
            if (itemDatas[i].ModelPrefab == null) continue;
            Vector3 nextPos = GetNextPos();
            GameObject name = Instantiate(spawnPrefab);
            name.transform.position = nextPos + new Vector3(0,0,-0.3f);
            name.GetComponentInChildren<TMP_Text>().text = itemDatas[i].ItemName + "\nID : " + itemDatas[i].ItemID;
            GameObject model = Instantiate(itemDatas[i].ModelPrefab, nextPos, Quaternion.identity);
            model.transform.Rotate(0, 90, 0);
        }
    }


    private Vector3 GetNextPos()
    {
        Vector3 newPos = new Vector3(startPos.position.x + (xInterval * currentXCount), 0.5f, startPos.position.z + (zInterval * currentZCount));
        currentXCount++;
        if (currentXCount >= maxXCount)
        {
            currentXCount = 0;
            currentZCount++;
        }

        return newPos;
    }
}
