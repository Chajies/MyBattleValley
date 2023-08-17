using Unity.Netcode;
using UnityEngine;

public class ObjectInstance
{
    public GameObject obj;
    public Transform form;
    public PoolObject poolObjectScript;
    //
    public ObjectInstance(GameObject objectInstance)
    {
        obj = objectInstance;
        form = obj.transform;
        obj.SetActive(false);
        poolObjectScript = obj.GetComponent<PoolObject>();
    }
    //
    public void Reuse(Vector2 position)
    {
        form.position = position;
        obj.SetActive(true);
        //
        poolObjectScript.OnObjectReuse();
    }
}
