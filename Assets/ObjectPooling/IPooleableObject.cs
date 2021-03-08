using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPooleableObject
{
    /// <summary>
    /// Called right after the object has been SetActive(true)
    /// </summary>
    void OnGetFromPool();

    /// <summary>
    /// Called right before the object is SetActive(false)
    /// </summary>
    void OnReturnToPool();
}
