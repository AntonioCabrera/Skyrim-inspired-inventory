using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{

    public static InventoryManager Instance;
    public float MaxCarryCapacity;
    public GameObject DropableArea;

    [HideInInspector]
    public float CurrentCarryCapacity;
    [HideInInspector]
    public float CurrentGold;
    [HideInInspector]
    public Dictionary<InventoryItemTypes, List<ItemScript>> InventoryDictionary;


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
        InitializeInventoryDictionaries();
    }


    public void InitializeInventoryDictionaries()
    {
        int inventoryItemTypesLenght = Enum.GetNames(typeof(InventoryItemTypes)).Length;

        InventoryDictionary = new Dictionary<InventoryItemTypes, List<ItemScript>>();
        for (int i = 0; i < inventoryItemTypesLenght; i++)
        {
            List<ItemScript> list = new List<ItemScript>();
            InventoryDictionary.Add((InventoryItemTypes)i, list);
        }
    }

    public bool AddItemToInventory(ItemScript item)
    {
        if (item.ScriptableItem.ItemWeight + CurrentCarryCapacity < MaxCarryCapacity)
        {
            InventoryDictionary[item.ScriptableItem.ItemType].Add(item);
            CurrentCarryCapacity += item.ScriptableItem.ItemWeight;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void RemoveItemFromInventory(ItemScript item)
    {
        CurrentCarryCapacity -= item.ScriptableItem.ItemWeight;
        foreach (var ownedItem in InventoryDictionary[item.ScriptableItem.ItemType])
        {
            if (ownedItem.ScriptableItem.ItemName.Equals(item.ScriptableItem.ItemName))
            {
                InventoryDictionary[item.ScriptableItem.ItemType].Remove(ownedItem);
                return;
            }
        }
        UIManager.Instance.InventoryCarryWeightText.text = "Current / Max Weight: " + CurrentCarryCapacity.ToString("F2") + " / " + MaxCarryCapacity.ToString("F2");

    }

    public void AddGoldCurrency(int amount)
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
            if (AddItemToInventory(RaycastManager.Instance.CurrentObjectRaycasted))
            {
                Destroy(RaycastManager.Instance.CurrentObjectRaycasted.gameObject);
            }
        }

    }

    public void UseItem(ItemScript item)
    {
        item.InventoryUseAction();
    }

    public void DropItem(ItemScript item)
    {
        GameObject dropped = Instantiate(item.ScriptableItem.ItemPrefab, DropableArea.transform, false);
        dropped.transform.position += new Vector3(UnityEngine.Random.insideUnitCircle.x / 2, 0, UnityEngine.Random.insideUnitCircle.y / 2);
        dropped.transform.SetParent(null);
        RemoveItemFromInventory(item);
        UIManager.Instance.RepeatedItemControlDecrease(item);
        UIManager.Instance.UpdateItemButtonAmount(item);
    }



    public void EquipItem(ItemScript item)
    {
        item.InventoryUseAction();
    }

}
