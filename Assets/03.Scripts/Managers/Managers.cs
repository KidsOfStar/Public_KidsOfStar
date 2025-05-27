using System.Collections;
using UnityEngine;

public class Managers : Singleton<Managers>
{
    [field: SerializeField] public SceneLoadManager SceneLoadManager { get; private set; }
#if UNITY_EDITOR
    [field: SerializeField] public bool IsDebugMode { get; private set; }
    [field: SerializeField] public bool LoadTestScene { get; set; } = false;
    [field: SerializeField] public SceneType TestScene { get; private set; } = SceneType.Chapter1;
#endif

    public ResourceManager ResourceManager { get; private set; }
    public DataManager DataManager { get; private set; }
    public PoolManager PoolManager { get; private set; }
    public GameManager GameManager { get; private set; }
    public UIManager UIManager { get; private set; }
    public SoundManager SoundManager { get; private set; }
    public DialogueManager DialogueManager { get; private set; }
    public SaveManager SaveManager { get; private set; }
    public CutSceneManager CutSceneManager { get; private set; }
    public AnalyticsManager AnalyticsManager { get; private set; }

    protected override void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        base.Awake();
        ResourceManager = new ResourceManager();
        DataManager = new DataManager();
        PoolManager = new PoolManager();
        GameManager = new GameManager();
		UIManager = new UIManager();
        SoundManager = new SoundManager();
        DialogueManager = new DialogueManager();
        SaveManager = new SaveManager();
        CutSceneManager = new CutSceneManager();
        AnalyticsManager = new AnalyticsManager();
#if UNITY_EDITOR
        if (!IsDebugMode) return;

        EditorLog.Log("Debug Mode");
        SceneLoadManager.DebugMode();
        OnSceneLoaded();
#endif
    }

    public void OnSceneLoaded()
    {
        UIManager.OnSceneLoaded();
        SoundManager.OnSceneLoaded();
        DialogueManager.OnSceneLoaded();
    }

    public void OnSceneUnloaded()
    {
        SoundManager.OnSceneUnloaded();
        DialogueManager.OnSceneUnloaded();
        PoolManager.OnSceneUnloaded();
    }
    
    public void CoroutineRunner(IEnumerator coroutine)
    {
        StartCoroutine(coroutine);
    }
}