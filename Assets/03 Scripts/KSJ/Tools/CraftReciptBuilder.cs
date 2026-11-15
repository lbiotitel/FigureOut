using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CraftReciptBuilder : MonoBehaviour
{
    [Header("---레시피 SO 생성---")]
    [SerializeField] private List<ItemData> itemData;
    [SerializeField] private string folderPath;

    #if UNITY_EDITOR
    [ContextMenu("레시피 SO 데이터 새로고침")]
    public void SetItemRecipt()
    {

        folderPath = "Assets/08 Datas/Items/ItemRecipe";

        for (int i = 0; i < itemData.Count; i++)
        {
            folderPath = ($"Assets/08 Datas/Items/ItemRecipt/ItemRecipt_{itemData[i].ItemID}.asset");
            ItemCraftRecipt so = ScriptableObject.CreateInstance<ItemCraftRecipt>();
            so.CraftItemID = itemData[i].ItemID;

            AssetDatabase.CreateAsset(so, folderPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
#endif
}
