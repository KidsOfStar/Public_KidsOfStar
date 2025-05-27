using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    public static T Instance
    {
        get
        {
            if (instance) return instance;
            instance = FindObjectOfType<T>();

            if (instance) return instance;
            GameObject singletonObject = new GameObject(typeof(T).Name);
            instance = singletonObject.AddComponent<T>();
            DontDestroyOnLoad(singletonObject);

            return instance;
        }
    }

    protected virtual void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}