1- Open the GameObjectPool class and define PoolTypes values to be the name you want to give to each pool.

2- Add the IncrementalPools script to a GameObject in the scene and configure your pool/s.

3- To get an object from the pool calling:
IncrementalPools.Instance.GetObjectFromPool(PoolTypes.CubesFalling.ToString());

4- To return an object to the pool calling:
IncrementalPools.Instance.ReturnObjectToPool(PoolTypes.TYPE.ToString(), gameObjectToReturn);

Optional: IF each pooled object instance object has a script on it, you can manage the pooled object instance
life cycle by implementing the IPooleableObject interface in there to define the OnGetFromPool() and OnReturnToPool() logics.
