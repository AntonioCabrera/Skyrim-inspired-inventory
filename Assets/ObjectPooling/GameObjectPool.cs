using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectPool
{
    [Tooltip("PoolTypes must be defined in the GameObjectPool class.")]
    public PoolTypes PoolType;
    [Tooltip("GameObject to pool")]
    public GameObject Prefab;
    [Tooltip("Initial size of the pool")]
    public int BaseSize;
    [Tooltip("How many new objects will be instantiated if pool needs to grow")]
    public int IncrementalSteps;

    [HideInInspector]
    public GameObject PooledObjectParent;       //To keep clean hierarchy each pool has its own parent
    
}

/// <summary>
/// To minimize string casts use ToString() to pass the enum keyname to the IncrementalPools get and return methods
/// </summary>
public enum PoolTypes
{
    InventoryItemButton,
    InventoryBottomBarButton
}
