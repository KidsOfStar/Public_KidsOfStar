using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager
{
    // Settings
    public Camera MainCamera { get; private set; }
    public float SfxVolume { get; private set; }
    public float BgmVolume { get; private set; }
    public bool IsNewGame { get; private set; } = true;

    // Stage Data
    private readonly Dictionary<ChapterType, int> trustDict = new();
    public Difficulty Difficulty { get; private set; }
    public SceneType CurrentScene { get; private set; }
    public ChapterType CurrentChapter { get; private set; }
    public int ChapterProgress { get; set; } = 1;   

    // Player Data
    public Vector3 PlayerPosition { get; private set; } = Vector3.zero;
    public Player Player { get; private set; }
    public PlayerFormType CurrentForm { get; private set; } = PlayerFormType.Human;
    public PlayerFormType UnlockedForms { get; private set; } = PlayerFormType.Stone;
    public EndingType CompletedEnding { get; private set; } = EndingType.None;

    // Events
    public Action OnProgressUpdated { get; set; }
    public Action<PlayerFormType> OnUnlockedForms { get; set; }

    // Chapter Data
    // 챕터 5에서만 사용하는 변수
    public int VisitCount { get; set; } = 0;    // 방문 횟수

    public GameManager()
    {
#if UNITY_WEBGL
        // Application.targetFrameRate = 60;
#elif UNITY_ANDROID || UNITY_IOS
        Application.targetFrameRate = 60;
#endif
        InitDictionary();
        LoadVolumeSetting();
    }

    public void SaveVolumeSetting(float bgm, float sfx)
    {
        PlayerPrefs.SetFloat("BgmVolume", bgm);
        PlayerPrefs.SetFloat("SfxVolume", sfx);
    }

    public void LoadVolumeSetting()
    {
        BgmVolume = PlayerPrefs.GetFloat("BgmVolume", 0.7f);
        SfxVolume = PlayerPrefs.GetFloat("SfxVolume", 0.7f);
    }

    private void InitDictionary()
    {
        trustDict.Clear();
        var count = Enum.GetValues(typeof(ChapterType)).Length;
        for (int i = 0; i < count; i++)
        {
            var chapter = (ChapterType)i;
            trustDict.TryAdd(chapter, 0);
        }
    }

    public void SetNewGame()
    {
        IsNewGame = true;
        UnlockedForms = 0;
        UnlockForm(PlayerFormType.Stone);
        InitDictionary();
    }

    public void SetLoadData(SaveData saveData)
    {
        IsNewGame = false;
        Difficulty = (Difficulty)saveData.difficulty;
        CurrentScene = (SceneType)saveData.scene;
        CurrentChapter = (ChapterType)saveData.chapter;
        ChapterProgress = saveData.chapterProgress;
        PlayerPosition = saveData.playerPosition;
        UnlockedForms = saveData.unlockedPlayerForms;
        CurrentForm = saveData.currentPlayerForm;
        CompletedEnding = saveData.completedEnding;
        VisitCount = saveData.visitCount;

        for (int i = 0; i < saveData.chapterTrust.Length; i++)
            trustDict[(ChapterType)i] = saveData.chapterTrust[i];
    }

    public int[] GetTrustArray()
    {
        var count = Enum.GetValues(typeof(ChapterType)).Length;
        var trustArr = new int[count];
        for (int i = 0; i < count; i++)
        {
            var chapter = (ChapterType)i;
            trustArr[i] = trustDict[chapter];
        }

        return trustArr;
    }

    public void UpdateProgress()
    {
        ChapterProgress++;
        EditorLog.Log(ChapterProgress.ToString());
        if (ChapterProgress > Managers.Instance.DataManager.GetMaxProgress(CurrentChapter))
            return;

        OnProgressUpdated?.Invoke();
    }

    public void SetChapter(ChapterType chapter)
    {
        CurrentChapter = chapter;
    }
    
    public void SetLoadedProgress()
    {
        ChapterProgress = ChapterProgress;
        IsNewGame = true;
        OnProgressUpdated?.Invoke();
    }

    public void ResetProgress()
    {
        ChapterProgress = 1;
        OnProgressUpdated?.Invoke();
    }

    // 폼이 해금 될 때 호출 할 함수
    public void UnlockForm(PlayerFormType formType)
    {
        UnlockedForms |= formType;
        OnUnlockedForms?.Invoke(formType);
    }

    // 폼이 해금 되었는지 확인하는 함수
    public bool IsFormUnlocked(PlayerFormType formType)
    {
        return (UnlockedForms & formType) == formType;
    }

    public void ModifyTrust(ChapterType chapterType, int value)
    {
        var trustData = Managers.Instance.DataManager.GetTrustData(chapterType);
        var currentValue = trustDict[chapterType];

        if (value > 0)
            trustDict[chapterType] = Mathf.Min(currentValue + value, trustData.maxTrust);
        else
            trustDict[chapterType] = Mathf.Max(currentValue + value, trustData.minTrust);
    }

    public bool EnoughTrustForEnding(ChapterType chapterType)
    {
        var trustData = Managers.Instance.DataManager.GetTrustData(chapterType);
        var currentValue = trustDict[chapterType];

        return currentValue >= trustData.endingThreshold;
    }

    public void SetCamera(Camera camera)
    {
        MainCamera = camera;
    }

    public void SetPlayer(Player player)
    {
        Player = player;
    }

    public void SetCurrentForm(PlayerFormType formType)
    {
        CurrentForm = formType;
    }

    public void SetCurrentScene(SceneType sceneType)
    {
        CurrentScene = sceneType;
    }

    public void TriggerEnding(EndingType endingType)
    {
        CompletedEnding |= endingType;
        Managers.Instance.UIManager.Show<UIEnding>(endingType);
    }
}