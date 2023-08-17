using System.Collections.Generic;
using UnityEngine.Pool;
using Unity.Netcode;
using UnityEngine;
using System;

public class NetworkObjectPool : NetworkBehaviour
{
    [SerializeField] List<PoolConfigObject> prefabList;
    Dictionary<GameObject, ObjectPool<NetworkObject>> pooledObjects = new Dictionary<GameObject, ObjectPool<NetworkObject>>();

    //

    // Registers all objects in prefabList to the cache.
    public override void OnNetworkSpawn()
    {
        for (int i = 0; i < prefabList.Count; i++)
        {
            CreateNetworkObjectPools(prefabList[i].prefab, prefabList[i].poolSize);
        }
    }
    //
    void CreateNetworkObjectPools(GameObject prefab, int poolSize)
    {
        NetworkObject CreateFunc() => Instantiate(prefab).GetComponent<NetworkObject>();
        void ActionOnGet(NetworkObject networkObject) => networkObject.gameObject.SetActive(true);
        void ActionOnRelease(NetworkObject networkObject) => networkObject.gameObject.SetActive(false);
        void ActionOnDestroy(NetworkObject networkObject) => Destroy(networkObject.gameObject);
        //
        // Create the pool and release objects to make them available for use
        pooledObjects[prefab] = new ObjectPool<NetworkObject>(CreateFunc, ActionOnGet, ActionOnRelease, ActionOnDestroy, defaultCapacity: poolSize);
        List<NetworkObject> cachedObjects = new List<NetworkObject>();
        for (int i = 0; i < poolSize; i++)
        {
            cachedObjects.Add(pooledObjects[prefab].Get());
        }
        foreach (var networkObject in cachedObjects)
        {
            ReturnNetworkObject(networkObject, prefab);
        }
        // Register Spawn handlers
        NetworkManager.Singleton.PrefabHandler.AddHandler(prefab, new PooledPrefabInstanceHandler(prefab, this));
    }
    //
    public void ReturnNetworkObject(NetworkObject networkObject, GameObject prefab) => pooledObjects[prefab].Release(networkObject);
    public NetworkObject GetNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation) => pooledObjects[prefab].Get();
    public void ReuseNetworkObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        NetworkObject networkObject = pooledObjects[prefab].Get();
        Transform form = networkObject.transform;
        form.position = position;
        form.rotation = rotation;
    }
    //
    public override void OnNetworkDespawn()
    {
        pooledObjects.Clear();
        for (int i = 0; i < prefabList.Count; i++)
        {
            // Unregister spawn handlers
            NetworkManager.Singleton.PrefabHandler.RemoveHandler(prefabList[i].prefab);
        }
    }
}

[Serializable]
struct PoolConfigObject
{
    public GameObject prefab;
    public int poolSize;
}
