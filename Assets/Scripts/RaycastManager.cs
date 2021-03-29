using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastManager : MonoBehaviour
{
    public static RaycastManager Instance;
    public Transform PlayerCamera;

    private int layerMask = 1 << 8;
    private RaycastHit hit;

    [HideInInspector]
    public ItemScript CurrentObjectRaycasted;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(Instance);
        }
        else
        {
            Instance = this;
        }
    }

    public bool LookForPickablesRay()
    {
        if (Physics.Raycast(PlayerCamera.position, PlayerCamera.TransformDirection(Vector3.forward), out hit, 3, layerMask))
        {
            CurrentObjectRaycasted = hit.transform.gameObject.GetComponent<ItemScript>();
            return true;
        }
        else
        {
            CurrentObjectRaycasted = null;
            return false;
        }

    }

    void FixedUpdate()
    {

        if (LookForPickablesRay())
        {
            if (InventoryManager.Instance.CanCarryThisNewWeight(CurrentObjectRaycasted.ScriptableItem.ItemWeight))
            {
                InputManager.Instance.CanPickUpAnItem = true;
                UIManager.Instance.TurnOnPickItemText(CurrentObjectRaycasted, true);
            }
            else
            {
                UIManager.Instance.TurnOnPickItemText(CurrentObjectRaycasted, false);
            }

        }
        else
        {
            UIManager.Instance.TurnOffPickItemText();
            InputManager.Instance.CanPickUpAnItem = false;
        }

    }

}
