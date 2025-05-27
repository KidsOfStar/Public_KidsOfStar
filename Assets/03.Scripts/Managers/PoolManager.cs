using System.Collections.Generic;
using UnityEngine;

public class PoolManager : ISceneLifecycleHandler
{
    private readonly Dictionary<string, Queue<GameObject>> poolDictionary = new();
    private readonly Dictionary<string, GameObject> prefabDictionary = new();
    private readonly Dictionary<string, Transform> poolParentDictionary = new();
    private readonly HashSet<GameObject> despawnedObjects = new();

    public void CreatePool(GameObject prefab, int poolSize)
    {
        string poolKey = prefab.name;

        if (!poolDictionary.ContainsKey(poolKey))
        {
            poolDictionary.Add(poolKey, new Queue<GameObject>());
            prefabDictionary.Add(poolKey, prefab);

            var poolRoot = new GameObject(poolKey + "Root");
            poolParentDictionary.Add(poolKey, poolRoot.transform);

            for (int i = 0; i < poolSize; i++)
            {
                var parent = poolParentDictionary[poolKey];
                GameObject obj = Object.Instantiate(prefab, parent, true);
                obj.name = poolKey;
                obj.SetActive(false);
                poolDictionary[poolKey].Enqueue(obj);
            }
        }
    }

    public GameObject Spawn(string poolKey, Vector3 position = default, Quaternion rotation = default)
    {
        if (poolDictionary.ContainsKey(poolKey))
        {
            if (poolDictionary[poolKey].Count == 0)
            {
                var prefab = prefabDictionary[poolKey];
                GameObject obj = Object.Instantiate(prefab);
                obj.name = poolKey;
                obj.SetActive(false);
                poolDictionary[poolKey].Enqueue(obj);
            }

            GameObject spawnObj = poolDictionary[poolKey].Dequeue();
            spawnObj.SetActive(true);
            spawnObj.transform.position = position;
            spawnObj.transform.rotation = rotation;
            despawnedObjects.Remove(spawnObj);

            return spawnObj;
        }

        EditorLog.LogWarning($"PoolManager: {poolKey} is not exist.");
        return null;
    }

    public GameObject Spawn(string poolKey, Transform parent)
    {
        GameObject obj = Spawn(poolKey);
        obj.transform.SetParent(parent);
        return obj;
    }

    public T Spawn<T>(string poolKey, Vector3 position = default, Quaternion rotation = default) where T : Component
    {
        GameObject obj = Spawn(poolKey, position, rotation);
        if (obj.TryGetComponent(out T component))
        {
            return component;
        }

        EditorLog.LogWarning($"PoolManager: {poolKey}<{typeof(T)}> is not exist.");
        return null;
    }
    
    public T Spawn<T>(string poolKey, Transform parent) where T : Component
    {
        GameObject obj = Spawn(poolKey, parent);
        if (obj.TryGetComponent(out T component))
        {
            return component;
        }

        EditorLog.LogWarning($"PoolManager: {poolKey}<{typeof(T)}> is not exist.");
        return null;
    }

    public void Despawn(GameObject obj)
    {
        if (despawnedObjects.Contains(obj))
        {
            EditorLog.LogWarning($"PoolManager: Object {obj.name} is already despawned.");
            return;
        }

        string poolKey = obj.name;

        if (!poolDictionary.ContainsKey(poolKey)) return;

        obj.SetActive(false);
        obj.transform.SetParent(poolParentDictionary[poolKey]);
        poolDictionary[poolKey].Enqueue(obj);
        despawnedObjects.Add(obj);
    }

    public void ClearPool()
    {
        foreach (var pool in poolDictionary)
        {
            while (pool.Value.Count > 0)
            {
                GameObject obj = pool.Value.Dequeue();
                if (!obj)
                {
                    obj.SetActive(true);
                }
            }
        }
        
        prefabDictionary.Clear();
        poolDictionary.Clear();
        poolParentDictionary.Clear();
    }

    public void OnSceneLoaded() { }
    public void OnSceneUnloaded()
    {
        ClearPool();
    }
}