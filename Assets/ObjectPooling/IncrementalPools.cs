using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementalPools : MonoBehaviour
{
    public static IncrementalPools Instance;    //Static accessor declaration

    public List<GameObjectPool> pools;          //Editor exposed pool list for configuration


    private Dictionary<string, Queue<GameObject>> pooledObjectQueues;       //Pooled objects Queues
    private Dictionary<string, GameObjectPool> gameObjectPools;             //GameObjectPool list
    private GameObject currentInstance;           //Reused field for memory alloc
    private GameObjectPool currentPool;           //Reused field for memory alloc
    private IPooleableObject IpooledObjectGet;    //Reused field for memory alloc
    private IPooleableObject IpooledObjectReturn; //Reused field for memory alloc


    #region Instance and pools initialization
    private void Awake()
    {
        Instance = this;
        PopulatePools();
    }

    private void PopulatePools()
    {
        pooledObjectQueues = new Dictionary<string, Queue<GameObject>>();
        gameObjectPools = new Dictionary<string, GameObjectPool>();
        foreach (GameObjectPool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();
            GameObject obj = null;
            pool.PooledObjectParent = new GameObject(pool.PoolType.ToString() + " pool");
            pool.PooledObjectParent.transform.SetParent(transform);

            for (int i = 0; i < pool.BaseSize; i++)
            {
                obj = Instantiate(pool.Prefab);
                obj.transform.SetParent(pool.PooledObjectParent.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
            gameObjectPools.Add(pool.PoolType.ToString(), pool);
            pooledObjectQueues.Add(pool.PoolType.ToString(), objectPool);

        }
    }
    #endregion

    /// <summary>
    /// Use "PoolTypes.TYPE.ToString()" as poolTag
    /// </summary>
    public GameObject GetObjectFromPool(string poolTag)
    {
        currentPool = gameObjectPools[poolTag];

        //If pool queue is empty creates new instances and asigns the last added instance
        if (pooledObjectQueues[poolTag].Count == 0)
        {
            for (int i = 0; i < currentPool.IncrementalSteps; i++)
            {
                currentInstance = Instantiate(currentPool.Prefab);
                currentInstance.transform.SetParent(currentPool.PooledObjectParent.transform);
                ReturnObjectToPool(poolTag, currentInstance);
            }
        }

        currentInstance = pooledObjectQueues[poolTag].Dequeue();
        currentInstance.SetActive(true);

        IpooledObjectGet = currentInstance.GetComponent<IPooleableObject>();
        if (IpooledObjectGet != null)
        {
            IpooledObjectGet.OnGetFromPool();
        }
        IpooledObjectGet = null;

        return currentInstance;

    }

    /// <summary>
    /// Use "PoolTypes.TYPE.ToString()" as poolTag
    /// </summary>
    public void ReturnObjectToPool(string poolTag, GameObject gameObjectToAdd)
    {
        pooledObjectQueues[poolTag].Enqueue(gameObjectToAdd);

        IpooledObjectReturn = currentInstance.GetComponent<IPooleableObject>();
        if (IpooledObjectReturn != null)
        {
            IpooledObjectReturn.OnReturnToPool();
        }
        IpooledObjectReturn = null;
        gameObjectToAdd.transform.SetParent(gameObjectPools[poolTag].PooledObjectParent.transform);

        gameObjectToAdd.SetActive(false);
    }



}
