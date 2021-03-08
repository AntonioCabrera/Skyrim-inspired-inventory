using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance;
    public float MaxCarryCapacity;

    [HideInInspector]
    public float CurrentCarryCapacity;
    [HideInInspector]
    public float CurrentGold;
    [HideInInspector]
    public Dictionary<InventoryItemTypes, List<PickableInventoryItem>> InventoryDictionary;

    private void Awake()
    {
        Instance = this;
        InitializeInventoryDictionaries();
    }


    public void InitializeInventoryDictionaries()
    {
        int inventoryItemTypesLenght = Enum.GetNames(typeof(InventoryItemTypes)).Length;

        InventoryDictionary = new Dictionary<InventoryItemTypes, List<PickableInventoryItem>>();
        for (int i = 0; i < inventoryItemTypesLenght; i++)
        {
            List<PickableInventoryItem> list = new List<PickableInventoryItem>();
            InventoryDictionary.Add((InventoryItemTypes)i, list);
        }
    }

    public bool AddItemToInventory(PickableInventoryItem item)
    {
        if (item.ItemWeight + CurrentCarryCapacity < MaxCarryCapacity)
        {
            InventoryDictionary[item.ItemType].Add(item);
            CurrentCarryCapacity += item.ItemWeight;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddGoldCurrency (int amount)
    {
        CurrentGold += amount;
    }

    public bool CanCarryThisNewWeight(float newWeight)
    {
        if (newWeight + CurrentCarryCapacity < MaxCarryCapacity)
        {
            return true;
        }
        else
        {
            return false;
        }

    }

    public void TryPickUpCurrentItem()
    {
        if (RaycastManager.Instance.CurrentObjectRaycasted != null)
        {
            if (RaycastManager.Instance.CurrentObjectRaycasted.ScriptableItem.ItemType == InventoryItemTypes.GoldCurrency)
            {
                GoldCurrencyScriptableObject g = (GoldCurrencyScriptableObject)RaycastManager.Instance.CurrentObjectRaycasted.ScriptableItem;
                AddGoldCurrency(g.currencyAmmount);
            }
            if (AddItemToInventory(RaycastManager.Instance.CurrentObjectRaycasted.ScriptableItem))
            {
                Destroy(RaycastManager.Instance.CurrentObjectRaycasted.gameObject);
            }
        }

    }

    public void UseItem (PickableInventoryItem item)
    {
        if(item.ItemType == InventoryItemTypes.Book)
        {
            //todo implementation for UIManager onClick
        }
    }

    public void DropItem(PickableInventoryItem item)
    {
        //todo implementation for UIManager onClick

    }

    public void EquipItem(PickableInventoryItem item)
    {
        //todo implementation for UIManager onClick

    }

}
