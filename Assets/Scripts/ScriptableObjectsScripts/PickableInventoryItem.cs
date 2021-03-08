using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PickableInventoryItem : ScriptableObject
{

    public InventoryItemTypes ItemType;
    public string ItemName;
    public float ItemWeight;
    public string ItemDescription;
    public bool ItemIsQuestItem;
    public bool ItemIsEquipable;
    public bool ItemIsUsable;
    public string ConsumableTextAction;
    public GameObject ItemObjectVisualizationInUI;
    public GameObject ItemPrefab;
    public int ItemValue;
}

public enum InventoryItemTypes
{
    Weapon,
    Apparel,
    Potion,
    Food,
    Ingredient,
    Book,
    Key,
    Misc,
    GoldCurrency
}
