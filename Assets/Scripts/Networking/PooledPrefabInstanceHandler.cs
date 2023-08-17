using Unity.Netcode;
using UnityEngine;

class PooledPrefabInstanceHandler : INetworkPrefabInstanceHandler
{
    GameObject obj;
    NetworkObjectPool networkPool;
    //
    public PooledPrefabInstanceHandler(GameObject newObj, NetworkObjectPool newPool)
    {
        obj = newObj;
        networkPool = newPool;
    }
    //
    void INetworkPrefabInstanceHandler.Destroy(NetworkObject networkObject) => networkPool.ReturnNetworkObject(networkObject, obj);
    NetworkObject INetworkPrefabInstanceHandler.Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation)
    {
        return networkPool.GetNetworkObject(obj, position, rotation);
    }
}
