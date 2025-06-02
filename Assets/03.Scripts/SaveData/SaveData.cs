using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[Serializable]
public class SaveData
{
    public string saveName;
    public int difficulty;
    public int scene;

    public int chapter;
    public int chapterProgress;
    public Vector3 playerPosition;
    public int[] chapterTrust;
    
    public PlayerFormType unlockedPlayerForms;
    public PlayerFormType currentPlayerForm;
    public EndingType completedEnding;
    public int savePoint;
    public bool[,] clearedSafePuzzles = new bool[3, 3];


    public void InitData()
    {
        var gameManager = Managers.Instance.GameManager;
        difficulty = (int)gameManager.Difficulty;
        scene = (int)gameManager.CurrentScene;
        chapter = (int)gameManager.CurrentChapter;
        chapterProgress = gameManager.ChapterProgress;
        playerPosition = gameManager.Player.transform.position;
        unlockedPlayerForms = gameManager.UnlockedForms;
        currentPlayerForm = gameManager.Player.FormControl.CurFormData.playerFormType; 
        chapterTrust = gameManager.GetTrustArray();
        completedEnding = gameManager.CompletedEnding;
        savePoint = gameManager.SavePoint;
    }

    public void LoadData()
    {
        Managers.Instance.GameManager.SetLoadData(this);
    }

    public IEnumerator FetchInternetTime(Action onFetched)
    {
        DateTime internetTime = DateTime.Now;

        using (UnityWebRequest req = UnityWebRequest.Head("https://www.google.com"))
        {
            yield return req.SendWebRequest();

            string dateHeader = req.GetResponseHeader("date");
            if (DateTime.TryParse(dateHeader, out DateTime serverTime))
                internetTime = serverTime.ToLocalTime();
        }

        var difficultyName = ((Difficulty)difficulty).GetName();
        var chapterName = ((ChapterType)chapter).GetName();
        var result = internetTime.ToString("yy-MM-dd HH:mm:ss");
        // saveName = $"[{difficultyName}]{chapterName}. {result}";
        saveName = $"{chapterName}. {result}";
        onFetched?.Invoke();
    }
}