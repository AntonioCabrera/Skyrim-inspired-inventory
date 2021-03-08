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
    private Dictionary<string, int> repeatedItemsNumberControl;
    private Dictionary<string, GameObject> inventoryItemsListButtonPrefabs;
    private GameObject currentInventoryItemInfoVisualization;

    [HideInInspector]
    public bool inventoryIsReadyToOpenAgain = true;

    private void Awake()
    {
        Instance = this;
        inventoryBottomBarButtons = new List<GameObject>();
        inventoryTypesListButtonPrefabs = new List<GameObject>();
        inventoryItemsListButtonPrefabs = new Dictionary<string, GameObject>();
        repeatedItemsNumberControl = new Dictionary<string, int>();
    }

    public void OpenMainInventory()
    {
        UpdateInventoryUI();
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

    public void UpdateInventoryUI()
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

    public void UpdateInventoryTypePanel()
    {
        inventoryTypesListButtonPrefabs.Clear();
        int inventoryTypesButtonCountIndex = 0;
        for (int i = 0; i < InventoryManager.Instance.InventoryDictionary.Count; i++)
        {
            if (InventoryManager.Instance.InventoryDictionary[(InventoryItemTypes)i].Count != 0 && InventoryManager.Instance.InventoryDictionary[(InventoryItemTypes)i][0].ItemType != InventoryItemTypes.GoldCurrency)
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

    public void UpdateGoldCurrencyUIText()
    {
        InventoryGoldCurrencyText.text = "Gold: " + InventoryManager.Instance.CurrentGold.ToString();
    }

    public void SetInventoryTypeButtonListener(Button.ButtonClickedEvent onClick, List<PickableInventoryItem> itemList)
    {
        itemList = itemList.OrderBy(go => go.name).ToList();
        onClick.AddListener(delegate { OnInventoryTypeButtonClick(itemList); });
    }

    public void OnInventoryTypeButtonClick(List<PickableInventoryItem> itemList)
    {
        InventoryItemInfoVisualizationPanel.SetActive(false);
        ReturnBottomBarButtonsToPool();

        foreach (var inventoryItemButton in inventoryItemsListButtonPrefabs)
        {
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryItemButton.ToString(), inventoryItemButton.Value);
        }
        inventoryItemsListButtonPrefabs.Clear();

        repeatedItemsNumberControl.Clear();
        foreach (var item in itemList)
        {
            if (repeatedItemsNumberControl.ContainsKey(item.ItemName))
            {
                GameObject pooledButton;
                repeatedItemsNumberControl[item.ItemName]++;
                inventoryItemsListButtonPrefabs.TryGetValue(item.ItemName, out pooledButton);
                pooledButton.GetComponentInChildren<TextMeshProUGUI>().text = item.ItemName + "(" + repeatedItemsNumberControl[item.ItemName] + ")";
            }
            else
            {
                GameObject pooledButton = IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryItemButton.ToString());
                pooledButton.GetComponentInChildren<TextMeshProUGUI>().text = item.ItemName;
                pooledButton.transform.SetParent(InventoryItemPanelContent);
                inventoryItemsListButtonPrefabs.Add(item.ItemName, pooledButton);
                repeatedItemsNumberControl.Add(item.ItemName, 1);
                SetInventoryItemButtonListener(pooledButton.GetComponentInChildren<Button>().onClick, item);
            }
        }
        InventoryItemPanel.SetActive(true);

    }

    public void SetInventoryItemButtonListener(Button.ButtonClickedEvent onClick, PickableInventoryItem item)
    {
        onClick.AddListener(delegate { OnInventoryItemClick(item); });
    }

    public void OnInventoryItemClick(PickableInventoryItem item)
    {

        if (currentInventoryItemInfoVisualization != null)
        {
            Destroy(currentInventoryItemInfoVisualization);
        }

        currentInventoryItemInfoVisualization = Instantiate(item.ItemObjectVisualizationInUI, InventoryItemInfoVisualizationObjectTransform, false);

        ReturnBottomBarButtonsToPool();
        GameObject currentBottomBarButton;
        if (item.ItemIsQuestItem == false)
        {
            currentBottomBarButton = IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryBottomBarButton.ToString());
            currentBottomBarButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { InventoryManager.Instance.DropItem(item); });//todo implementation
            currentBottomBarButton.GetComponentInChildren<TextMeshProUGUI>().text = "Drop";
            currentBottomBarButton.transform.SetParent(InventoryBottomBarButtonsContent);
            inventoryBottomBarButtons.Add(currentBottomBarButton);
        }
        if (item.ItemIsEquipable)
        {
            if (item.ItemType == InventoryItemTypes.Apparel)
            {
                ApparelScriptableObject s = (ApparelScriptableObject)item;
                InventoryItemInfoDamageOrArmor.text = "Armor:" +s.Armor;
            }
            else
            {
                WeaponScriptableObject s = (WeaponScriptableObject)item;
               InventoryItemInfoDamageOrArmor.text = "Damage:"+s.Damage;          
            }

            currentBottomBarButton = IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryBottomBarButton.ToString());
            currentBottomBarButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { InventoryManager.Instance.EquipItem(item); });//todo implementation
            currentBottomBarButton.GetComponentInChildren<TextMeshProUGUI>().text = "Equip";
            currentBottomBarButton.transform.SetParent(InventoryBottomBarButtonsContent);
            inventoryBottomBarButtons.Add(currentBottomBarButton);
        }
        else
        {
            InventoryItemInfoDamageOrArmor.text = string.Empty;
        }
        if (item.ItemIsUsable)
        {
            currentBottomBarButton = IncrementalPools.Instance.GetObjectFromPool(PoolTypes.InventoryBottomBarButton.ToString());
            currentBottomBarButton.GetComponentInChildren<Button>().onClick.AddListener(delegate { InventoryManager.Instance.UseItem(item); });//todo implementation
            currentBottomBarButton.GetComponentInChildren<TextMeshProUGUI>().text = item.ConsumableTextAction;
            currentBottomBarButton.transform.SetParent(InventoryBottomBarButtonsContent);
            inventoryBottomBarButtons.Add(currentBottomBarButton);
        }


        InventoryItemInfoName.text = item.ItemName;
        InventoryItemInfoValue.text = "Value: " + item.ItemValue.ToString();
        InventoryItemInfoWeight.text = "Weight: " + item.ItemWeight.ToString();
        InventoryItemInfoItemText.text = item.ItemDescription;
        InventoryItemInfoVisualizationPanel.SetActive(true);
    }

    public void ReturnBottomBarButtonsToPool()
    {
        foreach (var inventoryBottomBarButton in inventoryBottomBarButtons)
        {
            IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.InventoryBottomBarButton.ToString(), inventoryBottomBarButton);
        }
        inventoryBottomBarButtons.Clear();
    }

    public void TurnOnPickItemText(PickableInventoryItem item, bool canCarryTheItem)
    {
        PickItemNameText.text = item.ItemName;
        PickItemValueText.text = "Value: " + item.ItemValue.ToString();
        PickItemWeightText.text = "Weight: " + item.ItemWeight.ToString();
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
