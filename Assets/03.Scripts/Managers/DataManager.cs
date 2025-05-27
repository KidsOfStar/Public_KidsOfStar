using MainTable;
using System.Collections.Generic;

public class DataManager
{
    // UGS
    private readonly Dictionary<int, DialogData> dialogDatas;
    private readonly Dictionary<int, NPCData> npcDatas;
    private readonly Dictionary<int, SpecifiedAction> specifiedActionDatas;
    private readonly Dictionary<int, InteractionData> interactionDatas;
    
    // SO
    private readonly Dictionary<ChapterType, int> maxProgressDict = new();
    private Dictionary<ChapterType, RequiredIndex[]> requiredIndexDict;
    private Dictionary<ChapterType, TrustData> trustDataDict;
    
    public DataManager()
    {
        // UGS
        dialogDatas = DialogData.GetDictionary();
        npcDatas = NPCData.GetDictionary();
        specifiedActionDatas = SpecifiedAction.GetDictionary();
        interactionDatas = InteractionData.GetDictionary();
        
        // SO
        LoadChapterProgressData();
        LoadRequiredIndexData();
        LoadTrustData();
    }

    private void LoadChapterProgressData()
    {
        var data = Managers.Instance.ResourceManager.Load<ChapterData>(Define.dataPath + "ChapterData", true);
        if (data == null)
        {
            EditorLog.LogError("DataManager : ChapterData is null");
            return;
        }

        for (int i = 0; i < data.chapterProgresses.Length; i++)
        {
            var chapterProgress = data.chapterProgresses[i];
            if (chapterProgress != null)
            {
                maxProgressDict[chapterProgress.chapterType] = chapterProgress.maxProgress;
            }
        }
    }
    
    private void LoadRequiredIndexData()
    {
        // 필수 대사 인덱스
        var data = Managers.Instance.ResourceManager.Load<RequiredIndexData>(Define.dataPath + Define.requiredIndex);
        if (data == null)
        {
            EditorLog.LogError("DataManager : RequiredIndexData is null");
            return;
        }
        
        data.Init();
        requiredIndexDict = data.requiredIndexDict;
    }

    private void LoadTrustData()
    {
        var data = Managers.Instance.ResourceManager.Load<TrustValueData>(Define.dataPath + "TrustValueData");

        if (data == null)
        {
            EditorLog.LogError("DataManager : TrustValueData is null");
            return;
        }

        trustDataDict = data.GetTrustDataDict();
    }
    
    public RequiredIndex[] GetRequiredIndex(ChapterType chapterType)
    {
        if (requiredIndexDict.TryGetValue(chapterType, out var requiredIndexes))
            return requiredIndexes;
            
        else
        {
            EditorLog.LogError($"DataManager : Not found RequiredIndex with chapterType: {chapterType}");
            return null;
        }
    }

    public TrustData GetTrustData(ChapterType chapterType)
    {
        return trustDataDict[chapterType];
    }
    
    public DialogData GetDialogData(int index)
    {
        if (dialogDatas.TryGetValue(index, out var data))
            return data;
            
        else
        {
            EditorLog.LogError($"DataManager : Not found PlayerData with index: {index}");
            return null;
        }
    }
    
    public SpecifiedAction GetSpecifiedActionData(int index)
    {
        if (specifiedActionDatas.TryGetValue(index, out var data))
            return data;
            
        else
        {
            EditorLog.LogError($"DataManager : Not found SpecifiedActionData with index: {index}");
            return null;
        }
    }
    
    public InteractionData GetInteractionData(int index)
    {
        if (interactionDatas.TryGetValue(index, out var data))
            return data;
            
        else
        {
            EditorLog.LogError($"DataManager : Not found InteractionData with index: {index}");
            return null;
        }
    }

    public Dictionary<int, NPCData> GetNpcDataDict()
    {
        return npcDatas;
    }
    
    public Dictionary<int, InteractionData> GetInteractionDataDict()
    {
        return interactionDatas;
    }
    
    public int GetMaxProgress(ChapterType chapterType)
    {
        if (maxProgressDict.TryGetValue(chapterType, out var maxProgress))
            return maxProgress;
            
        else
        {
            EditorLog.LogError($"DataManager : Not found MaxProgress with chapterType: {chapterType}");
            return 0;
        }
    }
}