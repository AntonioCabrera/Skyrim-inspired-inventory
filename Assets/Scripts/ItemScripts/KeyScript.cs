using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : ItemScript
{
    public override void InventoryUseAction()
    {
        Debug.Log(ScriptableItem.ItemName + " used, this item type is:" + ScriptableItem.ItemType.ToString() + ".And it has everything to work out its unique logic");
    }
}
