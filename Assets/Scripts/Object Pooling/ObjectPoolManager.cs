using System.Collections.Generic;
using System.Collections;
// using Unity.Netcode;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    const int impactPoolSize = 20;
    public GameObject bulletImpact;
    [HideInInspector] public int impactID;
    public Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

    //

    void Awake() => Initialize();
    void Initialize() => impactID = CreatePool(bulletImpact, impactPoolSize);
    int CreatePool(GameObject prefab, int poolSize)
    {
        int poolKey = prefab.GetInstanceID();
        //
        if (!DictionaryHasKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<ObjectInstance>());
            for (int i = 0; i < poolSize; i++)
            {
                ObjectInstance newObject = new ObjectInstance(Instantiate(prefab) as GameObject);
                poolDictionary[poolKey].Enqueue(newObject);
            }
        }
        else
        {
            RemovePoolIndex(poolKey);
            CreatePool(prefab, poolSize);
        }
        //
        return poolKey;
    }
    //
    bool DictionaryHasKey(int key) => poolDictionary.ContainsKey(key);
    public void RemovePoolIndex(int index) => poolDictionary.Remove(index);
    public void ReuseObject(int key, Vector3 position)
    {
    	ObjectInstance objectToReuse = poolDictionary[key].Dequeue();
    	poolDictionary[key].Enqueue(objectToReuse);
    	objectToReuse.Reuse(position);
    }
    //
    void OnDestroy()
    {
        foreach (int key in poolDictionary.Keys)
        {
            Queue<ObjectInstance> objectQueue = poolDictionary[key];
            foreach (ObjectInstance objectInstance in poolDictionary[key])
            {
                Destroy(objectInstance.obj);
            }
            //
            poolDictionary[key].Clear();
        }
        //
        poolDictionary.Clear();
    }
}
