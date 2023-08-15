using Unity.Netcode;
using UnityEngine;

public abstract class PoolObject : NetworkBehaviour
{
    public abstract void OnObjectReuse();
}
