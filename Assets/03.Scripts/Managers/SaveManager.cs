using System;
using System.IO;
using UnityEngine;

public class SaveManager
{
    private readonly string savePath = Application.persistentDataPath;
    private const string SaveFileName = "SaveData{0}.json";

    public void Save(int index, Action<string> onComplete)
    {
        var data = new SaveData();
        data.InitData();

        Managers.Instance.CoroutineRunner(data.FetchInternetTime(() =>
        {
            onComplete?.Invoke(data.saveName);
            var path = GetSavePath(index);
            var json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);
            EditorLog.Log("Saved data to " + path);
        }));
    }

    public void Load(int index)
    {
        var path = GetSavePath(index);
        if (!File.Exists(path))
        {
            EditorLog.LogError($"Save file not found at {path}");
            return;
        }

        var json = File.ReadAllText(path);
        var data = JsonUtility.FromJson<SaveData>(json);
        data.LoadData();
    }
    
    public void Delete(int index, string timeStamp)
    {
        var path = GetSavePath(index);
        if (File.Exists(path))
        {
            File.Delete(path);
            EditorLog.Log($"Deleted save file at {path}");
        }
        else
        {
            EditorLog.LogError($"Save file not found at {path}");
        }
    }
    
    public void DeleteAll()
    {
        var files = Directory.GetFiles(savePath, "*.json");
        foreach (var file in files)
        {
            File.Delete(file);
            EditorLog.Log($"Deleted save file at {file}");
        }
    }

    private string GetSavePath(int index)
    {
        return Path.Combine(savePath, string.Format(SaveFileName, index));
    }
}