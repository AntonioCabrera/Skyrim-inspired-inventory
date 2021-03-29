using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Inventory UI References")]
    public GameObject InventoryUI;
    public Transform InventoryTypePanelContent;
    public GameObject InventoryItemPanel;
    public GameObject InventoryItemInfoVisualizationPanel;
    public Transform InventoryItemPanelContent;
    public Transform InventoryBottomBarButtonsContent;
    public Transform InventoryItemInfoVisualizationObjectTransform;
    public TextMeshProUGUI InventoryGoldCurrencyText;
    public TextMeshProUGUI InventoryCarryWeightText;
    public TextMeshProUGUI InventoryItemInfoName;
    public TextMeshProUGUI InventoryItemInfoValue;
    public TextMeshProUGUI InventoryItemInfoWeight;
    public TextMeshProUGUI InventoryItemInfoItemText;
    public TextMeshProUGUI InventoryItemInfoDamageOrArmor;

    [Header("Pickable object UI References")]
    public GameObject PickItemUI;
    public TextMeshProUGUI PickItemNameText;
    public TextMeshProUGUI PickItemValueText;
    public TextMeshProUGUI PickItemWeightText;
    public TextMeshProUGUI PickItemInputHintText;

    [Header("Pickable object color")]
    public Color PickableColor;
    public Color NonPickableColor;


    private List<GameObject> inventoryTypesListButtonPrefabs;
    private List<GameObject> inventoryBottomBarButtons;
    public Dictionary<string, GameObject> InventoryItemsListButtonPrefabs;
    private GameObject currentInventoryItemInfoVisualization;

    [HideInInspector]
    public Dictionary<string, int> RepeatedItemsNumberControl;
    [HideInInspector]
    public bool inventoryIsReadyToOpenAgain = true;

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
        inventoryBottomBarButtons = new List<GameObject>();
        inventoryTypesListButtonPrefabs = new List<GameObject>();
        InventoryItemsListButtonPrefabs = new Dictionary<string, GameObject>();
        RepeatedItemsNumberControl = new Dictionary<string, int>();
    }

    public void OpenMainInventory()
    {
        LoadInventoryUI();
        InventoryUI.SetActive(true);
    }

    public void CloseMainInventory()
    {
        inventoryIsReadyToOpenAgain = false;
        InventoryItemInfoVisualizationPanel.SetActive(false);
        InventoryItemPanel.SetActive(false);
        InventoryUI.SetActive(false);

        foreach (var inventoryTypeButton in inventoryTypesListButtonPrefabs)
        {
            inventoryTypeButton.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryItemButton.ToString(), inventoryTypeButton);
        }
        inventoryIsReadyToOpenAgain = true;
    }

    public void LoadInventoryUI()
    {
        UpdateGoldCurrencyUIText();
        UpdateInventoryTypePanel();
        UpdateCarryCapacityText();
        ReturnBottomBarButtonsToPool();
    }

    public void UpdateCarryCapacityText()
    {
        InventoryCarryWeightText.text = "Current / Max Weight: " + InventoryManager.Instance.CurrentCarryCapacity.ToString("F2") + " / " + InventoryManager.Instance.MaxCarryCapacity.ToString("F2");
    }

    public void UpdateGoldCurrencyUIText()
    {
        InventoryGoldCurrencyText.text = "Gold: " + InventoryManager.Instance.CurrentGold.ToString();
    }

    public void UpdateInventoryTypePanel()
    {
        inventoryTypesListButtonPrefabs.Clear();
        int inventoryTypesButtonCountIndex = 0;
        for (int i = 0; i < InventoryManager.Instance.InventoryDictionary.Count; i++)
        {
            if (InventoryManager.Instance.InventoryDictionary[(InventoryItemTypes)i].Count != 0 && InventoryManager.Instance.InventoryDictionary[(InventoryItemTypes)i][0].ScriptableItem.ItemType != InventoryItemTypes.GoldCurrency)
            {
                inventoryTypesListButtonPrefabs.Add(IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryItemButton.ToString()));
                GameObject go = inventoryTypesListButtonPrefabs[inventoryTypesButtonCountIndex];
                inventoryTypesButtonCountIndex++;

                go.SetActive(true);
                go.GetComponentInChildren<TextMeshProUGUI>().text = ((InventoryItemTypes)i).ToString();
                SetInventoryTypeButtonListener(go.GetComponentInChildren<Button>().onClick, InventoryManager.Instance.InventoryDictionary[(InventoryItemTypes)i]);
            }
        }

        if (inventoryTypesListButtonPrefabs.Count != 0)
        {
            inventoryTypesListButtonPrefabs = inventoryTypesListButtonPrefabs.OrderBy(go => go.GetComponentInChildren<TextMeshProUGUI>().text).ToList();
            foreach (var inventoryTypeButton in inventoryTypesListButtonPrefabs)
            {
                inventoryTypeButton.transform.SetParent(InventoryTypePanelContent);
            }
        }

    }

    public void SetInventoryTypeButtonListener(Button.ButtonClickedEvent onClick, List<ItemScript> itemList)
    {
        itemList = itemList.OrderBy(go => go.ScriptableItem.name).ToList();
        onClick.AddListener(delegate { OnInventoryTypeButtonClick(itemList); });
    }

    public void OnInventoryTypeButtonClick(List<ItemScript> itemList)
    {
        InventoryItemInfoVisualizationPanel.SetActive(false);
        ReturnBottomBarButtonsToPool();

        foreach (var inventoryItemButton in InventoryItemsListButtonPrefabs)
        {
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryItemButton.ToString(), inventoryItemButton.Value);
        }
        InventoryItemsListButtonPrefabs.Clear();

        RepeatedItemsNumberControl.Clear();
        foreach (var item in itemList)
        {
            if (RepeatedItemsNumberControl.ContainsKey(item.ScriptableItem.ItemName))
            {
                RepeatedItemsNumberControl[item.ScriptableItem.ItemName]++;
                UpdateItemButtonAmount(item);
            }
            else
            {
                GameObject pooledButton = IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryItemButton.ToString());
                pooledButton.GetComponentInChildren<TextMeshProUGUI>().text = item.ScriptableItem.ItemName;
                pooledButton.transform.SetParent(InventoryItemPanelContent);
                InventoryItemsListButtonPrefabs.Add(item.ScriptableItem.ItemName, pooledButton);
                RepeatedItemsNumberControl.Add(item.ScriptableItem.ItemName, 1);
                SetInventoryItemButtonListener(pooledButton.GetComponentInChildren<Button>().onClick, item);
            }
        }
        InventoryItemPanel.SetActive(true);

    }


    public void SetInventoryItemButtonListener(Button.ButtonClickedEvent onClick, ItemScript item)
    {
        onClick.AddListener(delegate { OnInventoryItemClick(item); });
    }

    public void OnInventoryItemClick(ItemScript item)
    {

        if (currentInventoryItemInfoVisualization != null)
        {
            Destroy(currentInventoryItemInfoVisualization);
        }

        currentInventoryItemInfoVisualization = Instantiate(item.ScriptableItem.ItemObjectVisualizationInUI, InventoryItemInfoVisualizationObjectTransform, false);


        if (item.ScriptableItem.ItemIsEquipable)
        {
            UpdateInventoryItemInfoDamageOrArmorText(item);

        }
        else
        {
            InventoryItemInfoDamageOrArmor.text = string.Empty;
        }

        InventoryItemInfoName.text = item.ScriptableItem.ItemName;
        InventoryItemInfoValue.text = "Value: " + item.ScriptableItem.ItemValue.ToString();
        InventoryItemInfoWeight.text = "Weight: " + item.ScriptableItem.ItemWeight.ToString();
        InventoryItemInfoItemText.text = item.ScriptableItem.ItemDescription;
        InventoryItemInfoVisualizationPanel.SetActive(true);
    }
    public void RepeatedItemControlDecrease(ItemScript item)
    {
        if (RepeatedItemsNumberControl[item.ScriptableItem.ItemName] >= 1)
        {
            RepeatedItemsNumberControl[item.ScriptableItem.ItemName]--;
        }
    }

    public void UpdateItemButtonAmount(ItemScript item)
    {
        GameObject pooledButton = InventoryItemsListButtonPrefabs[item.ScriptableItem.ItemName];
        if (RepeatedItemsNumberControl[item.ScriptableItem.ItemName] == 0)
        {
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryItemButton.ToString(), pooledButton);
            InventoryItemInfoVisualizationPanel.SetActive(false);
            return;
        }
        pooledButton.GetComponentInChildren<TextMeshProUGUI>().text = item.ScriptableItem.ItemName + "(" + RepeatedItemsNumberControl[item.ScriptableItem.ItemName] + ")";
    }

    public void UpdateInventoryItemInfoDamageOrArmorText(ItemScript item)
    {
        if (item.ScriptableItem.ItemType == InventoryItemTypes.Apparel)
        {
            ApparelScriptableObject s = (ApparelScriptableObject)item.ScriptableItem;
            InventoryItemInfoDamageOrArmor.text = "Armor:" + s.Armor;
        }
        else
        {
            WeaponScriptableObject s = (WeaponScriptableObject)item.ScriptableItem;
            InventoryItemInfoDamageOrArmor.text = "Damage:" + s.Damage;
        }
    }

    public void ReturnBottomBarButtonsToPool()
    {
        foreach (var inventoryBottomBarButton in inventoryBottomBarButtons)
        {
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryBottomBarButton.ToString(), inventoryBottomBarButton);
        }
        inventoryBottomBarButtons.Clear();
    }

    public void TurnOnPickItemText(ItemScript item, bool canCarryTheItem)
    {
        PickItemNameText.text = item.ScriptableItem.ItemName;
        PickItemValueText.text = "Value: " + item.ScriptableItem.ItemValue.ToString();
        PickItemWeightText.text = "Weight: " + item.ScriptableItem.ItemWeight.ToString();
        if (canCarryTheItem)
        {

            PickItemInputHintText.color = PickableColor;
            PickItemInputHintText.text = InputManager.Instance.UseKey.ToString() + " - Take";
        }
        else
        {
            PickItemInputHintText.color = NonPickableColor;
            PickItemInputHintText.text = "You can't carry this weight";
        }

        PickItemUI.SetActive(true);
    }

    public void TurnOffPickItemText()
    {
        PickItemUI.SetActive(false);
    }


}
