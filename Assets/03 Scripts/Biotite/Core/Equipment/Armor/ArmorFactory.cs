using UnityEngine;

public class ArmorFactory : IEquipmentFactory
{
    private Unit ownUnit;

    public void Init(Unit ownUnit)
    {
        this.ownUnit = ownUnit;
    }


    public T Create<T>(ItemData itemData)
    {
        ArmorData armorData = itemData as ArmorData;

        GameObject parentObject = new GameObject(armorData.name);
        parentObject.layer = LayerMask.NameToLayer(KeyDefines.MaskKey.PLAYER_MASK);
        ArmorBase armorBase = null;

        if (ownUnit.unitType == UnitType.Player)
        {
            armorBase = parentObject.AddComponent<PlayerArmor>();
        }
        else if(ownUnit.unitType == UnitType.Enemy)
        { 
            armorBase = parentObject.AddComponent<EnemyArmor>();
        }

        armorBase.Init(ownUnit, itemData as EquipmentData);

        SetUp(parentObject, armorData);

        return parentObject.GetComponent<T>();
    }


    private void SetUp(GameObject parentObject, ArmorData armorData)
    {
        GameObject armorModel = GameObject.Instantiate(armorData.ModelPrefab);
        armorModel.name = "Model";
        armorModel.transform.SetParent(parentObject.transform);
        armorModel.transform.localPosition = armorData.GrabPos;
        armorModel.transform.rotation = ownUnit.transform.rotation;

        SetLayerRecursively(armorModel, parentObject.layer);
    }

    private void SetLayerRecursively(GameObject obj, int layer)
    {
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetLayerRecursively(child.gameObject, layer);
        }
    }


}
