using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu]
public class EvolutionRecipeData : ScriptableObject
{
    [SerializeField] private string recipeID;
    [SerializeField] private string recipeName;
    [SerializeField] private string revolutionEquipmentID;

    [SerializeField] private int requiredDollar;
    [SerializeField] private List<EevolutionRecipeRequiredData> recipeRequiredDatas;

    public string RecipeID => recipeID;
    public string RecipeName => recipeName;
    public int RequiredDollar => requiredDollar;    
    public string RevolutionEquipmentID => revolutionEquipmentID;
    public List<EevolutionRecipeRequiredData> RecipeRequiredDatas => recipeRequiredDatas;
}


[Serializable]
public struct EevolutionRecipeRequiredData
{
    public string itemID;
    public int amount;
}
