using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CraftRecipt_", menuName = "Craft Recipt")]
public class ItemCraftRecipt : ScriptableObject
{
    [SerializeField] private string craftItemID;
    [SerializeField] private List<RequiredInRecipe> craftRecipt;

    public string CraftItemID
    {
        get { return craftItemID; }
        set { craftItemID = value; }
    }

    public List<RequiredInRecipe> CraftRecipt
    {
        get { return craftRecipt; }
        private set { craftRecipt = value; }
    }
}

[Serializable]
public class RequiredInRecipe
{
    public string reqItemID;
    [SerializeField] private int reqItemAmount;

    public int ReqItemAmount
    {
        get { return reqItemAmount; }
        private set { reqItemAmount = Mathf.Max(0, value); }
    }
}
