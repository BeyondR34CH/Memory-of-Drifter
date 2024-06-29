using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPool<T> : Singleton<T> where T : ObjectPool<T>
{
    private static Dictionary<string, Queue<GameObject>> objectPool = new Dictionary<string, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        GameManager.OnTransScene += ClearPool;
    }

    public static GameObject GetObject(GameObject prefab)
    {
        if (!objectPool.ContainsKey(prefab.name)) CreatePool(prefab.name);
        if (objectPool[prefab.name].Count <= 0)
             return CreateObject(prefab);
        else return PopObject(prefab);
    }

    public static void PushObject(GameObject obj)
    {
        if (obj.activeSelf)
        {
            if (!objectPool.ContainsKey(obj.name))
            {
                CreatePool(obj.name);
                obj.transform.SetParent(instance.transform.Find(obj.name + "Pool"));
            }
            objectPool[obj.name].Enqueue(obj);
            obj.SetActive(false);
        }
    }

    #region ClearPool

    public static void ClearPool()
    {
        foreach (Transform obj in instance.transform) Destroy(obj.gameObject);
        objectPool.Clear();
    }

    public static void ClearPool(GameObject prefab)
    {
        Destroy(instance.transform.Find(prefab.name + "Pool").gameObject);
        objectPool.Remove(prefab.name);
    }

    public static void ClearPool(string name)
    {
        Destroy(instance.transform.Find(name + "Pool").gameObject);
        objectPool.Remove(name);
    }

    #endregion 

    private static void CreatePool(string name)
    {
        objectPool.Add(name, new Queue<GameObject>());
        GameObject childPool = new GameObject(name + "Pool");
        childPool.transform.parent = instance.transform;
        childPool.transform.localScale = Vector3.one;
    }

    private static GameObject CreateObject(GameObject prefab)
    {
        GameObject obj = Instantiate(prefab, instance.transform.Find(prefab.name + "Pool"));
        obj.name = prefab.name;
        return obj;
    }

    private static GameObject PopObject(GameObject prefab)
    {
        GameObject obj = objectPool[prefab.name].Dequeue();
        obj.SetActive(true);
        return obj;
    }
}
