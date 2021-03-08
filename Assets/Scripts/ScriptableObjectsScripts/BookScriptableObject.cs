using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "BookScriptableObject", menuName = "ScriptableObjects/Book", order = 6)]
public class BookScriptableObject : PickableInventoryItem
{
    public bool IsReadable;
    public string ReadableText;
}
