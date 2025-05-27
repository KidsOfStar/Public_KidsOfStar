using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : ISceneLifecycleHandler
{
    private readonly Dictionary<string, Object> globalCache = new();
    private readonly Dictionary<string, Object> sceneCache = new();

    public T Load<T>(string path, bool isGlobal = false) where T : Object
    {
        if (isGlobal)
        {
            if (globalCache.TryGetValue(path, out var cached)) return (T)cached;
            var obj = Resources.Load<T>(path);
            globalCache[path] = obj;
            return obj;
        }
        else
        {
            if (sceneCache.TryGetValue(path, out var cached)) return (T)cached;
            var obj = Resources.Load<T>(path);
            sceneCache[path] = obj;
            return obj;
        }
    }

    public T Instantiate<T>(string path, Transform parent = null, bool isGlobal = false) where T : Object
    {
        var prefab = Load<T>(path, isGlobal);
        return prefab == null ? null : Object.Instantiate(prefab, parent);
    }

    public void UnloadSceneResource()
    {
        foreach (var obj in sceneCache.Values)
            Resources.UnloadAsset(obj);

        sceneCache.Clear();
    }

    public void OnSceneLoaded() { }
    public void OnSceneUnloaded()
    {
        UnloadSceneResource();
    }
}