using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "IngredientScriptableObject", menuName = "ScriptableObjects/Ingredient", order = 5)]
public class IngredientScriptableObject : PickableInventoryItem
{
    public bool CanBeIngested;
}
