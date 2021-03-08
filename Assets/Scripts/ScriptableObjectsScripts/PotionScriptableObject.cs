using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "PotionScriptableObject", menuName = "ScriptableObjects/Potion", order = 2)]
public class PotionScriptableObject : PickableInventoryItem
{
    public string ProvisionalPotionEffect;
    public string PotionText;
}
