using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ObjectPoolManager : MonoBehaviour
{
    const int bulletPoolSize = 50;
    public GameObject bulletImpact;
    [HideInInspector] public int bulletID;
    //
    WaitForSeconds objectWait = new WaitForSeconds(0.05f);
    public Dictionary<int, Queue<ObjectInstance>> poolDictionary = new Dictionary<int, Queue<ObjectInstance>>();

    //

    void Awake() => Initialize();
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
            RemovePool(poolKey);
            CreatePool(prefab, poolSize);
        }
        //
        return poolKey;
    }
    //
    bool DictionaryHasKey(int key) => poolDictionary.ContainsKey(key);
    public void RemovePool(int index) => poolDictionary.Remove(index);
	public void ReuseObject(int key, Vector3 position)
    {
		ObjectInstance objectToReuse = poolDictionary[key].Dequeue();
		poolDictionary[key].Enqueue(objectToReuse);
		objectToReuse.Reuse(position);
	}
    //
    void Initialize()
    {
        bulletID = CreatePool(bulletImpact, bulletPoolSize);
    }
}
