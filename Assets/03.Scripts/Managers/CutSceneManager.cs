using System;

public class CutSceneManager
{
    public string CurrentCutSceneName { get; private set; }
    public bool IsCutScenePlaying { get; private set; } = false;
    public Action OnCutSceneStart { get; set; }
    public Action OnCutSceneEnd { get; set; }
    
    private CutSceneBase currentCutScene;
    private const string CutScenePath = "CutScenes/";

    public CutSceneManager()
    {
        OnCutSceneEnd += () =>
        {
            IsCutScenePlaying = false;
            currentCutScene = null;
        };
    }
    
    public void PlayCutScene(CutSceneType cutscene, Action localEndCallback = null)
    {
        CurrentCutSceneName = cutscene.GetName();
        string prefabPath = $"{CutScenePath}{cutscene.GetName()}";
        var cutSceneBase = Managers.Instance.ResourceManager.Instantiate<CutSceneBase>(prefabPath);
        
        if (!cutSceneBase)
        {
            EditorLog.Log($"컷씬 프리팹이 없습니다: {prefabPath}");
            return;
        }
        
        currentCutScene = cutSceneBase;
        IsCutScenePlaying = true;
        cutSceneBase.Play();
        OnCutSceneStart?.Invoke();
        
        if (localEndCallback != null) cutSceneBase.OnCutSceneCompleted += localEndCallback;
        cutSceneBase.Init();
    }

    public bool IsPlayingCutScene()
    {
        return currentCutScene != null;
    }
    
    public void DestroyCurrentCutScene(bool isImmediate = false)
    {
        if (!currentCutScene)
        {
            EditorLog.LogWarning("현재 재생중인 컷씬이 없습니다.");
            return;
        }

        currentCutScene.DestroyPrefab(isImmediate);
    }
}
