using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemScript : MonoBehaviour
{
    public PickableInventoryItem ScriptableItem;
    public virtual void InventoryUseAction()
    {
        //to be implemented in child classes (WeaponScript...BookScript...etc)
    }
}
